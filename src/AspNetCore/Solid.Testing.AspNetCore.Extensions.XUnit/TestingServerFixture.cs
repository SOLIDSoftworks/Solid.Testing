using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Solid.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit.Abstractions;

namespace Solid.Testing.AspNetCore.Extensions.XUnit
{
    public class TestingServerFixture<TStartup> : IDisposable
    {
        private static readonly string DefaultKey = string.Empty;
        private ConcurrentDictionary<string, TestingServer> _testingServers;

        public TestingServerFixture()
        {
            _testingServers = new ConcurrentDictionary<string, TestingServer>(); 
        }
        public TestingServer TestingServer => GetTestingServer(DefaultKey);

        public TestingServer GetTestingServer(string key)
            => _testingServers.GetOrAdd(key, k => InitializeTestingServer(k));

        public void SetOutput(ITestOutputHelper output)
        {
            if (CurrentOutput != null)
                throw new InvalidOperationException("Running multiple tests concurrently using single test fixture not supported.");
            CurrentOutput = output;
        }

        protected virtual void Disposing() { }

        public void Dispose()
        {
            Disposing();
            foreach (var server in _testingServers.Values)
                server.Dispose();
        }
        protected virtual void ConfigureAppConfiguration(string name, WebHostBuilderContext context, IConfigurationBuilder builder) => ConfigureAppConfiguration(context, builder);
        protected virtual void ConfigureAppConfiguration(WebHostBuilderContext context, IConfigurationBuilder builder) { }
        protected virtual void ConfigureServices(string name, IServiceCollection services) => ConfigureServices(services);
        protected virtual void ConfigureServices(IServiceCollection services) { }
        protected virtual TestingServerBuilder AddAspNetCoreHostFactory(string name, TestingServerBuilder builder, Action<IWebHostBuilder> configure)
            => AddAspNetCoreHostFactory(builder, configure);
        protected virtual TestingServerBuilder AddAspNetCoreHostFactory(TestingServerBuilder builder, Action<IWebHostBuilder> configure)
            => builder.AddAspNetCoreHostFactory(configure);
        private ITestOutputHelper CurrentOutput { get; set; }

        private TestingServer InitializeTestingServer(string name)
        {
            return 
                AddAspNetCoreHostFactory(name, new TestingServerBuilder(), builder =>
                {
                    builder
                        .ConfigureServices(services => ConfigureServices(name, services))
                        .ConfigureAppConfiguration((ctx, b) => ConfigureAppConfiguration(name, ctx, b))
                    ;
                })
                .ConfigureAspNetCoreHost(options =>
                {
                    options.OnLogMessage = (services, message) =>
                    {
                        CurrentOutput?.WriteLine(message);
                    };
                })
                .AddTestingServices(services =>
                {
                    services
                        .ConfigureSolidHttp(builder =>
                        {
                            builder.Configure(options =>
                            {
                                options.OnHttpResponse(response =>
                                {
                                    CurrentOutput = null;
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
