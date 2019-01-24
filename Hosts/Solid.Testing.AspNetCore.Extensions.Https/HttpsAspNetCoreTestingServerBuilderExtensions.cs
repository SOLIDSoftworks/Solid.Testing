using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Solid.Testing.AspNetCore.Abstractions.Factories;
using Solid.Testing.AspNetCore.Extensions.Https.Abstractions;
using Solid.Testing.AspNetCore.Extensions.Https.Factories;
using Solid.Testing.AspNetCore.Extensions.Https.Providers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Solid.Testing
{
    public static class HttpsAspNetCoreTestingServerBuilderExtensions
    {
        private static readonly string _defaultHostName = "localhost";

        public static TestingServerBuilder AddAspNetCoreHttpsHostFactory(this TestingServerBuilder builder)
            => builder.AddAspNetCoreHttpsHostFactory(_defaultHostName);
        public static TestingServerBuilder AddAspNetCoreHttpsHostFactory(this TestingServerBuilder builder, string hostname)
            => builder.AddAspNetCoreHttpsHostFactory(hostname, _ => { });
        public static TestingServerBuilder AddAspNetCoreHttpsHostFactory(this TestingServerBuilder builder, Action<IWebHostBuilder> configure)
            => builder.AddAspNetCoreHttpsHostFactory(_defaultHostName, configure);

        public static TestingServerBuilder AddAspNetCoreHttpsHostFactory(this TestingServerBuilder builder, string hostname, Action<IWebHostBuilder> configure)
        {
            builder
                .AddTestingServices(services =>
                { 
                    services
                        .AddSingleton<ISelfSignedCertificateFactory, SelfSignedCertificateFactory>()
                        .AddSingleton<IHttpClientFactory, TestingHttpClientFactory>()
                        .AddSingleton<IWebHostFactory, HttpsWebHostFactory>()
                    ;
                })
                .AddSolidHttpOptions(b => b.UseHttpClientProvider<TestingHttpClientProvider>())
            ;
            return builder.AddAspNetCoreHostFactory(hostname, configure);
        }
    }
}
