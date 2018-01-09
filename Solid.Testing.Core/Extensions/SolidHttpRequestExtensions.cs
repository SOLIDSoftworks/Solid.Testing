using Solid.Http;
using Solid.Testing.Abstractions;
using Solid.Testing.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Solid.Testing.Extensions
{
    public static class SolidHttpRequestExtensions
    {
        public static Assertion ShouldRespondWith(this SolidHttpRequest request, HttpStatusCode statusCode)
        {
            return request.ShouldRespondWith((int)statusCode);
        }

        public static Assertion ShouldRespondWith(this SolidHttpRequest request, int statusCode)
        {
            var assertion = new Assertion(request);
            assertion.Request.OnResponse += (sender, args) =>
            {
                var asserter = args.Services.GetService<IAsserter>();
                asserter.AreEqual(statusCode, (int)args.Response.StatusCode, $"Expected {statusCode} status code. Got {args.Response.StatusCode} instead.");
            };
            return assertion;
        }

        public static Assertion ShouldRespondSuccessfully(this SolidHttpRequest request)
        {
            var assertion = new Assertion(request);
            assertion.Request.OnResponse += (sender, args) =>
            {
                var asserter = args.Services.GetService<IAsserter>();
                asserter.IsTrue(args.Response.IsSuccessStatusCode, $"Expected successful status code. Got {args.Response.StatusCode} instead.");
            };
            return assertion;
        }
    }
}
