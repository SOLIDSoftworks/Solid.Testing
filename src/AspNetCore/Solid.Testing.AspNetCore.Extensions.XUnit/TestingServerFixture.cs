using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Solid.Http;
using Solid.Testing.AspNetCore.Extensions.XUnit.Logging;
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
        private static ConcurrentDictionary<Type, ITestOutputHelper> _helpers = new ConcurrentDictionary<Type, ITestOutputHelper>();
        private Lazy<TestingServer> _lazyTestingServer;

        public TestingServerFixture()
            : this(LogLevel.Debug)
        {
        }

        protected TestingServerFixture(LogLevel defaultLogLevel)
        {
            _lazyTestingServer = new Lazy<TestingServer>(InitializeTestingServer, LazyThreadSafetyMode.ExecutionAndPublication);
            DefaultLogLevel = defaultLogLevel;
        }

        public TestingServer TestingServer => _lazyTestingServer.Value;
        public LogLevel DefaultLogLevel { get; }
        public void SetOutput(ITestOutputHelper output) => _helpers.AddOrUpdate(GetType(), output, (key, _) => output);

        public void Dispose()
        {
            if (_lazyTestingServer.IsValueCreated)
                TestingServer?.Dispose();
        }

        protected virtual void ConfigureAppConfiguration(WebHostBuilderContext context, IConfigurationBuilder builder) { }
        protected virtual void ConfigureServices(IServiceCollection services) { }
        protected virtual TestingServerBuilder AddAspNetCoreHostFactory(TestingServerBuilder builder, Action<IWebHostBuilder> configure)
            => builder.AddAspNetCoreHostFactory(configure);

        private TestingServer InitializeTestingServer()
        {
            var type = GetType();
            return AddAspNetCoreHostFactory(new TestingServerBuilder(), builder =>
            {
                builder
                    .ConfigureServices(s =>
                    {
                        s.AddLogging();
                        s.AddTransient(_ => _helpers.TryGetValue(type, out var helper) ? helper : null);
                        s.AddSingleton<ILoggerProvider>(p => new XUnitLoggerProvider(p));
                    })
                    .ConfigureServices(ConfigureServices)
                    .ConfigureAppConfiguration((context, b) =>
                    {
                        var logging = new Dictionary<string, string>();
                        logging.Add("Logging__IncludeScopes", "true");
                        logging.Add("Logging__LogLevel__Default", DefaultLogLevel.ToString());
                        b.AddInMemoryCollection(logging);
                        ConfigureAppConfiguration(context, b);
                    })
                ;
            })
                .AddStartup<TStartup>()
                .Build()
            ;
        }
    }
}
