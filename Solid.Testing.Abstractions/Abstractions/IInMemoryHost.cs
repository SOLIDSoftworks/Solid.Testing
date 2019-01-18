using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.Abstractions
{
    public interface IInMemoryHost : IDisposable
    {
        Uri BaseAddress { get; }
    }
}
