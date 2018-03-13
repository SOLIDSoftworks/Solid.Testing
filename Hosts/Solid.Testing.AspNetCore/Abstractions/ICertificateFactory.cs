using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Solid.Testing.AspNetCore.Abstractions
{
    internal interface ICertificateFactory
    {
        X509Certificate2 CreateSelfSignedCertificate(string hostname, string friendlyName);
    }
}
