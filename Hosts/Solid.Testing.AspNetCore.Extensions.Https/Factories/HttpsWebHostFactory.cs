using Microsoft.AspNetCore.Hosting;
using Solid.Testing.AspNetCore.Abstractions.Factories;
using Solid.Testing.AspNetCore.Extensions.Https.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Solid.Testing.AspNetCore.Extensions.Https.Factories
{
    internal class HttpsWebHostFactory : IWebHostFactory
    {
        private ISelfSignedCertificateFactory _certificateFactory;

        public HttpsWebHostFactory(ISelfSignedCertificateFactory factory)
        {
            _certificateFactory = factory;
        }
        public IWebHost CreateWebHost(Type startup, string hostname)
        {
            var certificate = _certificateFactory.GenerateCertificate(hostname);
            return new WebHostBuilder()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, 0, listener => listener.UseHttps(certificate));
                })
                .UseStartup(startup)
                .Start()
            ;
        }
    }
}
