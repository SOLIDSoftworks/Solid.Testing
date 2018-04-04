using Microsoft.Extensions.DependencyInjection;
using Solid.Testing;
using Solid.Testing.AspNetCore.Abstractions;
using Solid.Testing.AspNetCore.Factories;
using Solid.Testing.AspNetCore.Providers;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Solid.Testing
{
    public static class TestingServerBuilderExtensions
    {
        public static TestingServerBuilder AddAspNetCoreHostFactory(this TestingServerBuilder builder, Scheme scheme = Scheme.Http, string hostname = "localhost")
        {
            //var callback = new Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>((sender, certificate, chain, sslPolicyErrors) =>
            //{
            //    return true;
            //});
            return builder.AddTestingServerBuilderServices(services =>
            {
                services
                //.AddSingleton(callback)
                .AddSingleton(new SchemeProvider(scheme))
                .AddSingleton(new HostNameProvider(hostname))
                .AddSingleton<ICertificateStoreProvider, CertificateStoreProvider>()
                .AddSingleton<ICertificateProvider, CertificateProvider>()
                .AddSingleton<ICertificateFactory, CertificateFactory>();
            }).AddHostFactory<AspNetCoreInMemoryHostFactory>();          
        }
    }
}
