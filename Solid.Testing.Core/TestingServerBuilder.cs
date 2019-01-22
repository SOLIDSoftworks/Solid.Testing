using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Solid.Http;
using Solid.Http.Abstractions;
using Solid.Testing.Abstractions;
using Solid.Testing.Services;
using System;
using System.Linq;
using System.Net.Http;

namespace Solid.Testing
{
    public class TestingServerBuilder
    {
    //    private IInMemoryHostFactory _hostFactory;
        private Action<IServiceCollection> _servicesAction = (_ => { });
        private Action<ISolidHttpBuilder> _builderAction = (_ => { });
        private Type _startup;
        
        public TestingServerBuilder AddHostFactory<TFactory>()
            where TFactory : class, IInMemoryHostFactory
        {
            AddTestingServices(services =>
            {
                var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IInMemoryHostFactory));
                if (descriptor != null)
                    throw new InvalidOperationException($"Host factory ({descriptor.ServiceType.FullName}) has already been added.");
                services.AddSingleton<IInMemoryHostFactory, TFactory>();
            });
            return this;
        }

        public TestingServerBuilder AddHostFactory(IInMemoryHostFactory factory)
        {
            AddTestingServices(services =>
            {
                var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IInMemoryHostFactory));
                if (descriptor != null)
                    throw new InvalidOperationException($"Host factory ({descriptor.ServiceType.FullName}) has already been added.");
                services.AddSingleton<IInMemoryHostFactory>(factory);
            });
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

        public TestingServerBuilder AddTestingServices(Action<IServiceCollection> action)
        {
            _servicesAction += action;
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

        public TestingServerBuilder AddSolidHttpOptions(Action<ISolidHttpBuilder> action)
        {
            _builderAction += action;
            return this;
        }

        public TestingServer Build()
        {
            var services = new ServiceCollection();
            _servicesAction(services);   
            services.AddSolidHttp(b => _builderAction(b));

            services.TryAddSingleton<IAsserter, BasicAsserter>();
            var provider = services.BuildServiceProvider();
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
