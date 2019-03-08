using Solid.Http.Providers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Solid.Testing.AspNetCore.Extensions.Https.Providers
{
    internal class TestingHttpClientProvider : HttpClientProvider
    {
        public TestingHttpClientProvider(IHttpClientFactory factory) 
            : base(factory)
        {
        }

        protected override string GenerateHttpClientName(Uri url) => "localhost";
    }
}
