using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Providers
{
    internal class HostNameProvider
    {
        public HostNameProvider(string hostname)
        {
            HostName = hostname;
        }

        public string HostName { get; }
    }
}
