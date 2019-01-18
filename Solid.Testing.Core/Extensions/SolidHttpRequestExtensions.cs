using Solid.Http;
using Solid.Testing.Abstractions;
using Solid.Testing.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Solid.Http.Abstractions;

namespace Solid.Testing
{
    public static class SolidHttpRequestExtensions
    {
        public static Assertion ShouldRespondWith(this ISolidHttpRequest request, HttpStatusCode statusCode)
        {
            return request.ShouldRespondWith((int)statusCode);
        }

        public static Assertion ShouldRespondWith(this ISolidHttpRequest request, int statusCode)
        {
            var assertion = new Assertion(request);
            assertion.Request.OnResponse((provider, response) =>
            {
                var asserter = provider.GetService<IAsserter>();
                asserter.AreEqual(statusCode, (int)response.StatusCode, $"Expected {statusCode} status code. Got {response.StatusCode} instead.");
            });
            return assertion;
        }

        public static Assertion ShouldRespondSuccessfully(this ISolidHttpRequest request)
        {
            var assertion = new Assertion(request);
            assertion.Request.OnResponse((provider, response) =>
            {
                var asserter = provider.GetService<IAsserter>();
                asserter.IsTrue(response.IsSuccessStatusCode, $"Expected successful status code. Got {response.StatusCode} instead.");
            });
            return assertion;
        }
    }
}
