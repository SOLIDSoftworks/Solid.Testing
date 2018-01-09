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
        public InMemoryHost CreateHost<TStartup>()
        {
            return CreateHost(typeof(TStartup));
        }

        public InMemoryHost CreateHost(Type startup)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup(startup)
                .Start("http://127.0.0.1:0" /*, "http://[::1]:0"*/);

            var urls = host.ServerFeatures.Get<IServerAddressesFeature>();
            var baseAddress = urls.Addresses.Select(s => new Uri(s)).First();

            return new InMemoryHost(host, baseAddress);
        }
    }
}
