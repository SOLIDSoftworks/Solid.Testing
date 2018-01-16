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
        public HeaderAssertion(string name, Assertion assertion)
            : base(assertion.Request)
        {
            Name = name;
            Assertion = assertion;
        }

        internal string Name { get; }
        public Assertion Assertion { get; }
    }
}
