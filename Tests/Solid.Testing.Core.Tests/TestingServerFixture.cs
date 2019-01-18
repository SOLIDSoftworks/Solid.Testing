using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Solid.Testing.Core.Tests
{
    public class TestingServerFixture : IDisposable
    {
        public TestingServerFixture()
        {
            TestingServer = new TestingServerBuilder()
                .AddAspNetCoreHostFactory(scheme: Scheme.Https)
                .AddStartup<Startup>()
                .Build();
        }

        public TestingServer TestingServer { get; }

        public void Dispose()
        {
            TestingServer.Dispose();
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

        }
        public void Configure(IApplicationBuilder builder)
        {
            builder.Use(async (context, next) =>
            {
                context.Response.Headers.Add("x-testing", bool.TrueString);
                await next();
                var json = JsonConvert.SerializeObject(new { Json = true });
                var bytes = json.ToCharArray().Select(c => (byte)c).ToArray();
                context.Response.ContentType = "application/json";
                context.Response.ContentLength = bytes.Length;
                context.Response.StatusCode = 200;
                await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            });
        }
    }
}
