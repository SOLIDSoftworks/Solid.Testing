using Solid.Testing.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Net.Http;
using Solid.Http.Abstractions;

namespace Solid.Testing.Models
{
    /// <summary>
    /// A fluent assertion object 
    /// <para>This object is awaitable</para>
    /// </summary>
    public class Assertion
    {
        /// <summary>
        /// Create an assertion
        /// </summary>
        /// <param name="request">The Solid.Http request to be performed</param>
        public Assertion(ISolidHttpRequest request)
        {
            Request = request;
        }

        /// <summary>
        /// The Solid.Http request to be performed
        /// </summary>
        public ISolidHttpRequest Request { get; }

        /// <summary>
        /// Gets the task awaiter 
        /// <para>This enables await</para>
        /// </summary>
        /// <returns>A task awaiter</returns>
        public TaskAwaiter<HttpResponseMessage> GetAwaiter()
        {
            Func<Assertion, Task<HttpResponseMessage>> waiter = (async r =>
            {
                var response = await Request;
                return response;
            });
            return waiter(this).GetAwaiter();
        }
    }
}
