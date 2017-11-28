using Solid.Testing.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.Abstractions
{
    public interface IInMemoryHostFactory
    {
        InMemoryHost CreateHost<TStartup>();
        InMemoryHost CreateHost(Type startup);
    }
}
