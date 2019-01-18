using Solid.Testing.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Concurrent;

namespace Solid.Testing.Models
{
    public class Assertion
    {
        public Assertion(SolidHttpRequest request)
        {
            Request = request;
            Properties = new ConcurrentDictionary<string, object>();               
        }

        public ConcurrentDictionary<string, object> Properties { get; }

        public SolidHttpRequest Request { get; }

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
