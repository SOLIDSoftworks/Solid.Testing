using Solid.Testing.Abstractions;
using Solid.Testing.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Solid.Testing.Exceptions;
using System.Net;

namespace Solid.Testing
{
    /// <summary>
    /// Assertion extensions
    /// </summary>
    public static class AssertionExtensions
    {
        /// <summary>
        /// Performs an assertion against the http response message
        /// </summary>
        /// <param name="assertion">The fluent assertion</param>
        /// <param name="assert">The assertion action</param>
        /// <returns>The fluent assertion</returns>
        public static Assertion Should(this Assertion assertion, Action<HttpResponseMessage> assert)
            => assertion.Should(response =>
            {
                assert(response);
                return Task.CompletedTask;
            });

        /// <summary>
        /// Performs an async assertion against the http response message
        /// </summary>
        /// <param name="assertion">The fluent assertion</param>
        /// <param name="assert">The async assertion action</param>
        /// <returns>The fluent assertion</returns>
        public static Assertion Should(this Assertion assertion, Func<HttpResponseMessage, Task> assert)
        {
            assertion.Request.OnHttpResponse(async (provider, response) => await assert(response));
            return assertion;
        }

        /// <summary>
        /// Asserts that a header with the specifed name is in the response message
        /// </summary>
        /// <param name="assertion">The fluent assertion</param>
        /// <param name="name">The name of the header</param>
        /// <returns>A fluent header assertion</returns>
        public static HeaderAssertion ShouldHaveResponseHeader(this Assertion assertion, string name)
        {
            assertion.Should(response =>
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
                if(!contains)
                    throw new SolidTestingAssertionException($"Expected '{name}' header");
            });
            return new HeaderAssertion(name, assertion);
        }

        /// <summary>
        /// Asserts the status code of the response
        /// </summary>
        /// <param name="assertion">The fluent assertion</param>
        /// <param name="statusCode">The expected status code</param>
        /// <returns>The fluent assertion</returns>
        public static Assertion ShouldRespondWith(this Assertion assertion, HttpStatusCode statusCode)
        {
            return assertion.ShouldRespondWith((int)statusCode);
        }

        /// <summary>
        /// Asserts the status code of the response
        /// </summary>
        /// <param name="assertion">The fluent assertion</param>
        /// <param name="statusCode">The expected status code</param>
        /// <returns>The fluent assertion</returns>
        public static Assertion ShouldRespondWith(this Assertion assertion, int statusCode)
        {
            return assertion.Should(response => response.AssertStatusCode(statusCode));
        }

        /// <summary>
        /// Asserts that the status code of the response is successful (200-299)
        /// </summary>
        /// <param name="assertion">The fluent assertion</param>
        /// <returns>The fluent assertion</returns>
        public static Assertion ShouldRespondSuccessfully(this Assertion assertion)
        {
            return assertion.Should(response => response.AssertSuccessful());
        }
    }
}
