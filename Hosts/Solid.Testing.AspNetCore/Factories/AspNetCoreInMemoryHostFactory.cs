using Solid.Testing.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.Testing.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Linq;

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
            var url = $"{_scheme}://{_hostname}:0";
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup(startup)
                .Start(url);

            var urls = host.ServerFeatures.Get<IServerAddressesFeature>();
            var baseAddress = urls.Addresses.Select(s => new Uri(s)).First();

            return new InMemoryHost(host, baseAddress);
        }
    }
}
