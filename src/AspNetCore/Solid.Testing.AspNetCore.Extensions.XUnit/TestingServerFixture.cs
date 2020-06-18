﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Solid.Http;
using Solid.Testing.AspNetCore.Logging;
using Solid.Testing.AspNetCore.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Solid.Testing.AspNetCore.Extensions.XUnit
{
    public class TestingServerFixture<TStartup> : IDisposable
    {
        private Lazy<TestingServer> _lazyTestingServer;
        private InMemoryConfigurationSource _configurationSource;
        private ConcurrentDictionary<Guid, ITestOutputHelper> _helpers;
        private AsyncLocal<Guid> _localGuid = new AsyncLocal<Guid>();
        //private int _requests = 0;
        //private bool _wait = false;

        public TestingServerFixture()
        {
            _lazyTestingServer = new Lazy<TestingServer>(InitializeTestingServer, LazyThreadSafetyMode.ExecutionAndPublication);
            _helpers = new ConcurrentDictionary<Guid, ITestOutputHelper>();
        }
        public TestingServer TestingServer => _lazyTestingServer.Value;

        public void SetOutput(ITestOutputHelper output)
        {
            _localGuid.Value = Guid.NewGuid();
            _helpers.AddOrUpdate(_localGuid.Value, output, (_, __) => output);
        }

        public void Dispose()
        {
            Disposing();
            if (_lazyTestingServer.IsValueCreated)
                _lazyTestingServer.Value.Dispose();
        }

        /// <summary>
        /// Updates the configuration for the TestingServer.
        /// <para>Disclaimer: Not all services/extensions register callbacks on the configuration change token.</para>
        /// </summary>
        /// <param name="configure"></param>
        /// <param name="clear"></param>
        public virtual void UpdateConfiguration(Action<IInMemoryConfigurationBuilderRoot> configure, bool clear = false)
        {
            var builder = new InMemoryConfigurationBuilder();
            configure(builder);
            UpdateConfiguration(builder.InMemoryConfiguration, clear);
        }

        public virtual void UpdateConfiguration(IDictionary<string, string> data, bool clear = false)
        {
            // ensure initialized
            _ = TestingServer;
            if (_configurationSource == null) return;

            //if(_requests > 0)
            //    _wait = true;
            if (clear)
                _configurationSource.Provider.Clear();

            _configurationSource.Provider.Update(data);
            _configurationSource.Provider.Load();
        }

        protected virtual void Disposing() { }
        protected virtual void ConfigureAppConfiguration(IInMemoryConfigurationBuilderRoot builder) { }
        protected virtual void ConfigureAppConfiguration(WebHostBuilderContext context, IConfigurationBuilder builder) { }
        protected virtual void ConfigureServices(IServiceCollection services) { }
        protected virtual TestingServerBuilder AddAspNetCoreHostFactory(TestingServerBuilder builder, Action<IWebHostBuilder> configure)
            => builder.AddAspNetCoreHostFactory(configure);

        protected TestingServer InitializeTestingServer()
        {
            return 
                AddAspNetCoreHostFactory(new TestingServerBuilder(), builder =>
                {
                    builder
                        .ConfigureServices(services =>
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
                            ConfigureServices(services);
                        })
                        .ConfigureAppConfiguration((ctx, b) =>
                        {
                            var inMemoryConfigurationBuilder = new InMemoryConfigurationBuilder();
                            ConfigureAppConfiguration(inMemoryConfigurationBuilder);
                            var source = inMemoryConfigurationBuilder.Build();
                            _configurationSource = source;
                            b.Sources.Add(source);

                            ConfigureAppConfiguration(ctx, b);
                        })
                    ;
                })
                .ConfigureAspNetCoreHost(options =>
                {
                    options.OnLogMessage = context =>
                    {
                        if (context.Properties.TryGetValue("id", out var id) && _helpers.TryGetValue((Guid)id, out var helper))
                            helper.WriteLine(context.Message);
                    };
                })
                .AddTestingServices(services =>
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
                                options.OnHttpResponse(async (s, r) =>
                                {
                                    var channel = s.GetRequiredService<LogMessageChannel>();
                                    if (channel.MessagesWaiting)
                                        await Task.Delay(50);
                                    _helpers.TryRemove(_localGuid.Value, out _);
                                });
                            });
                        })
                    ;
                })
                .AddStartup<TStartup>()
                .Build()
            ;
        }
    }
}
