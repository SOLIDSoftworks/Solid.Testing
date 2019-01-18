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
    public class Assertion
    {
        public Assertion(ISolidHttpRequest request)
        {
            Request = request;
        }

        public ISolidHttpRequest Request { get; }

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
