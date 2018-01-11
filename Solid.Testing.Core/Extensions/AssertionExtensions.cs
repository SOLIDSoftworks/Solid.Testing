using Solid.Testing.Abstractions;
using Solid.Testing.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing
{
    public static class AssertionExtensions
    {
        public static AssertionContinuation And(this Assertion assertion)
        {
            return new AssertionContinuation(assertion);
        }
    }
}
