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
        public static TestingServerBuilder AddAspNetCoreHttpsHostFactory(this TestingServerBuilder builder, string hostname = "localhost")
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
            return builder.AddAspNetCoreHostFactory(hostname: hostname);
        }
    }
}
