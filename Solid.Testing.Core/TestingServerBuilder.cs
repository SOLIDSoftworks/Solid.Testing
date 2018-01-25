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
        private IServiceCollection _services;
        private IInMemoryHostFactory _hostFactory;
        private ISolidHttpBuilder _httpBuilder;
        private Type _startup;

        public TestingServerBuilder()
        {
            _services = new ServiceCollection();
            _httpBuilder = _services.AddSolidHttp();
        }

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

        public TestingServerBuilder AddServices(Action<IServiceCollection> action)
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
            if (_hostFactory == null)
                throw new InvalidOperationException("Cannot build testing server without an InMemoryHostFactory");
            if (_startup == null)
                throw new InvalidOperationException("Cannot build testing server without Startup class");

            var provider = _services.BuildServiceProvider();
            var host = _hostFactory.CreateHost(_startup);
            return new TestingServer(host, provider);
        }
    }
}
