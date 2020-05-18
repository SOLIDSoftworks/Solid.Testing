using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private static ConcurrentDictionary<Guid, ITestOutputHelper> _helpers = new ConcurrentDictionary<Guid, ITestOutputHelper>();
        private Lazy<TestingServer> _lazyTestingServer;
        private AsyncLocal<Guid> _localGuid = new AsyncLocal<Guid> { Value = Guid.Empty };

        public TestingServerFixture()
        {
            _lazyTestingServer = new Lazy<TestingServer>(InitializeTestingServer, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public TestingServer TestingServer => _lazyTestingServer.Value;
        public void SetOutput(ITestOutputHelper output)
        {
            _localGuid.Value = Guid.NewGuid();
            _helpers.AddOrUpdate(_localGuid.Value, output, (key, _) => output);
        }

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
            const string key = "x-id";
            return 
                AddAspNetCoreHostFactory(new TestingServerBuilder(), builder =>
                {                    
                    builder
                        .ConfigureServices((context, s) =>
                        {
                            s.AddLogging(logging => logging.AddConfiguration(context.Configuration.GetSection("Logging")));
                            s.AddHttpContextAccessor();
                            s.AddTransient<ITestOutputHelper>(p =>
                            {
                                var request = p.GetService<IHttpContextAccessor>()?.HttpContext?.Request;
                                if (request == null) return null;

                                var header = request.Headers[key].ToString();
                                if (Guid.TryParse(header, out var guid) && _helpers.TryGetValue(guid, out var helper))
                                    return helper;
                                return null;
                            });
                            s.AddSingleton<ILoggerProvider, XUnitLoggerProvider>();
                        })
                        .ConfigureServices(ConfigureServices)
                        .ConfigureAppConfiguration(ConfigureAppConfiguration)
                    ;
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
                                    request.WithHeader(key, _localGuid.Value.ToString());
                                    request.BaseRequest.Properties.Add(key, _localGuid.Value);
                                });
                                options.OnHttpResponse(response =>
                                {
                                    if (!response.RequestMessage.Properties.TryGetValue(key, out var id)) return;
                                    _helpers.TryRemove((Guid)id, out _);
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
