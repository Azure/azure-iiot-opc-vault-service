using System;
using System.Collections;
using System.IO;
using static System.Security.Cryptography.X509Certificates.CertificateRequest;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Collections.Generic;
using Opc.Ua;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Numerics;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.Services.Models
{
    public class KeyVaultCertFactory
    {
        /// <summary>
        /// Creates a CA signed certificate.
        /// </summary>
        /// <returns>The signed certificate</returns>
        public static Task<X509Certificate2> CreateSignedCertificate(
            string applicationUri,
            string applicationName,
            string subjectName,
            IList<String> domainNames,
            ushort keySize,
            DateTime startTime,
            ushort lifetimeInMonths,
            ushort hashSizeInBits,
            X509Certificate2 issuerCAKeyCert,
            byte[] publicKeyArray,
            X509SignatureGenerator generator,
            bool isCA = false
            )
        {
            if (publicKeyArray == null || issuerCAKeyCert == null)
            {
                throw new NotSupportedException("Need a public key and a CA certificate.");
            }

            // set default values.
            X500DistinguishedName subjectDN = SetSuitableDefaults(
                ref applicationUri,
                ref applicationName,
                ref subjectName,
                ref domainNames,
                ref keySize,
                ref lifetimeInMonths);
            //var pb = new PublicKey();
            //var aa = new AsymmetricAlgorithm(issuerCAKeyCert.PublicKey);
            //issuerCAKeyCert.PublicKey.
            //var publicKey = new PublicKey(new Oid("1.2.840.113549.1.1.1"), issuerCAKeyCert.PublicKey.EncodedKeyValue, new AsnEncodedData(publicKeyArray));
            //RSAParameters parameters = rsa.ExportParameters(false);
            //byte[] rsaPublicKey = DerEncoder.ConstructSequence(
            //    DerEncoder.SegmentedEncodeUnsignedInteger(parameters.Modulus),
            //    DerEncoder.SegmentedEncodeUnsignedInteger(parameters.Exponent));

            const string RsaRsa = "1.2.840.113549.1.1.1";
            Oid oid = new Oid(RsaRsa);
            var encodedPublicKey = new AsnEncodedData(oid, publicKeyArray);
            var content = encodedPublicKey.Format(true);
            var publicKey = new PublicKey(oid, new AsnEncodedData(oid, new byte[] { 0x05, 0x00 }), encodedPublicKey);
            RSA rsa = RSA.Create();
            //X509SignatureGenerator generator = X509SignatureGenerator.CreateForRSA(issuerCAKeyCert.GetRSAPrivateKey(), RSASignaturePadding.Pkcs1);
            //KeyVaultSignatureGenerator kvGenerator = new KeyVaultSignatureGenerator(issuerCAKeyCert);

            //RSAParameters parameters = new RSAParameters();
            //parameters.Exponent =
            //parameters.Modulus = 
            //rsa.ImportParameters(parameters);
            var provider = DecodeX509PublicKey(publicKeyArray);
            //var generator = new X509SignatureGenerator();
            var request = new CertificateRequest(subjectDN, provider, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            request.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(false, false, 0, false));
            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.DigitalSignature |
                    X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.KeyEncipherment, true));
            request.CertificateExtensions.Add(
                new X509EnhancedKeyUsageExtension(
                    new OidCollection {
                        new Oid("1.3.6.1.5.5.7.3.1"),
                        new Oid("1.3.6.1.5.5.7.3.2") }, false));
            X509Certificate2 signedCert = request.Create(
                issuerCAKeyCert.SubjectName,
                generator,
                startTime,
                startTime.AddDays(lifetimeInMonths),
                new byte[] { 0, 1, 2 }  // serial number
                );

#if SIGNKEYLOCAL
            RSAParameters parameters = new RSAParameters();
            //parameters.Exponent =
            //parameters.Modulus = 
            //rsa.ImportParameters(parameters);
            var provider = DecodeX509PublicKey(publicKeyArray);
            //var generator = new X509SignatureGenerator();
            var request = new CertificateRequest(subjectDN, provider, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            request.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(false, false, 0, false));
            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.DigitalSignature |
                    X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.KeyEncipherment, true));
            request.CertificateExtensions.Add(
                new X509EnhancedKeyUsageExtension(
                    new OidCollection { new Oid("1.3.6.1.5.5.7.3.2") }, false));

            X509Certificate2 signedCert = request.Create(
                issuerCAKeyCert,
                startTime,
                startTime.AddDays(lifetimeInMonths),
                new byte[] { 0, 1, 2 }
                );
#endif

#if NEWKEYPAIR
            var request = new CertificateRequest(subjectDN, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            X509Certificate2 signedCert = request.Create(
                issuerCAKeyCert,
                startTime,
                startTime.AddDays(lifetimeInMonths),
                new byte[] { 0, 1, 2 }
                );
#endif
#if ALT
            using (var cfrg = new CertificateFactoryRandomGenerator())
            {
                // cert generators
                SecureRandom random = new SecureRandom(cfrg);
                X509V3CertificateGenerator cg = new X509V3CertificateGenerator();

                // Serial Number
                BigInteger serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
                cg.SetSerialNumber(serialNumber);

                // subject and issuer DN
                X509Name issuerDN = null;
                if (issuerCAKeyCert != null)
                {
                    issuerDN = new CertificateFactoryX509Name(issuerCAKeyCert.Subject);
                }
                else
                {
                    // self signed 
                    issuerDN = subjectDN;
                }
                cg.SetIssuerDN(issuerDN);
                cg.SetSubjectDN(subjectDN);

                // valid for
                cg.SetNotBefore(startTime);
                cg.SetNotAfter(startTime.AddMonths(lifetimeInMonths));

                // set Private/Public Key
                AsymmetricKeyParameter subjectPublicKey;
                AsymmetricKeyParameter subjectPrivateKey;
                if (publicKey == null)
                {
                    var keyGenerationParameters = new KeyGenerationParameters(random, keySize);
                    var keyPairGenerator = new RsaKeyPairGenerator();
                    keyPairGenerator.Init(keyGenerationParameters);
                    AsymmetricCipherKeyPair subjectKeyPair = keyPairGenerator.GenerateKeyPair();
                    subjectPublicKey = subjectKeyPair.Public;
                    subjectPrivateKey = subjectKeyPair.Private;
                }
                else
                {
                    // special case, if a cert is signed by CA, the private key of the cert is not needed
                    subjectPublicKey = PublicKeyFactory.CreateKey(publicKey);
                    subjectPrivateKey = null;
                }
                cg.SetPublicKey(subjectPublicKey);

                // add extensions
                // Subject key identifier
                cg.AddExtension(X509Extensions.SubjectKeyIdentifier.Id, false,
                    new SubjectKeyIdentifier(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(subjectPublicKey)));

                // Basic constraints
                cg.AddExtension(X509Extensions.BasicConstraints.Id, true, new BasicConstraints(isCA));

                // Authority Key identifier references the issuer cert or itself when self signed
                AsymmetricKeyParameter issuerPublicKey;
                BigInteger issuerSerialNumber;
                issuerPublicKey = GetPublicKeyParameter(issuerCAKeyCert);
                issuerSerialNumber = GetSerialNumber(issuerCAKeyCert);

                cg.AddExtension(X509Extensions.AuthorityKeyIdentifier.Id, false,
                    new AuthorityKeyIdentifier(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(issuerPublicKey),
                        new GeneralNames(new GeneralName(issuerDN)), issuerSerialNumber));

                if (!isCA)
                {
                    // Key usage 
                    cg.AddExtension(X509Extensions.KeyUsage, true,
                        new KeyUsage(KeyUsage.DataEncipherment | KeyUsage.DigitalSignature |
                            KeyUsage.NonRepudiation | KeyUsage.KeyCertSign | KeyUsage.KeyEncipherment));

                    // Extended Key usage
                    cg.AddExtension(X509Extensions.ExtendedKeyUsage, true,
                        new ExtendedKeyUsage(new List<DerObjectIdentifier>() {
                    new DerObjectIdentifier("1.3.6.1.5.5.7.3.1"), // server auth
                    new DerObjectIdentifier("1.3.6.1.5.5.7.3.2"), // client auth
                        }));

                    // subject alternate name
                    List<GeneralName> generalNames = new List<GeneralName>();
                    generalNames.Add(new GeneralName(GeneralName.UniformResourceIdentifier, applicationUri));
                    generalNames.AddRange(CreateSubjectAlternateNameDomains(domainNames));
                    cg.AddExtension(X509Extensions.SubjectAlternativeName, false, new GeneralNames(generalNames.ToArray()));
                }
                else
                {
                    // Key usage CA
                    cg.AddExtension(X509Extensions.KeyUsage, true,
                        new KeyUsage(KeyUsage.CrlSign | KeyUsage.DigitalSignature | KeyUsage.KeyCertSign));
                }

                // sign certificate
                AsymmetricKeyParameter signingKey;
                if (issuerCAKeyCert != null)
                {
                    // signed by issuer
                    signingKey = null; // GetPrivateKeyParameter(issuerCAKeyCert);
                }
                else
                {
                    // self signed
                    signingKey = subjectPrivateKey;
                }
                ISignatureFactory signatureFactory =
                            new Asn1SignatureFactory(GetRSAHashAlgorithm(hashSizeInBits), signingKey, random);
                Org.BouncyCastle.X509.X509Certificate x509 = cg.Generate(signatureFactory);

                // convert to X509Certificate2
                X509Certificate2 certificate = null;
                if (subjectPrivateKey == null)
                {
                    // create the cert without the private key
                    certificate = new X509Certificate2(x509.GetEncoded());
                }

                Utils.Trace(Utils.TraceMasks.Security, "Created new certificate: {0}", certificate.Thumbprint);
                return Task.FromResult(certificate);
#endif
            return Task.FromResult<X509Certificate2>(signedCert);
        }

        public static RSACryptoServiceProvider DecodeX509PublicKey(byte[] x509key)
        {
            byte[] SeqOID = { 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01 };

            MemoryStream ms = new MemoryStream(x509key);
            BinaryReader reader = new BinaryReader(ms);

            if (reader.ReadByte() == 0x30)
                ReadASNLength(reader); //skip the size
            else
                return null;

            int identifierSize = 0; //total length of Object Identifier section
            if (reader.ReadByte() == 0x30)
                identifierSize = ReadASNLength(reader);
            else
                return null;

            if (reader.ReadByte() == 0x06) //is the next element an object identifier?
            {
                int oidLength = ReadASNLength(reader);
                byte[] oidBytes = new byte[oidLength];
                reader.Read(oidBytes, 0, oidBytes.Length);
                if (oidBytes.SequenceEqual(SeqOID) == false) //is the object identifier rsaEncryption PKCS#1?
                    return null;

                int remainingBytes = identifierSize - 2 - oidBytes.Length;
                reader.ReadBytes(remainingBytes);
            }

            if (reader.ReadByte() == 0x03) //is the next element a bit string?
            {
                ReadASNLength(reader); //skip the size
                reader.ReadByte(); //skip unused bits indicator
                if (reader.ReadByte() == 0x30)
                {
                    ReadASNLength(reader); //skip the size
                    if (reader.ReadByte() == 0x02) //is it an integer?
                    {
                        int modulusSize = ReadASNLength(reader);
                        byte[] modulus = new byte[modulusSize];
                        reader.Read(modulus, 0, modulus.Length);
                        if (modulus[0] == 0x00) //strip off the first byte if it's 0
                        {
                            byte[] tempModulus = new byte[modulus.Length - 1];
                            Array.Copy(modulus, 1, tempModulus, 0, modulus.Length - 1);
                            modulus = tempModulus;
                        }

                        if (reader.ReadByte() == 0x02) //is it an integer?
                        {
                            int exponentSize = ReadASNLength(reader);
                            byte[] exponent = new byte[exponentSize];
                            reader.Read(exponent, 0, exponent.Length);

                            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                            RSAParameters rsaKeyInfo = new RSAParameters();
                            rsaKeyInfo.Modulus = modulus;
                            rsaKeyInfo.Exponent = exponent;
                            rsa.ImportParameters(rsaKeyInfo);
                            return rsa;
                        }
                    }
                }
            }
            return null;
        }

        public static int ReadASNLength(BinaryReader reader)
        {
            //Note: this method only reads lengths up to 4 bytes long as
            //this is satisfactory for the majority of situations.
            int length = reader.ReadByte();
            if ((length & 0x00000080) == 0x00000080) //is the length greater than 1 byte
            {
                int count = length & 0x0000000f;
                byte[] lengthBytes = new byte[4];
                reader.Read(lengthBytes, 4 - count, count);
                Array.Reverse(lengthBytes); //
                length = BitConverter.ToInt32(lengthBytes, 0);
            }
            return length;
        }

        /// <summary>
        /// Sets the parameters to suitable defaults.
        /// </summary>
        private static X500DistinguishedName SetSuitableDefaults(
            ref string applicationUri,
            ref string applicationName,
            ref string subjectName,
            ref IList<String> domainNames,
            ref ushort keySize,
            ref ushort lifetimeInMonths)
        {
            // enforce recommended keysize unless lower value is enforced.
            if (keySize < 1024)
            {
                keySize = CertificateFactory.DefaultKeySize;
            }

            if (keySize % 1024 != 0)
            {
                throw new ArgumentNullException("keySize", "KeySize must be a multiple of 1024.");
            }

            // enforce minimum lifetime.
            if (lifetimeInMonths < 1)
            {
                lifetimeInMonths = 1;
            }

            // parse the subject name if specified.
            List<string> subjectNameEntries = null;

            if (!String.IsNullOrEmpty(subjectName))
            {
                subjectNameEntries = Utils.ParseDistinguishedName(subjectName);
            }

            // check the application name.
            if (String.IsNullOrEmpty(applicationName))
            {
                if (subjectNameEntries == null)
                {
                    throw new ArgumentNullException("applicationName", "Must specify a applicationName or a subjectName.");
                }

                // use the common name as the application name.
                for (int ii = 0; ii < subjectNameEntries.Count; ii++)
                {
                    if (subjectNameEntries[ii].StartsWith("CN="))
                    {
                        applicationName = subjectNameEntries[ii].Substring(3).Trim();
                        break;
                    }
                }
            }

            if (String.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentNullException("applicationName", "Must specify a applicationName or a subjectName.");
            }

            // remove special characters from name.
            StringBuilder buffer = new StringBuilder();

            for (int ii = 0; ii < applicationName.Length; ii++)
            {
                char ch = applicationName[ii];

                if (Char.IsControl(ch) || ch == '/' || ch == ',' || ch == ';')
                {
                    ch = '+';
                }

                buffer.Append(ch);
            }

            applicationName = buffer.ToString();

            // ensure at least one host name.
            if (domainNames == null || domainNames.Count == 0)
            {
                domainNames = new List<string>();
                domainNames.Add(Utils.GetHostName());
            }

            // create the application uri.
            if (String.IsNullOrEmpty(applicationUri))
            {
                StringBuilder builder = new StringBuilder();

                builder.Append("urn:");
                builder.Append(domainNames[0]);
                builder.Append(":");
                builder.Append(applicationName);

                applicationUri = builder.ToString();
            }

            Uri uri = Utils.ParseUri(applicationUri);

            if (uri == null)
            {
                throw new ArgumentNullException(nameof(applicationUri), "Must specify a valid URL.");
            }

            // create the subject name,
            if (String.IsNullOrEmpty(subjectName))
            {
                subjectName = Utils.Format("CN={0}", applicationName);
            }

            if (!subjectName.Contains("CN="))
            {
                subjectName = Utils.Format("CN={0}", subjectName);
            }

            if (domainNames != null && domainNames.Count > 0)
            {
                if (!subjectName.Contains("DC=") && !subjectName.Contains("="))
                {
                    subjectName += Utils.Format(", DC={0}", domainNames[0]);
                }
                else
                {
                    subjectName = Utils.ReplaceDCLocalhost(subjectName, domainNames[0]);
                }
            }

            return new X500DistinguishedName(subjectName);
        }

#if ALT
        /// <summary>
        /// helper to get the Bouncy Castle hash algorithm name by hash size in bits.
        /// </summary>
        /// <param name="hashSizeInBits"></param>
        private static string GetRSAHashAlgorithm(uint hashSizeInBits)
        {
            if (hashSizeInBits <= 160)
                return "SHA1WITHRSA";
            if (hashSizeInBits <= 224)
                return "SHA224WITHRSA";
            else if (hashSizeInBits <= 256)
                return "SHA256WITHRSA";
            else if (hashSizeInBits <= 384)
                return "SHA384WITHRSA";
            else
                return "SHA512WITHRSA";
        }

        private static RsaKeyParameters GetPublicKeyParameter(X509Certificate2 certificate)
        {
            RSA rsa = null;
            try
            {
                rsa = certificate.GetRSAPublicKey();
                RSAParameters rsaParams = rsa.ExportParameters(false);
                return new RsaKeyParameters(
                    false,
                    new BigInteger(1, rsaParams.Modulus),
                    new BigInteger(1, rsaParams.Exponent));
            }
            finally
            {
                RsaUtils.RSADispose(rsa);
            }
        }

        /// <summary>
        /// helper to build alternate name domains list for certs.
        /// </summary>
        private static List<GeneralName> CreateSubjectAlternateNameDomains(IList<String> domainNames)
        {
            // subject alternate name
            List<GeneralName> generalNames = new List<GeneralName>();
            for (int i = 0; i < domainNames.Count; i++)
            {
                int domainType = GeneralName.OtherName;
                switch (Uri.CheckHostName(domainNames[i]))
                {
                    case UriHostNameType.Dns: domainType = GeneralName.DnsName; break;
                    case UriHostNameType.IPv4:
                    case UriHostNameType.IPv6: domainType = GeneralName.IPAddress; break;
                    default: continue;
                }
                generalNames.Add(new GeneralName(domainType, domainNames[i]));
            }
            return generalNames;
        }

        /// <summary>
        /// Get the serial number from a certificate as BigInteger.
        /// </summary>
        private static BigInteger GetSerialNumber(X509Certificate2 certificate)
        {
            byte[] serialNumber = certificate.GetSerialNumber();
            Array.Reverse(serialNumber);
            return new BigInteger(1, serialNumber);
        }
#endif
        public class KeyVaultSignatureGenerator : X509SignatureGenerator
        {
            //private readonly RSA _key;
            X509Certificate2 _issuerCert;
            KeyVaultServiceClient _keyVaultServiceClient;
            string _signingKey;

            public KeyVaultSignatureGenerator(
                KeyVaultServiceClient keyVaultServiceClient,
                string signingKey,
                X509Certificate2 issuerCertificate)
            {
                _issuerCert = issuerCertificate;
                _keyVaultServiceClient = keyVaultServiceClient;
                _signingKey = signingKey;
                GetSignatureAlgorithmIdentifier(HashAlgorithmName.SHA256);
                GetSignatureAlgorithmIdentifier(HashAlgorithmName.SHA384);
                GetSignatureAlgorithmIdentifier(HashAlgorithmName.SHA512);
                //_key = key;
            }

            public override byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm)
            {
                var hash = SHA256.Create();
                var digest = hash.ComputeHash(data);
                var resultKeyVaultPkcs = _keyVaultServiceClient.SignDigestAsync(_signingKey, digest, hashAlgorithm, RSASignaturePadding.Pkcs1).Result;
                if (_issuerCert.HasPrivateKey)
                {
                    var resultKeyVaultPss = _keyVaultServiceClient.SignDigestAsync(_signingKey, digest, hashAlgorithm, RSASignaturePadding.Pss).Result;
                    var resultLocalPkcs = _issuerCert.GetRSAPrivateKey().SignData(data, hashAlgorithm, RSASignaturePadding.Pkcs1);
                    var resultLocalPss = _issuerCert.GetRSAPrivateKey().SignData(data, hashAlgorithm, RSASignaturePadding.Pss);
                    for (int i = 0; i < resultKeyVaultPkcs.Length; i++)
                    {
                        if (resultKeyVaultPkcs[i] != resultLocalPkcs[i])
                        {
                            Debug.WriteLine("{0} != {1}", resultKeyVaultPkcs[i], resultLocalPkcs[i]);
                        }
                    }
                    for (int i = 0; i < resultKeyVaultPss.Length; i++)
                    {
                        if (resultKeyVaultPss[i] != resultLocalPss[i])
                        {
                            Debug.WriteLine("{0} != {1}", resultKeyVaultPss[i], resultLocalPss[i]);
                        }
                    }
                }
                return resultKeyVaultPkcs;
            }

            protected override PublicKey BuildPublicKey()
            {
                return _issuerCert.PublicKey;
            }

            internal static PublicKey BuildPublicKey(RSA rsa)
            {
                return null;
            }

            public override byte[] GetSignatureAlgorithmIdentifier(HashAlgorithmName hashAlgorithm)
            {
                const string RsaPkcs1Sha256 = "1.2.840.113549.1.1.11";
                const string RsaPkcs1Sha384 = "1.2.840.113549.1.1.12";
                const string RsaPkcs1Sha512 = "1.2.840.113549.1.1.13";

                string oid = null;

                if (hashAlgorithm == HashAlgorithmName.SHA256)
                {
                    oid = RsaPkcs1Sha256;
                }
                else if (hashAlgorithm == HashAlgorithmName.SHA384)
                {
                    oid = RsaPkcs1Sha384;
                }
                else if (hashAlgorithm == HashAlgorithmName.SHA512)
                {
                    oid = RsaPkcs1Sha512;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(hashAlgorithm));
                }
                //var encoded = new AsnEncodedData(oid, null);
                var result = DerEncoder.ConstructSequence(
                    DerEncoder.SegmentedEncodeOid(oid),
                    DerEncoder.SegmentedEncodeNull()); ;
                return result;
            }
        }
    }

}

