using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Solid.Http;
using Solid.Http.Abstractions;
using Solid.Testing.Abstractions;
using Solid.Testing.Factories;
using Solid.Testing.Services;
using System;
using System.Linq;

namespace Solid.Testing
{
    public class TestingServerBuilder
    {
        private IServiceCollection _services;
        private ISolidHttpBuilder _httpBuilder;
        private Type _startup;

        public TestingServerBuilder()
        {
            _services = new ServiceCollection();
            _httpBuilder = _services.AddSolidHttp<HttpClientFactory>();
        }

        public TestingServerBuilder AddHostFactory<TFactory>()
            where TFactory : class, IInMemoryHostFactory
        {
            var descriptor = _services.FirstOrDefault(d => d.ServiceType == typeof(IInMemoryHostFactory));
            if (descriptor != null)
            {
                var type = descriptor.ImplementationType ?? descriptor.ImplementationInstance.GetType();
                throw new InvalidOperationException($"Host factory ({type.FullName}) has already been added.");
            }
            _services.AddSingleton<IInMemoryHostFactory, TFactory>();
            return this;
        }

        public TestingServerBuilder AddHostFactory(IInMemoryHostFactory factory)
        {
            var descriptor = _services.FirstOrDefault(d => d.ServiceType == typeof(IInMemoryHostFactory));
            if (descriptor != null)
            {
                var type = descriptor.ImplementationType ?? descriptor.ImplementationInstance.GetType();
                throw new InvalidOperationException($"Host factory ({type.FullName}) has already been added.");
            }
            _services.AddSingleton<IInMemoryHostFactory>(factory);
            return this;
        }

        //public TestingServerBuilder AddAsserter(IAsserter asserter)
        //{
        //    return AddServices(s => s.AddSingleton<IAsserter>(asserter));
        //}

        //public TestingServerBuilder AddAsserter<TAsserter>()
        //    where TAsserter : class, IAsserter
        //{
        //    return AddServices(s => s.AddSingleton<IAsserter, TAsserter>());
        //}
        
        public TestingServerBuilder AddTestingServerBuilderServices(Action<IServiceCollection> action)
        {
            action(_services);
            return this;
        }

        public TestingServerBuilder AddStartup<TStartup>()
        {
            return AddStartup(typeof(TStartup));
        }

        public TestingServerBuilder AddStartup(Type type)
        {
            if (_startup != null)
                throw new InvalidOperationException($"Startup type ({_startup.FullName}) has already been added.");
            _startup = type;
            return this;
        }

        public TestingServerBuilder AddSolidHttpOptions(Action<ISolidHttpOptions> action)
        {
            _httpBuilder.AddSolidHttpOptions(action);
            return this;
        }

        public TestingServer Build()
        {
            _services.TryAddSingleton<IAsserter, BasicAsserter>();
            var provider = _services.BuildServiceProvider();
            var factory = provider.GetService<IInMemoryHostFactory>();
            if (factory == null)
                throw new InvalidOperationException("Cannot build testing server without an InMemoryHostFactory");
            if (_startup == null)
                throw new InvalidOperationException("Cannot build testing server without Startup class");
            
            var host = factory.CreateHost(_startup);
            return new TestingServer(host, provider);
        }
    }
}
