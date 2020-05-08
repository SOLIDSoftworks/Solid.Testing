# Solid.Testing [![License](https://img.shields.io/github/license/mashape/apistatus.svg)](https://en.wikipedia.org/wiki/MIT_License) [![solidsoftworks AppVeyor Build Status](https://ci.appveyor.com/api/projects/status/vy4d0qc5tp93nxhr?svg=true)](https://www.appveyor.com/)

Solid.Testing is a library used for integration testing and system testing of web apis. It's been designed to be used with AspNetCore and ASP.Net OWIN. It uses Solid.Http internally to perform HTTP requests to an in memory host.

## Packages
* [Solid.Testing.AspNetCore](https://www.nuget.org/packages/Solid.Testing.AspNetCore) (netcoreapp3.1)
* [Solid.Testing.AspNetCore.Extensions.Https](https://www.nuget.org/packages/Solid.Testing.AspNetCore.Extensions.Https) (netcoreapp3.1)
* [Solid.Testing.Owin](https://www.nuget.org/packages/Solid.Testing.Owin) (net461)
* [Solid.Testing.Core](https://www.nuget.org/packages/Solid.Testing.Core) (netstandard2.0)

## TestingServer
TestingServer is a class that wraps an in-memory instance of your api, whether it's AspNetCore or ASP.Net OWIN. A random port is chosen for the hosting and a client set up internally for communication. Once you have an instance of the TestingServer, you can perform requests and assert them using a fluid interface.

``` csharp

[Fact]
public async Task ShouldRespondWithTwoValues()
{
    await server
        // This is the fluent interface from Solid.Http 
        .GetAsync("values")
        .WithHeader("my-header", "my-header-value")

        // This is the fluent interface from Solid.Testing
        .ShouldRespondSuccessfully()
        .Should(async response =>
        {
            using(var stream = await response.ReadAsStreamAsync())
            {
                var values = JsonSerializer.Deserialize<IEnumerable<string>>(stream);
                // We use xUnit internally, so we use it for our examples. However, any unit test framework can work.
                Assert.Collection(
                  values,
                  value => Assert.Equal("value1", value),
                  value => Assert.Equal("value2", value)
                );
            }
        })
    ;
}
```

### Building the TestingServer
To build a TestingServer, you need to use the TestingServerBuilder. There are different extension methods for AspNetCore and ASP.Net OWIN. It's also possible to extend this to another self-hosted http framework.

#### AspNetCore
The basic testing server builder method is pretty simple.
```csharp
private TestingServer BuildTestingServer()
{
    return new TestingServerBuilder()
        .AddAspNetCoreHostFactory()
        // This is the startup class for your AspNetCore application
        .AddStartup<Startup>()
        .Build()
    ;
}
```
