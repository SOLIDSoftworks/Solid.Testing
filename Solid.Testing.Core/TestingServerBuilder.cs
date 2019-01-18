using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Solid.Http;
using Solid.Http.Abstractions;
using Solid.Testing.Abstractions;
using Solid.Testing.Services;
using System;
using System.Linq;

namespace Solid.Testing
{
    public class TestingServerBuilder
    {
        private IInMemoryHostFactory _hostFactory;
        private Action<IServiceCollection> _servicesAction = (_ => { });
        private Action<ISolidHttpBuilder> _builderAction = (_ => { });
        private Type _startup;
        
        public TestingServerBuilder AddHostFactory(IInMemoryHostFactory factory)
        {
            if (_hostFactory != null)
                throw new InvalidOperationException($"Host factory ({_hostFactory.GetType().FullName}) has already been added.");
            _hostFactory = factory;
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
            var services = new ServiceCollection()
                .AddSolidHttp(b => _builderAction(b));
            _servicesAction(services);            

            services.TryAddSingleton<IAsserter, BasicAsserter>();
            if (_hostFactory == null)
                throw new InvalidOperationException("Cannot build testing server without an InMemoryHostFactory");
            if (_startup == null)
                throw new InvalidOperationException("Cannot build testing server without Startup class");

            var provider = services.BuildServiceProvider();
            var host = _hostFactory.CreateHost(_startup);
            return new TestingServer(host, provider);
        }
    }
}
