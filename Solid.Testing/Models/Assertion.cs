using Solid.Testing.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Solid.Testing.Models
{
    public class Assertion
    {
        public Assertion(SolidHttpRequest request)
        {
            Request = request;
        }

        public SolidHttpRequest Request { get; }

        public TaskAwaiter GetAwaiter()
        {
            Func<Assertion, Task> waiter = (async r =>
            {
                await Request;
            });
            return waiter(this).GetAwaiter();
        }
    }
}
