using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Providers
{
    internal class SchemeProvider
    {
        public SchemeProvider(Scheme scheme)
        {
            Scheme = scheme;
        }

        public Scheme Scheme { get; }
    }
}
