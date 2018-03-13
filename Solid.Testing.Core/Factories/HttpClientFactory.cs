using Solid.Http.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Solid.Testing.Factories
{
    internal class HttpClientFactory : IHttpClientFactory
    {
        private Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> _certificateValidation;

        public HttpClientFactory(Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> certificateValidation = null)
        {
            _certificateValidation = certificateValidation;
        }
        public HttpClient Create()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = _certificateValidation;
            var client = new HttpClient(handler);
            return client;
        }
    }
}
