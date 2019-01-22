using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Solid.Testing;
using Solid.Testing.AspNetCore.Abstractions;
using Solid.Testing.AspNetCore.Abstractions.Factories;
using Solid.Testing.AspNetCore.Factories;
using Solid.Testing.AspNetCore.Options;
using Solid.Testing.Extensions.AspNetCore.Factories;
using System;
using System.Net.Http;

namespace Solid.Testing
{
    public static class AspNetCoreTestingServerBuilderExtensions
    {
        public static TestingServerBuilder AddAspNetCoreHostFactory(this TestingServerBuilder builder, string hostname = "localhost")
        {
            var options = new UrlOptions
            {
                HostName = hostname
            };
            builder.AddTestingServices(services =>
            {
                services.TryAddSingleton<IWebHostFactory, DefaultWebHostFactory>();
                services.AddSingleton(options);
            });
            return builder.AddHostFactory<AspNetCoreInMemoryHostFactory>();            
        }
    }
}
