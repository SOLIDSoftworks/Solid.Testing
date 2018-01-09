using Solid.Testing.Extensions.MsTest.Asserters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Testing.Extensions.MsTest
{
    public static class TestingServerBuilderExtensions
    {
        public static TestingServerBuilder AddMsTestAsserter(this TestingServerBuilder builder)
        {
            return builder.AddAsserter<MsTestAsserter>();
        }
    }
}
