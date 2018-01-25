using Solid.Testing;
using Solid.Testing.Extensions.AspNetCore.Factories;
using System;

namespace Solid.Testing.AspNetCore
{
    public static class TestingServerBuilderExtensions
    {
        public static TestingServerBuilder AddAspNetCoreHostFactory(this TestingServerBuilder builder)
        {
            return builder.AddHostFactory(new AspNetCoreInMemoryHostFactory());            
        }
    }
}
