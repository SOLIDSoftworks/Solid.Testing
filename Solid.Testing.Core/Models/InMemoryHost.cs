using Solid.Testing.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.Models
{
    public class InMemoryHost : IInMemoryHost, IDisposable
    {
        private IDisposable _host;

        public InMemoryHost(IDisposable host, Uri baseAddress)
        {
            _host = host;
            BaseAddress = baseAddress;
        }

        public Uri BaseAddress { get; }

        public void Dispose()
        {
            _host.Dispose();
        }
    }
}
