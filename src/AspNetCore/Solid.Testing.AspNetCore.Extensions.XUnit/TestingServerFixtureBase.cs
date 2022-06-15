using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solid.Http;
using Solid.Testing.AspNetCore.Logging;
using Solid.Testing.AspNetCore.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Solid.Testing.AspNetCore.Extensions.XUnit
{
    public abstract class TestingServerFixtureBase 
    {
        private readonly ConcurrentDictionary<Guid, ITestOutputHelper> _helpers;
        private readonly AsyncLocal<Guid> _localOutputId = new AsyncLocal<Guid>();
        protected Guid OutputId => _localOutputId.Value;
        protected virtual string OutputIdHeaderName => "x-output-id";
        
        protected TestingServerFixtureBase()
        {
            _helpers = new ConcurrentDictionary<Guid, ITestOutputHelper>();
        }

        public void SetOutput(ITestOutputHelper output)
        {
            _localOutputId.Value = Guid.NewGuid();
            _helpers.AddOrUpdate(OutputId, output, (_, __) => output);
        }

        public bool WriteLine(string message)
        {
            if (_helpers.TryGetValue(OutputId, out var helper))
            {
                helper.WriteLine(message);
                return true;
            }

            return false;
        }

        protected void ConfigureRequiredServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.Configure<ChannelLoggerOptions>(options =>
            {
                options.OnCreatingLogMessage = (provider, context) =>
                {
                    var accessor = provider.GetRequiredService<IHttpContextAccessor>();
                    if (accessor.HttpContext?.Request.Headers.TryGetValue(OutputIdHeaderName, out var id) == true)
                        context.Properties.Add("id", Guid.Parse(id));
                };
            });
        }

        protected void ConfigureRequiredTestingServices(IServiceCollection services)
        {
            services
                .ConfigureSolidHttp(builder =>
                {
                    builder.Configure(options =>
                    {
                        options.OnRequestCreated(ConfigureRequest);
                        options.OnHttpRequest(LogRequestStartAsync);
                        options.OnHttpResponse(LogRequestEndAsync);
                    });
                })
            ;
        }

        protected virtual void ConfigureRequest(ISolidHttpRequest request)
        {
            request.WithHeader(OutputIdHeaderName, OutputId.ToString());
        }

        protected virtual ValueTask LogRequestStartAsync(IServiceProvider services, HttpRequestMessage request)
        {
            WriteLine("------------------ Start request ------------------");
            return new ValueTask();
        }

        protected virtual async ValueTask LogRequestEndAsync(IServiceProvider services, HttpResponseMessage response)
        {
            var channel = services.GetRequiredService<LogMessageChannel>();
            var maxWait = TimeSpan.FromMilliseconds(500);
            var maxWaitAfterCompletion = TimeSpan.FromMilliseconds(100);
            var interval = TimeSpan.FromMilliseconds(10);
            var wait = TimeSpan.Zero;
            var waitAfterCompletion = TimeSpan.Zero;
            while (channel.MessagesWaiting)
            {
                await Task.Delay(interval);
                if (channel.Completed)
                    waitAfterCompletion = waitAfterCompletion.Add(interval);
                wait = wait.Add(interval);
                if (wait > maxWait) break;
                if (waitAfterCompletion > maxWaitAfterCompletion) break;
            }

            WriteLine("------------------ End request ------------------");
        }

        protected virtual void ConfigureAspNetCoreHost(AspNetCoreHostOptions options)
        {
            options.OnLogMessage = context =>
            {
                if (context != null && context.Properties.TryGetValue("id", out var id) && _helpers.TryGetValue((Guid)id, out var helper))
                    helper.WriteLine(context.Message);
            };
        }

        internal void ConfigureAppConfiguration(IConfigurationBuilder builder, Action<IInMemoryConfigurationBuilderRoot> configure, out InMemoryConfigurationSource source)
        {
            var inMemoryConfigurationBuilder = new InMemoryConfigurationBuilder();
            configure(inMemoryConfigurationBuilder);
            source = inMemoryConfigurationBuilder.Build();
            builder.Sources.Add(source);
        }
    }
}
