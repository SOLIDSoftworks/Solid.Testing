using Solid.Testing.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.Testing.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto;
using Solid.Testing.AspNetCore.Abstractions;
using Solid.Testing.AspNetCore.Providers;

namespace Solid.Testing.AspNetCore.Factories
{
    internal class AspNetCoreInMemoryHostFactory : IInMemoryHostFactory
    {
        private Scheme _scheme;
        private string _hostname;
        private ICertificateProvider _certificateProvider;

        public AspNetCoreInMemoryHostFactory(SchemeProvider schemeProvider, HostNameProvider hostnameProvider, ICertificateProvider certificateProvider)
        {
            _scheme = schemeProvider.Scheme;
            _hostname = hostnameProvider.HostName;
            _certificateProvider = certificateProvider;
        }

        public InMemoryHost CreateHost<TStartup>()
        {
            return CreateHost(typeof(TStartup));
        }

        public InMemoryHost CreateHost(Type startup)
        {
            EnsureLocal();
            var host = new WebHostBuilder()
                .UseKestrel(options =>
                {
                    if (_scheme == Scheme.Http)
                        options.Listen(IPAddress.Loopback, 0);
                    else
                        options.Listen(IPAddress.Loopback, 0, o =>
                        {
                            o.UseHttps(_certificateProvider.GetCertificate(_hostname));
                        });
                })
                .UseStartup(startup)
                .Build();

            host.Start();
                //.Start($"{_scheme}://127.0.0.1:0");

            var urls = host.ServerFeatures.Get<IServerAddressesFeature>();
            var baseAddress = urls.Addresses.Select(s => new Uri(s)).First();
            var url = new Uri($"{Convert(_scheme)}://{_hostname}:{baseAddress.Port}");
            return new InMemoryHost(host, url);
        }

        private void EnsureLocal()
        {
            var addresses = Dns.GetHostAddresses(_hostname);
            if (!addresses.Any(a => a.Equals(IPAddress.Loopback)))
                throw new ArgumentException("Parameter must be a local host name", "hostname");
        }

        private string Convert(Scheme scheme)
        {
            return scheme.ToString().ToLower();
        }
    }
}
