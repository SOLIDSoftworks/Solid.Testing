using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                var path = ((string)context.Request.Path).Trim('/');
                var configuration = context.RequestServices.GetService<IConfiguration>();
                var section = configuration.GetSection(path);
                var value = section.GetValue<string>("Value");
                var bytes = Encoding.UTF8.GetBytes(value);
                context.Response.ContentType = "text/plain";
                context.Response.ContentLength = value?.Length;
                context.Response.StatusCode = 200;
                await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            });
        }
    }
}
