using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Testing.AspNetCore.Extensions.Https.Tests
{
    public class ClientCertificateStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //services
            //    .AddAuthentication()
            //    .AddCertificate("Certificate", options =>
            //    {
            //        options.AllowedCertificateTypes = CertificateTypes.All;
            //        options.ValidateCertificateUse = false;                    
            //    })
            //;
        }

        public void Configure(IApplicationBuilder builder)
        {
            builder
                .Use(async (context, _) =>
                {
                    var factory = context.RequestServices.GetService<ILoggerFactory>();
                    var logger = factory.CreateLogger("ClientCertificateTests");

                    var certificate = await context.Connection.GetClientCertificateAsync();
                    if (certificate == null)
                    {
                        logger.LogError("No client certificate received.");
                        context.Response.StatusCode = 500;
                    }
                    else
                    {
                        logger.LogInformation($"Client certificate '{context.Connection.ClientCertificate.Subject}' received.");
                        context.Response.StatusCode = 200;
                    }
                })
            ;
        }
    }
}
