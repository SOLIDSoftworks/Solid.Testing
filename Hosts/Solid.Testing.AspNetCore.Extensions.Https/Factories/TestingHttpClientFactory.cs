using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Solid.Testing.AspNetCore.Extensions.Https.Factories
{
    internal class TestingHttpClientFactory : IHttpClientFactory
    {
        private ConcurrentDictionary<string, HttpClient> _clients = new ConcurrentDictionary<string, HttpClient>();

        public HttpClient CreateClient(string name)
        {
            return _clients.GetOrAdd(name, key =>
            {
                var handler = new HttpClientHandler();
                handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                handler.ServerCertificateCustomValidationCallback = (request, certificate, chain, policy) =>
                {
                    return certificate.Subject == $"CN={key}";
                };
                return new HttpClient(handler);
            });
        }
    }
}
