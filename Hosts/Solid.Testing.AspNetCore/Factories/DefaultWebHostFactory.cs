using Microsoft.AspNetCore.Hosting;
using Solid.Testing.AspNetCore.Abstractions.Factories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Factories
{
    internal class DefaultWebHostFactory : IWebHostFactory
    {
        public IWebHost CreateWebHost(Type startup, string hostname) =>
            new WebHostBuilder()
                   .UseKestrel()
                   .UseStartup(startup)
                   .Start("http://127.0.0.1:0");
    }
}
