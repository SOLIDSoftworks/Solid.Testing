using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.Abstractions.Factories
{
    public interface IInMemoryHostFactory
    {
        IInMemoryHost CreateHost(Type startup);
    }
}
