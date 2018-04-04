using Solid.Testing.AspNetCore.Abstractions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Solid.Testing.AspNetCore.Providers
{
    internal class CertificateStoreProvider : ICertificateStoreProvider, IDisposable
    {
        private Lazy<X509Store> _lazyStore;
        public CertificateStoreProvider()
        {
            _lazyStore = new Lazy<X509Store>(InitializeStore, LazyThreadSafetyMode.ExecutionAndPublication);
        }


        public X509Store GetCertificateStore()
        {
            return _lazyStore.Value;
        }

        private X509Store InitializeStore()
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            return store;
        }

        public void Dispose()
        {
            if (!_lazyStore.IsValueCreated) return;
            _lazyStore.Value.Close();
            _lazyStore.Value.Dispose();
        }
    }
}
