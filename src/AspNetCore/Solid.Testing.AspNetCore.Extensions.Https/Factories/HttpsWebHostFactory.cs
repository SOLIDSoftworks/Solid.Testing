using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Solid.Testing.AspNetCore.Abstractions.Factories;
using Solid.Testing.AspNetCore.Abstractions.Providers;
using Solid.Testing.AspNetCore.Extensions.Https.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Solid.Testing.AspNetCore.Extensions.Https.Factories
{
    internal class HttpsWebHostFactory : WebHostFactory
    {
        private ISelfSignedCertificateFactory _certificateFactory;
        private IWebHostOptionsProvider _provider;

        public HttpsWebHostFactory(ISelfSignedCertificateFactory factory, IWebHostOptionsProvider provider)
            : base(provider)
        {
            _certificateFactory = factory;
            _provider = provider;
        }

        protected override IWebHostBuilder InitializeWebHostBuilder(Type startup, string hostname)
        {
            var certificate = _certificateFactory.GenerateCertificate(hostname);
            return new WebHostBuilder()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, 0, listener => listener.UseHttps(options =>
                    {
                        options.ServerCertificate = certificate;
                        options.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                        options.AllowAnyClientCertificate();
                    }));
                })
                .UseStartup(startup);
        }
    }
}
