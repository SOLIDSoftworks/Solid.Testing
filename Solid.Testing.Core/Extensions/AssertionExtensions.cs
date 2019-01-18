using Solid.Testing.Abstractions;
using Solid.Testing.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Solid.Http.Abstractions;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Solid.Testing
{
    public static class AssertionExtensions
    {
        //public static AssertionContinuation And(this Assertion assertion)
        //{
        //    return new AssertionContinuation(assertion);
        //}
        
        public static Assertion Should(this Assertion assertion, Action<HttpResponseMessage, IAsserter> assert)
        {
            assertion.Request.OnResponse((provider, response) =>
            {
                var asserter = provider.GetService<IAsserter>();
                assert(response, asserter);
            });
            return assertion;
        }

        public static HeaderAssertion ShouldHaveResponseHeader(this Assertion assertion, string name)
        {
            assertion.Should((response, asserter) =>
            {
                var contains = false;
                try
                {
                    contains = response.Headers.Contains(name);
                }
                catch (InvalidOperationException) // if checking on content header
                {
                    if (response.Content != null)
                        contains = response.Content.Headers.Contains(name);
                }
                asserter.IsTrue(contains, $"Expected '{name}' header");
            });
            return new HeaderAssertion(name, assertion);
        }
    }
}
