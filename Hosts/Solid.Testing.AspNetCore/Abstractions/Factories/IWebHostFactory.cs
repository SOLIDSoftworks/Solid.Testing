using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Abstractions.Factories
{
    public interface IWebHostFactory
    {
        IWebHost CreateWebHost(Type startup, string hostname);
    }
}
