using Microsoft.AspNetCore.Hosting;
using Solid.Testing.AspNetCore.Abstractions.Factories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Abstractions.Providers
{
    public interface IWebHostOptionsProvider
    {
        Action<IWebHostBuilder> Configure { get; }
    }
}
