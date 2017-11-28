using Solid.Testing.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.Http;
using System.Runtime.CompilerServices;

namespace Solid.Testing.Models
{
    public class HeaderAssertion : Assertion
    {
        public HeaderAssertion(string name, AssertionContinuation continuation)
            : base(continuation.Assertion.Request)
        {
            Name = name;
            Continuation = continuation;
        }

        internal string Name { get; }
        public AssertionContinuation Continuation { get; }
    }
}
