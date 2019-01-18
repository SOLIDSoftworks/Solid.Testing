using Solid.Testing.Abstractions;
using Solid.Testing.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Solid.Http;

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
            assertion.Request.OnResponse += (sender, args) =>
            {
                var asserter = args.Services.GetService<IAsserter>();
                assert(args.Response, asserter);
            };
            return assertion;
        }

        public static Assertion Should<T>(this Assertion assertion, Action<T, IAsserter> assert)
        {
            assertion.Request.OnResponse += (sender, args) =>
            {
                //var request = sender as SolidHttpRequest;
                //var body = assertion.Properties.GetOrAdd("response.body", )
                //request.as

                //var asserter = args.Services.GetService<IAsserter>();
                //assert(args.Response, asserter);
            };

            //if (assertion.Properties.ContainsKey("response.body"))
            //{
            //    var body = assertion.Properties["response.body"];
            //    var cast = default(T);
            //    if (body != null)
            //        cast = (T)body;
            //    assert()
            //}
            return assertion;
        }

        public static Assertion Should<T>(this Assertion assertion, T anonymous, Action<T, IAsserter> assert)
        {
            return assertion.Should<T>(assert);
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
