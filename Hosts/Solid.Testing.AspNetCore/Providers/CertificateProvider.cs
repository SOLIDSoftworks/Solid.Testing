using Solid.Testing.AspNetCore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Solid.Testing.AspNetCore.Providers
{
    internal class CertificateProvider : ICertificateProvider
    {
        private ICertificateStoreProvider _storeProvider;
        private ICertificateFactory _factory;

        public CertificateProvider(ICertificateStoreProvider storeProvider, ICertificateFactory factory)
        {
            _storeProvider = storeProvider;
            _factory = factory;
        }
        public X509Certificate2 GetCertificate(string hostname)
        {
            var store = _storeProvider.GetCertificateStore();
            var certificates = store.Certificates.Find(X509FindType.FindBySubjectName, hostname, false).Cast<X509Certificate2>();
            var certificate = certificates.FirstOrDefault(c => c.Verify());
            if (certificate == null)
                certificate = _factory.CreateSelfSignedCertificate(hostname, "Solid.Testing");
            return certificate;
        }
    }
}
