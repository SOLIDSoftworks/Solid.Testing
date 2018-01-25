﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.Abstractions
{
    internal interface IAsserter
    {
        void AreEqual<T>(T expected, T actual, string message);
        void AreEqual(object expected, object actual, string message);

        void AreNotEqual<T>(T notExpected, T actual, string message);
        void AreNotEqual(object notExpected, object actual, string message);

        void AreSame(object expected, object actual, string message);

        void AreNotSame(object notExpected, object actual, string message);

        void IsInstanceOfType<TExpected>(object actual, string message);
        void IsInstanceOfType(Type expected, object actual, string message);

        void IsNotInstanceOfType<TNotExpected>(object actual, string message);
        void IsNotInstanceOfType(Type notExpected, object actual, string message);

        void IsNull(object value, string message);

        void IsNotNull(object value, string message);

        void Fail(string message);

        void IsTrue(bool value, string message);

        void IsFalse(bool value, string message);
    }
}
