using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solid.Testing.Models
{
    public class InMemoryHost : IDisposable
    {
        private IDisposable _host;


        public InMemoryHost(IDisposable host, IEnumerable<Uri> baseAddresses)
        {
            _host = host;
            BaseAddresses = baseAddresses;
        }

        public virtual Uri BaseAddress => BaseAddresses.FirstOrDefault();
        public IEnumerable<Uri> BaseAddresses { get; }

        public void Dispose()
        {
            _host.Dispose();
        }
    }
}
