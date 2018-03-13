using Solid.Testing.Extensions.Owin.Factories;
using System;

namespace Solid.Testing
{
    public static class TestingServerBuilderExtensions
    {
        public static TestingServerBuilder AddOwinHostFactory(this TestingServerBuilder builder)
        {
            return builder.AddHostFactory<OwinInMemoryHostFactory>();
        }
    }
}
