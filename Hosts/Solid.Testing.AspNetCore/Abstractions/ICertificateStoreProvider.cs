using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Solid.Testing.AspNetCore.Abstractions
{
    internal interface ICertificateStoreProvider
    {
        X509Store GetCertificateStore();
    }
}
