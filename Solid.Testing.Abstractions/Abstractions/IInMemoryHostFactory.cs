using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.Abstractions
{
    public interface IInMemoryHostFactory
    {
        IInMemoryHost CreateHost(Type startup);
    }
}
