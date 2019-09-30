using Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solid.Http.Abstractions;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace Solid.Testing.AspNetCore.Tests
{
    public class HostTests : IClassFixture<HostTestFixture>
    {
        private TestingServer _server;

        public HostTests(HostTestFixture fixture)
        {
            _server = fixture.TestingServer;
        }

        [Fact]
        public async Task ShouldReadJsonConfigFromApplicationFolder()
        {
            var response = await _server.Client.GetAsync("ShouldReadJsonConfigFromApplicationFolder");
            var text = await response.Content.ReadAsStringAsync();
            Assert.Equal("external", text);
        }

        [Fact]
        public async Task ShouldReadJsonConfigFromTestFolder()
        {
            var response = await _server.Client.GetAsync("ShouldReadJsonConfigFromTestFolder");
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

            var response = await _server
                .Client
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
                await _server
                    .Client
                    .GetAsync("ShouldReadJsonConfigFromTestFolder")
                    .Should(_ => Assert.True(false))
                    .Should(assertion)
                ;
            }
            catch(TrueException ex)
            {
                exception = ex;
            }
            Assert.NotNull(exception);
        }
    }
}
