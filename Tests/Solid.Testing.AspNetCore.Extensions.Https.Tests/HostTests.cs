using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Solid.Http.Abstractions;
using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Solid.Testing.AspNetCore.Extensions.Https.Tests
{
    public class HostTests
    {
        [Fact]
        public async Task ShouldServeHttps()
        {
            var server = new TestingServerBuilder()
                .AddAspNetCoreHttpsHostFactory()
                .AddStartup<Startup>()
                .Build();
            var response = await server.Client.GetAsync("/").AsText();
            var https = bool.Parse(response);
            Assert.True(https);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
            }

            public void Configure(IApplicationBuilder builder)
            {
                builder
                    .Use(async (context, next) =>
                    {
                        var bytes = Encoding.UTF8.GetBytes(context.Request.IsHttps.ToString());
                        context.Response.ContentType = "text/plain";
                        context.Response.ContentLength = context.Request.IsHttps ? 4 : 5;
                        context.Response.StatusCode = 200;
                        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    });
            }
        }
    }
}
