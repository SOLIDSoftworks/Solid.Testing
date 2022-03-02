using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solid.Http;
using Solid.Testing.AspNetCore.Logging;
using Solid.Testing.AspNetCore.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Solid.Testing.AspNetCore.Extensions.XUnit
{
    public abstract class TestingServerFixtureBase 
    {
        private ConcurrentDictionary<Guid, ITestOutputHelper> _helpers;
        private AsyncLocal<Guid> _localGuid = new AsyncLocal<Guid>();
        
        protected TestingServerFixtureBase()
        {
            _helpers = new ConcurrentDictionary<Guid, ITestOutputHelper>();
        }

        public void SetOutput(ITestOutputHelper output)
        {
            _localGuid.Value = Guid.NewGuid();
            _helpers.AddOrUpdate(_localGuid.Value, output, (_, __) => output);
        }

        public bool WriteLine(string message)
        {
            if (_helpers.TryGetValue(_localGuid.Value, out var helper))
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
                    if (accessor.HttpContext?.Request.Headers.TryGetValue("x-output-id", out var id) == true)
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
                        options.OnRequestCreated(request =>
                        {
                            var g = _localGuid.Value;
                            request.WithHeader("x-output-id", g.ToString());
                        });
                        options.OnHttpRequest(request =>
                        {
                            WriteLine("------------------ Start request ------------------");
                        });
                        options.OnHttpResponse(async (s, r) =>
                        {
                            var channel = s.GetRequiredService<LogMessageChannel>();
                            while (channel.MessagesWaiting)
                                await Task.Delay(10);
                            WriteLine("------------------ End request ------------------");
                        });
                    });
                })
            ;
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
