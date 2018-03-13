using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Solid.Testing.AspNetCore.Abstractions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Solid.Testing.AspNetCore.Factories
{
    internal class CertificateFactory : ICertificateFactory
    {
        private ICertificateStoreProvider _storeProvider;
        private SecureRandom _random;

        public CertificateFactory(ICertificateStoreProvider storeProvider)
        {
            _storeProvider = storeProvider;
            _random = CreateRandomNumberGenerator();
        }

        public X509Certificate2 CreateSelfSignedCertificate(string hostname, string friendlyName)
        {
            var generator = CreateGenerator();
            var subject = CreateSubject(hostname);
            generator.SetIssuerDN(subject);
            generator.SetSubjectDN(subject);
            var now = DateTime.UtcNow.AddMinutes(-5);
            var expires = now.AddYears(2);
            generator.SetNotBefore(now);
            generator.SetNotAfter(expires);
            var keyPair = CreateKeyPair();
            generator.SetPublicKey(keyPair.Public);

            var generated = generator.Generate(keyPair.Private, _random);

            var certificate = Convert(generated, keyPair, friendlyName);

            var store = _storeProvider.GetCertificateStore();
            store.Add(certificate);
            return certificate;
        }

        private SecureRandom CreateRandomNumberGenerator()
        {
            return new SecureRandom();
        }

        private X509Name CreateSubject(string hostname)
        {
            var subject = new X509Name($"DN={hostname}");
            return subject;
        }

        private AsymmetricCipherKeyPair CreateKeyPair()
        {
            const int strength = 2048;
            var parameters = new KeyGenerationParameters(_random, strength);

            var generator = new RsaKeyPairGenerator();
            generator.Init(parameters);
            var pair = generator.GenerateKeyPair();
            return pair;
        }

        private X509V3CertificateGenerator CreateGenerator()
        {
            var certificateGenerator = new X509V3CertificateGenerator();
            var serialNumber = BigIntegers.CreateRandomInRange(
                BigInteger.One,
                BigInteger.ValueOf(Int64.MaxValue), _random);
            certificateGenerator.SetSerialNumber(serialNumber);

            const string signatureAlgorithm = "SHA256WithRSA";
            certificateGenerator.SetSignatureAlgorithm(signatureAlgorithm);

            return certificateGenerator;
        }

        private X509Certificate2 Convert(Org.BouncyCastle.X509.X509Certificate generated, AsymmetricCipherKeyPair keyPair, string friendlyName)
        {
            var certificate = new X509Certificate2(generated.GetEncoded());
            certificate.PrivateKey = Convert(keyPair.Private);
            certificate.FriendlyName = friendlyName;
            return certificate;
        }

        private RSACryptoServiceProvider Convert(AsymmetricKeyParameter privateKey)
        {
            var rsa = privateKey as RsaPrivateCrtKeyParameters;

            var parameters = new RSAParameters();
            parameters.Modulus = rsa.Modulus.ToByteArrayUnsigned();
            parameters.Exponent = rsa.PublicExponent.ToByteArrayUnsigned();
            parameters.P = rsa.P.ToByteArrayUnsigned();
            parameters.Q = rsa.Q.ToByteArrayUnsigned();
            parameters.D = ConvertRsaParametersField(rsa.Exponent, parameters.Modulus.Length);
            parameters.DP = ConvertRsaParametersField(rsa.DP, parameters.P.Length);
            parameters.DQ = ConvertRsaParametersField(rsa.DQ, parameters.Q.Length);
            parameters.InverseQ = ConvertRsaParametersField(rsa.QInv, parameters.Q.Length);
            var csp = new CspParameters(1, "Microsoft Strong Cryptographic Provider", new Guid().ToString());
            var provider = new RSACryptoServiceProvider(csp);
            provider.ImportParameters(parameters);
            return provider;
        }
        private byte[] ConvertRsaParametersField(BigInteger n, int size)
        {
            byte[] bs = n.ToByteArrayUnsigned();

            if (bs.Length == size)
                return bs;

            if (bs.Length > size)
                throw new ArgumentException("Specified size too small", "size");

            byte[] padded = new byte[size];
            Array.Copy(bs, 0, padded, size - bs.Length, bs.Length);
            return padded;
        }
    }
}
