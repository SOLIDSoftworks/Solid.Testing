using Solid.Http;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Solid.Testing.Core.Tests
{
    public class DoubleRequestTests : IClassFixture<TestingServerFixture>
    {
        public DoubleRequestTests(TestingServerFixture fixture)
        {
            TestingServer = fixture.TestingServer;
        }

        public TestingServer TestingServer { get; }

        [Fact]
        public async Task ShouldNotThrowExceptionOnCheckingHeader()
        {
            await TestingServer
                .Client
                .GetAsync("")
                .ShouldRespondSuccessfully()
                .ShouldHaveResponseHeader("x-testing")
                .WithValue(bool.TrueString);

        }

        [Fact]
        public async Task ShouldNotThrowExceptionOnCheckingResponseBody()
        {
            await TestingServer
                .Client
                .GetAsync("")
                .ShouldRespondSuccessfully()
                .Should(new { Json = false }, (r, a) =>
                {
                    Assert.True(r.Json);
                });

        }
    }
}
