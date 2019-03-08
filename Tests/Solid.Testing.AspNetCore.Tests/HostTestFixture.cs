using Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Tests
{
    public class HostTestFixture : IDisposable
    {
        public HostTestFixture()
        {
            TestingServer = new TestingServerBuilder()
                .AddAspNetCoreHostFactory()
                .AddStartup<Startup>()
                .Build();
        }

        public TestingServer TestingServer { get; }

        public void Dispose()
        {
            TestingServer.Dispose();
        }
    }
}
