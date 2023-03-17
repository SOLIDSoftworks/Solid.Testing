﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solid.Http;
using Solid.Testing.AspNetCore.Logging;
using Solid.Testing.AspNetCore.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Xunit.Abstractions;

namespace Solid.Testing.AspNetCore.Extensions.XUnit
{
    public abstract class TestingServerFixtureBase 
    {
        private readonly ConcurrentDictionary<string, ITestOutputHelper> _helpers;
        private readonly AsyncLocal<string> _localTraceId = new AsyncLocal<string>();
        private static readonly string SpanId = ActivitySpanId.CreateRandom().ToHexString();
        protected string TraceParent => $"00-{_localTraceId.Value}-{SpanId}-00";
        
        protected TestingServerFixtureBase()
        {
            _helpers = new ConcurrentDictionary<string, ITestOutputHelper>();
        }

        public void SetOutput(ITestOutputHelper output)
        {
            var traceId = ActivityTraceId.CreateRandom().ToHexString();
            _localTraceId.Value = traceId;
            _helpers.AddOrUpdate(traceId, output, (_, __) => output);
        }

        public bool WriteLine(string message)
        {
            if (_helpers.TryGetValue(_localTraceId.Value, out var helper))
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
                    var activity = Activity.Current;
                    if (activity == null) return;
                    context.Properties.Add("id", activity.TraceId.ToHexString());
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
            request.WithHeader(HeaderNames.TraceParent, TraceParent);
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
                if (context != null && context.Properties.TryGetValue("id", out var obj) && obj is string id && _helpers.TryGetValue(id, out var helper))
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
