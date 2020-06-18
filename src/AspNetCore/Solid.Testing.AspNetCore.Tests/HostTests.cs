using AspNetCoreApplication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Solid.Http;
using Solid.Testing.AspNetCore.Extensions.XUnit;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Solid.Testing.AspNetCore.Tests
{
    public class HostTests : IClassFixture<TestingServerFixture<Startup>>
    {
        private TestingServerFixture<Startup> _fixture;

        public HostTests(TestingServerFixture<Startup> fixture, ITestOutputHelper output)
        {
            fixture.SetOutput(output);
            fixture.UpdateConfiguration(builder =>
            {
                builder
                    .SetDefaultLogLevel(LogLevel.Debug)
                    .SetLogLevel("Solid", LogLevel.Trace)
                ;
            });
            _fixture = fixture;
        }

        [Theory]
        [InlineData("custom-value")]
        [InlineData("other-value")]
        public async Task ShouldReadJsonConfigFromInMemorySource(string value)
        {
            _fixture.UpdateConfiguration(builder =>
            {
                builder.Add("ShouldReadConfigFromInMemorySource", section => section.Add("Value", value));
            });

            var response = await _fixture.TestingServer.GetAsync("ShouldReadConfigFromInMemorySource");
            var text = await response.Content.ReadAsStringAsync();
            Assert.Equal(value, text);
        }

        [Fact]
        public async Task ShouldReadJsonConfigFromApplicationFolder()
        {
            var response = await _fixture.TestingServer.GetAsync("ShouldReadJsonConfigFromApplicationFolder");
            var text = await response.Content.ReadAsStringAsync();
            Assert.Equal("external", text);
        }

        [Fact]
        public async Task ShouldReadJsonConfigFromTestFolder()
        {
            var response = await _fixture.TestingServer.GetAsync("ShouldReadJsonConfigFromTestFolder");
            var text = await response.Content.ReadAsStringAsync();
            Assert.Equal("overridden", text);
        }

        [Fact]
        public async Task ShouldRunMultipleAssertions()
        {
            var asserted1 = false;
            var assertion1 = new Action<HttpResponseMessage>(_ => asserted1 = true);
            var asserted2 = false;
            var assertion2 = new Action<HttpResponseMessage>(_ => asserted2 = true);

            var response = await _fixture.TestingServer
                .GetAsync("ShouldReadJsonConfigFromTestFolder")
                .Should(assertion1)
                .Should(assertion2)
            ;

            Assert.True(asserted1);
            Assert.True(asserted2);
        }

        [Fact]
        public async Task ShouldFailOnSingleFailedAssertion()
        {
            var assertion = new Action<HttpResponseMessage>(_ => { });

            var exception = null as Exception;
            try
            {
                await _fixture.TestingServer
                    .GetAsync("ShouldReadJsonConfigFromTestFolder")
                    .Should(_ => Assert.True(false))
                    .Should(assertion)
                ;
            }
            catch (TrueException ex)
            {
                exception = ex;
            }
            Assert.NotNull(exception);
        }

        [Theory]
        [InlineData(LogLevel.None)]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.Warning)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Critical)]
        // make sure logging doesn't interfere
        public async Task ShouldWorkOnAllLogLevels(LogLevel level)
        {
            _fixture.UpdateConfiguration(builder => builder.SetDefaultLogLevel(level), clear: true);
            await _fixture.TestingServer
                .GetAsync("ShouldReadJsonConfigFromTestFolder")
                .ShouldRespondSuccessfully()
            ;
        }
    }
}
