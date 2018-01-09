using Microsoft.AspNetCore.Hosting;
using Solid.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.Http.Abstractions;
using Solid.Testing.Abstractions;
using Solid.Testing.Models;
using Solid.Testing.Factories;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Solid.Testing.Services;

namespace Solid.Testing
{
    public class TestingServer<TStartup> : TestingServer<AspNetCoreInMemoryHostFactory, TStartup>
    {
    }


    public class TestingServer<THostFactory, TStartup> : IDisposable
        where THostFactory : IInMemoryHostFactory, new()
    {
        private Type _startup;
        private Lazy<ISolidHttpClientFactory> _lazyFactory;
        private Lazy<InMemoryHost> _lazyHost;

        public TestingServer()
        {
            _startup = typeof(TStartup);
            Services = new ServiceCollection();
            Services.AddSingleton<IAsserter, BasicAsserter>();

            _lazyFactory = new Lazy<ISolidHttpClientFactory>(InitializeFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            _lazyHost = new Lazy<InMemoryHost>(InitializeHost, LazyThreadSafetyMode.ExecutionAndPublication);

            HttpBuilder = new SolidHttpBuilder(Services);            
        }

        public IServiceCollection Services { get; }
        public SolidHttpBuilder HttpBuilder { get; }
        public SolidHttpClient Client => _lazyFactory.Value.CreateWithBaseAddress(_lazyHost.Value.BaseAddress);

        public void Dispose()
        {
            if (_lazyFactory.IsValueCreated)
                _lazyFactory.Value.Dispose();
            if (_lazyHost.IsValueCreated)
                _lazyHost.Value.Dispose();

            HttpBuilder.Dispose();
        }
        private ISolidHttpClientFactory InitializeFactory()
        {
            return HttpBuilder.Build();
        }

        private InMemoryHost InitializeHost()
        {
            return new THostFactory()
                .CreateHost(_startup);
        }
    }
}
