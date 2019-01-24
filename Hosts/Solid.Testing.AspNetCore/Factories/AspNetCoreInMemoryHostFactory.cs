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
using Solid.Testing.AspNetCore.Abstractions;
using Solid.Testing.AspNetCore.Options;
using System.Threading.Tasks;
using Solid.Testing.AspNetCore.Abstractions.Factories;
using Solid.Testing.Abstractions.Factories;

namespace Solid.Testing.Extensions.AspNetCore.Factories
{
    internal class AspNetCoreInMemoryHostFactory : IInMemoryHostFactory
    {
        private IWebHostFactory _factory;
        private string _hostname;

        public AspNetCoreInMemoryHostFactory(IWebHostFactory factory, UrlOptions options)
        {
            _factory = factory;
            _hostname = options.HostName;
        }

        public IInMemoryHost CreateHost<TStartup>()
        {
            return CreateHost(typeof(TStartup));
        }

        public IInMemoryHost CreateHost(Type startup)
        {
            EnsureLocal();

            var host = _factory.CreateWebHost(startup, _hostname);

            var urls = host.ServerFeatures.Get<IServerAddressesFeature>();
            var baseAddresses = urls.Addresses.Select(s => new Uri(s));
            var baseAddress = baseAddresses.First();
            //var baseAddress = urls.Addresses.Select(s => new Uri(s)).First();
            var url = new Uri($"{baseAddress.Scheme}://{_hostname}:{baseAddress.Port}");
            return new InMemoryHost(host, url);
        }

        private void EnsureLocal()
        {
            var addresses = Dns.GetHostAddresses(_hostname);
            if (!addresses.Any(a => a.Equals(IPAddress.Loopback)))
                throw new ArgumentException("Parameter must be a local host name", "hostname");
        }
    }
}
