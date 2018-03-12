using Solid.Testing.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.Testing.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Linq;
using System.Net;

namespace Solid.Testing.Extensions.AspNetCore.Factories
{
    internal class AspNetCoreInMemoryHostFactory : IInMemoryHostFactory
    {
        private string _scheme;
        private string _hostname;

        public AspNetCoreInMemoryHostFactory(string scheme, string hostname)
        {
            _scheme = scheme;
            _hostname = hostname;
        }

        public InMemoryHost CreateHost<TStartup>()
        {
            return CreateHost(typeof(TStartup));
        }

        public InMemoryHost CreateHost(Type startup)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup(startup)
                .Start($"{_scheme}://127.0.0.1:0");

            var urls = host.ServerFeatures.Get<IServerAddressesFeature>();
            var baseAddress = urls.Addresses.Select(s => new Uri(s)).First();
            var url = new Uri($"{_scheme}://{_hostname}:{baseAddress.Port}");
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
