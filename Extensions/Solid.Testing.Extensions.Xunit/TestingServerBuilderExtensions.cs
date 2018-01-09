using Solid.Testing.Extensions.Xunit.Asserters;
using System;

namespace Solid.Testing.Extensions.Xunit
{
    public static class TestingServerBuilderExtensions
    {
        public static TestingServerBuilder AddMsTestAsserter(this TestingServerBuilder builder)
        {
            return builder.AddAsserter<XunitAsserter>();
        }
    }
}
