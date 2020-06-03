using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Options
{
    public class AspNetCoreHostOptions
    {
        public string HostName { get; set; }
        public Action<IServiceProvider, string> OnLogMessage { get; set; } = (_, __) => { };
    }
}
