using Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solid.Http.Abstractions;
using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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
    }
}
