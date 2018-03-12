using Solid.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Solid.Http.Abstractions;
using Solid.Testing.Models;

namespace Solid.Testing
{
    public class TestingServer : IDisposable
    {
        private InMemoryHost _host;
        private ServiceProvider _root;
        private List<IServiceScope> _scopes;

        internal TestingServer(InMemoryHost host, ServiceProvider provider)
        {
            _scopes = new List<IServiceScope>();
            _host = host;
            _root = provider;
        }

        public Uri BaseAddress => _host.BaseAddress;
        public IServiceProvider Provider => CreateScope().ServiceProvider;
        public SolidHttpClient Client => Provider.GetService<ISolidHttpClientFactory>().CreateWithBaseAddress(_host.BaseAddress);

        public void Dispose()
        {
            foreach (var scope in _scopes)
                scope.Dispose();
        }

        private IServiceScope CreateScope()
        {
            var scope = _root.CreateScope();
            _scopes.Add(scope);
            return scope;
        }
    }
}
