using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Solid.Testing.AspNetCore.Abstractions
{
    internal interface ICertificateProvider
    {
        X509Certificate2 GetCertificate(string hostname);
    }
}
