using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solid.Http;
using Solid.Testing.AspNetCore.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Solid.Testing.AspNetCore.Extensions.XUnit
{
    public class MultipleTestingServerFixture<TStartup>
    {
        private ConcurrentDictionary<string, TestingServer> _testingServers;
        private ConcurrentDictionary<string, InMemoryConfigurationSource> _configurationSources;
        private ConcurrentDictionary<Guid, ITestOutputHelper> _helpers;
        private AsyncLocal<Guid> _localGuid = new AsyncLocal<Guid>();

        public MultipleTestingServerFixture()
        {
            _testingServers = new ConcurrentDictionary<string, TestingServer>();
            _configurationSources = new ConcurrentDictionary<string, InMemoryConfigurationSource>();
            _helpers = new ConcurrentDictionary<Guid, ITestOutputHelper>();
        }

        public TestingServer GetTestingServer(string key)
            => _testingServers.GetOrAdd(key, k => InitializeTestingServer(k));

        public void SetOutput(ITestOutputHelper output)
        {
            _localGuid.Value = Guid.NewGuid();
            _helpers.AddOrUpdate(_localGuid.Value, output, (_, __) => output);
        }

        public void Dispose()
        {
            Disposing();
            foreach (var server in _testingServers.Values)
                server.Dispose();
        }

        public virtual void UpdateConfiguration(string name, Action<IInMemoryConfigurationBuilderRoot> configure, bool clear = false)
        {
            var builder = new InMemoryConfigurationBuilder();
            configure(builder);
            UpdateConfiguration(name, builder.InMemoryConfiguration, clear);
        }

        public virtual void UpdateConfiguration(string name, IDictionary<string, string> data, bool clear = false)
        {
            // ensure initialized
            _ = GetTestingServer(name);
            if (!_configurationSources.TryGetValue(name, out var source)) return;

            if (clear)
                source.Provider.Clear();

            source.Provider.Update(data);
            source.Provider.Load();
        }

        protected virtual void Disposing() { }
        protected virtual void ConfigureAppConfiguration(string name, WebHostBuilderContext context, IConfigurationBuilder builder) { }
        protected virtual void ConfigureAppConfiguration(string name, IInMemoryConfigurationBuilderRoot builder) { }
        protected virtual void ConfigureServices(string name, IServiceCollection services) { }
        protected virtual TestingServerBuilder AddAspNetCoreHostFactory(string name, TestingServerBuilder builder, Action<IWebHostBuilder> configure)
            => builder.AddAspNetCoreHostFactory(configure);

        protected TestingServer InitializeTestingServer(string name)
        {
            return
                AddAspNetCoreHostFactory(name, new TestingServerBuilder(), builder =>
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
                            ConfigureServices(name, services);
                        })
                        .ConfigureAppConfiguration((ctx, b) =>
                        {
                            var inMemoryConfigurationBuilder = new InMemoryConfigurationBuilder();
                            ConfigureAppConfiguration(name, inMemoryConfigurationBuilder);
                            var source = inMemoryConfigurationBuilder.Build();
                            _configurationSources.AddOrUpdate(name, source, (_, __) => source);
                            b.Sources.Add(source);

                            ConfigureAppConfiguration(name, ctx, b);
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
