using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Solid.Testing.AspNetCore.Logging;
using Solid.Testing.AspNetCore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Solid.Testing.AspNetCore.Factories
{
    internal class TestingServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        private IConfiguration _configuration;
        private LogMessageChannel _channel;

        public TestingServiceProviderFactory(IConfiguration configuration, LogMessageChannel channel)
        {
            _configuration = configuration;
            _channel = channel;
        }

        public IServiceCollection CreateBuilder(IServiceCollection services) => services;

        public IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            var options = new ServiceProviderOptions
            {
                ValidateScopes = true
            };

            // These services get injected dead last. After Startup.ConfigureServices

            services.RemoveAll<LogMessageChannel>();
            services.AddSingleton(_channel);
            services.AddSingleton<ILoggerProvider, ChannelLoggerProvider>();
            services.AddLogging(logging => logging.AddConfiguration(_configuration.GetSection("Logging")));


            return services.BuildServiceProvider(options);
        }
    }
}
