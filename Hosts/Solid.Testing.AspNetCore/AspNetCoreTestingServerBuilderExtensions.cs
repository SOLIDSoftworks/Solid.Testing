using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Solid.Testing;
using Solid.Testing.AspNetCore.Abstractions;
using Solid.Testing.AspNetCore.Abstractions.Factories;
using Solid.Testing.AspNetCore.Abstractions.Providers;
using Solid.Testing.AspNetCore.Factories;
using Solid.Testing.AspNetCore.Options;
using Solid.Testing.AspNetCore.Providers;
using Solid.Testing.Extensions.AspNetCore.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Solid.Testing
{
    public static class AspNetCoreTestingServerBuilderExtensions
    {
        private static readonly string _defaultHostName = "localhost";

        /// <summary>
        /// Adds the host factory for asp net core
        /// </summary>
        /// <param name="builder">The testing server builder</param>
        /// <returns>The testing server builder</returns>
        public static TestingServerBuilder AddAspNetCoreHostFactory(this TestingServerBuilder builder)
            => builder.AddAspNetCoreHostFactory(_defaultHostName);

        /// <summary>
        /// Adds the host factory for asp net core
        /// </summary>
        /// <param name="builder">The testing server builder</param>
        /// <param name="hostname">The hostname for the in memory host</param>
        /// <returns>The testing server builder</returns>
        public static TestingServerBuilder AddAspNetCoreHostFactory(this TestingServerBuilder builder, string hostname)
            => builder.AddAspNetCoreHostFactory(hostname, _ => { });

        /// <summary>
        /// Adds the host factory for asp net core
        /// </summary>
        /// <param name="builder">The testing server builder</param>
        /// <param name="configure">Configuration delegate for the web host</param>
        /// <returns>The testing server builder</returns>
        public static TestingServerBuilder AddAspNetCoreHostFactory(this TestingServerBuilder builder, Action<IWebHostBuilder> configure)
            => builder.AddAspNetCoreHostFactory(_defaultHostName, configure);

        /// <summary>
        /// Adds the host factory for asp net core
        /// </summary>
        /// <param name="builder">The testing server builder</param>
        /// <param name="hostname">The hostname for the in memory host</param>
        /// <param name="configure">Configuration delegate for the web host</param>
        /// <returns>The testing server builder</returns>
        public static TestingServerBuilder AddAspNetCoreHostFactory(this TestingServerBuilder builder, string hostname, Action<IWebHostBuilder> configure)
        {
            var options = new UrlOptions
            {
                HostName = hostname
            };
            builder.AddTestingServices(services =>
            {
                services.AddSingleton<IWebHostOptionsProvider>(new WebHostOptionsProvider { Configure = configure });
                services.TryAddSingleton<IWebHostFactory, DefaultWebHostFactory>();
                services.AddSingleton(options);
            });

            return builder.AddHostFactory<AspNetCoreInMemoryHostFactory>();
        }
    }
}
