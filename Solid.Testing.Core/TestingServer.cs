using Solid.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Solid.Http.Abstractions;
using Solid.Testing.Models;
using Solid.Testing.Abstractions;

namespace Solid.Testing
{
    /// <summary>
    /// The testing server
    /// </summary>
    public class TestingServer : IDisposable
    {
        private IInMemoryHost _host;
        private ServiceProvider _root;
        private List<IServiceScope> _scopes;

        internal TestingServer(IInMemoryHost host, ServiceProvider provider)
        {
            _scopes = new List<IServiceScope>();
            _host = host;
            _root = provider;
        }

        /// <summary>
        /// The base address of the testing server
        /// </summary>
        public Uri BaseAddress => _host.BaseAddress;

        /// <summary>
        /// The service provider for the server
        /// <para>This is NOT the service provider that the in memory host uses internally</para>
        /// </summary>
        public IServiceProvider Provider => CreateScope().ServiceProvider;

        /// <summary>
        /// Solid.Http http client for communication to the in memory host
        /// </summary>
        public ISolidHttpClient Client => Provider.GetService<ISolidHttpClientFactory>().CreateWithBaseAddress(_host.BaseAddress);

        /// <summary>
        /// Disposes the service scopes created for this testing server
        /// </summary>
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
