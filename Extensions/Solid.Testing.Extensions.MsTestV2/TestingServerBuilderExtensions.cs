using Solid.Testing.Extensions.MsTestV2.Asserters;
using System;

namespace Solid.Testing.Extensions.MsTestV2
{
    public static class TestingServerBuilderExtensions
    {
        public static TestingServerBuilder AddMsTestV2Asserter(this TestingServerBuilder builder)
        {
            return builder.AddAsserter<MsTestV2Asserter>();
        }
    }
}
