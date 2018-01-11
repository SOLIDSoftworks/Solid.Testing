using Solid.Testing.Abstractions;
using Solid.Testing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solid.Testing
{
    public static class HeaderAssertionExtensions
    {
        public static HeaderAssertion WithValue(this HeaderAssertion assertion, string value)
        {
            return assertion.WithValue(value, StringComparer.Ordinal);
        }
        public static HeaderAssertion WithValue(this HeaderAssertion assertion, string value, IEqualityComparer<string> comparer)
        {
            return assertion.WithValueComparer(values => values.Contains(value, comparer), "Expected value '{value}' not found.");
        }

        public static HeaderAssertion WithValueStartingWith(this HeaderAssertion assertion, string value)
        {
            return assertion.WithValueStartingWith(value, StringComparison.Ordinal);
        }

        public static HeaderAssertion WithValueStartingWith(this HeaderAssertion assertion, string value, StringComparison comparison)
        {
            return assertion.WithValueComparer(values => values.Any(v => v.StartsWith(value, comparison)), "Expected value starting with '{value}' not found.");
        }

        public static HeaderAssertion WithValueEndingWith(this HeaderAssertion assertion, string value)
        {
            return assertion.WithValueEndingWith(value, StringComparison.Ordinal);
        }

        public static HeaderAssertion WithValueEndingWith(this HeaderAssertion assertion, string value, StringComparison comparison)
        {
            return assertion.WithValueComparer(values => values.Any(v => v.EndsWith(value, comparison)), "Expected value starting with '{value}' not found.");
        }

        //public static Assertion WithValueContaining(this HeaderAssertion assertion, string value)
        //{
        //    return assertion.WithValueContaining(value, StringComparison.Ordinal);
        //}

        //public static Assertion WithValueContaining(this HeaderAssertion assertion, string value, IEqualityComparer<char> comparer)
        //{
        //    return assertion.WithValueComparer(values => values.Any(v => v.Contains(value, comparer)), "Expected value starting iwth '{value}' not found.");
        //}

        private static HeaderAssertion WithValueComparer(this HeaderAssertion assertion, Func<IEnumerable<string>, bool> compare, string message)
        {
            assertion.Continuation.Should((response, asserter) =>
            {
                var header = response.GetHeaderValues(assertion.Name);
                asserter.IsTrue(compare(header), message);
            });
            return assertion;
        }
    }
}
