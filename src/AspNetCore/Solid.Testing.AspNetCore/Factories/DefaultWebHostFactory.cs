using Microsoft.AspNetCore.Hosting;
using Solid.Testing.AspNetCore.Abstractions.Factories;
using Solid.Testing.AspNetCore.Abstractions.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Solid.Testing.AspNetCore.Factories
{
    internal class DefaultWebHostFactory : WebHostFactory
    {
        public DefaultWebHostFactory(IWebHostOptionsProvider provider)
            : base(provider)
        {
        }
        protected override IWebHostBuilder InitializeWebHostBuilder(Type startup, string hostname) =>
            new WebHostBuilder().UseKestrel(o => o.Listen(IPAddress.Loopback, 0)).UseStartup(startup);
    }
}
