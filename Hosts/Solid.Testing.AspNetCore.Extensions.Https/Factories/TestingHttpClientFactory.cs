using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Solid.Testing.AspNetCore.Extensions.Https.Factories
{
    internal class TestingHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (request, certificate, chain, policy) =>
            {
                return certificate.Subject == $"CN={name}";
            };
            return new HttpClient(handler);
        }
    }
}
