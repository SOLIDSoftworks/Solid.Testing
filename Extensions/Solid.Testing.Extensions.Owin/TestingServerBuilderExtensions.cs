using Solid.Testing.Extensions.Owin.Factories;
using System;

namespace Solid.Testing.Extensions.Owin
{
    public static class TestingServerBuilderExtensions
    {
        public static TestingServerBuilder AddOwinHostFactory(this TestingServerBuilder builder)
        {
            return builder.AddHostFactory(new OwinInMemoryHostFactory());
        }
    }
}
