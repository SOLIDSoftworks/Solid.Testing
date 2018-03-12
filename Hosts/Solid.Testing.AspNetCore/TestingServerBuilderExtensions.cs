using Solid.Testing;
using Solid.Testing.Extensions.AspNetCore.Factories;
using System;

namespace Solid.Testing
{
    public static class TestingServerBuilderExtensions
    {
        public static TestingServerBuilder AddAspNetCoreHostFactory(this TestingServerBuilder builder, string scheme = "http", string hostname = "localhost")
        {
            return builder.AddHostFactory(new AspNetCoreInMemoryHostFactory(scheme, hostname));            
        }
    }
}
