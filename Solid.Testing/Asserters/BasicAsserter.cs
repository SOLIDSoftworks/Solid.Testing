using Solid.Testing.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.Asserters
{
    internal class BasicAsserter : IAsserter
    {
        public void AreEqual<T>(T expected, T actual, string message)
        {
            if (!object.Equals(expected, actual))
                throw new BasicAssertionException(message);
        }

        public void AreEqual(object expected, object actual, string message)
        {
            if (!object.Equals(expected, actual))
                throw new BasicAssertionException(message);
        }

        public void AreNotEqual<T>(T notExpected, T actual, string message)
        {
            if (object.Equals(notExpected, actual))
                throw new BasicAssertionException(message);
        }

        public void AreNotEqual(object notExpected, object actual, string message)
        {
            if (object.Equals(notExpected, actual))
                throw new BasicAssertionException(message);
        }

        public void AreNotSame(object notExpected, object actual, string message)
        {
            if (object.ReferenceEquals(notExpected, actual))
                throw new BasicAssertionException(message);
        }

        public void AreSame(object expected, object actual, string message)
        {
            if (!object.ReferenceEquals(expected, actual))
                throw new BasicAssertionException(message);
        }

        public void Fail(string message)
        {
            throw new BasicAssertionException(message);
        }

        public void IsFalse(bool value, string message)
        {
            if (value)
                throw new BasicAssertionException(message);
        }

        public void IsInstanceOfType<TExpected>(object actual, string message)
        {
            if (!typeof(TExpected).IsAssignableFrom(actual.GetType()))
                throw new BasicAssertionException(message);
        }

        public void IsInstanceOfType(Type expected, object actual, string message)
        {
            if (!expected.IsAssignableFrom(actual.GetType()))
                throw new BasicAssertionException(message);
        }

        public void IsNotInstanceOfType<TNotExpected>(object actual, string message)
        {
            if (typeof(TNotExpected).IsAssignableFrom(actual.GetType()))
                throw new BasicAssertionException(message);
        }

        public void IsNotInstanceOfType(Type notExpected, object actual, string message)
        {
            if (notExpected.IsAssignableFrom(actual.GetType()))
                throw new BasicAssertionException(message);
        }

        public void IsNotNull(object value, string message)
        {
            if (value == null)
                throw new NotImplementedException(message);
        }

        public void IsNull(object value, string message)
        {
            if (value != null)
                throw new NotImplementedException(message);
        }

        public void IsTrue(bool value, string message)
        {
            if (!value)
                throw new NotImplementedException(message);
        }

        class BasicAssertionException : Exception
        {
            public BasicAssertionException(string message) 
                : base(message)
            {
            }
        }
    }
}
