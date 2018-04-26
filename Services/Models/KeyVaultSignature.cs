using Opc.Ua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

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
            RSA publicKey,
            X509SignatureGenerator generator,
            bool isCA = false
            )
        {
            if (publicKey == null || issuerCAKeyCert == null)
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

            var request = new CertificateRequest(subjectDN, publicKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // Basic constraints
            request.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(false, false, 0, true));
            // Subject key identifier
            RSAParameters rsaParams = publicKey.ExportParameters(false);
            var subjectPublicKey = new Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters(
                false,
                new Org.BouncyCastle.Math.BigInteger(1, rsaParams.Modulus),
                new Org.BouncyCastle.Math.BigInteger(1, rsaParams.Exponent));
            var subjectKeyIdentifier = new AsnEncodedData(new Oid(Org.BouncyCastle.Asn1.X509.X509Extensions.SubjectKeyIdentifier.Id),
                new Org.BouncyCastle.Asn1.X509.SubjectKeyIdentifier(
                    Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(subjectPublicKey)).GetDerEncoded());
            request.CertificateExtensions.Add(new X509Extension(subjectKeyIdentifier, false));

            // Authority Key Identifier
            var issuerPublicKey = GetPublicKeyParameter(issuerCAKeyCert);
            var issuerSerialNumber = GetSerialNumber(issuerCAKeyCert);
            var authorityKey = new AsnEncodedData(new Oid(Org.BouncyCastle.Asn1.X509.X509Extensions.AuthorityKeyIdentifier.Id),
                    new Org.BouncyCastle.Asn1.X509.AuthorityKeyIdentifier(
                        Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(issuerPublicKey),
                        new Org.BouncyCastle.Asn1.X509.GeneralNames(
                            new Org.BouncyCastle.Asn1.X509.GeneralName(
                                new CertificateFactoryX509Name(issuerCAKeyCert.Subject))),
                        issuerSerialNumber).GetDerEncoded());
            request.CertificateExtensions.Add(new X509Extension(authorityKey, false));

            // Key Usage
            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.DataEncipherment |
                    X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.KeyEncipherment, true));

            // Enhanced key usage
            request.CertificateExtensions.Add(
                new X509EnhancedKeyUsageExtension(
                    new OidCollection {
                        new Oid("1.3.6.1.5.5.7.3.1"),
                        new Oid("1.3.6.1.5.5.7.3.2") }, true));

            // Subject Alternative Name
            var generalNames = new List<Org.BouncyCastle.Asn1.X509.GeneralName>();
            generalNames.Add(new Org.BouncyCastle.Asn1.X509.GeneralName(
                Org.BouncyCastle.Asn1.X509.GeneralName.UniformResourceIdentifier, applicationUri));
            generalNames.AddRange(CreateSubjectAlternateNameDomains(domainNames));
            var subjectAltName = new AsnEncodedData(new Oid(Org.BouncyCastle.Asn1.X509.X509Extensions.SubjectAlternativeName.Id),
                new Org.BouncyCastle.Asn1.X509.GeneralNames(generalNames.ToArray()).GetDerEncoded());
            request.CertificateExtensions.Add(new X509Extension(subjectAltName, false));

            using (var cfrg = new CertificateFactoryRandomGenerator())
            {
                // cert generators
                var random = new Org.BouncyCastle.Security.SecureRandom(cfrg);

                // Serial Number
                var serialNumber = Org.BouncyCastle.Utilities.BigIntegers.CreateRandomInRange(
                    Org.BouncyCastle.Math.BigInteger.One,
                    Org.BouncyCastle.Math.BigInteger.ValueOf(Int64.MaxValue), random);

                DateTime notAfter = startTime.AddMonths(lifetimeInMonths);
                if (notAfter > issuerCAKeyCert.NotAfter)
                {
                    notAfter = issuerCAKeyCert.NotAfter;
                }

                X509Certificate2 signedCert = request.Create(
                    issuerCAKeyCert.SubjectName,
                    generator,
                    startTime,
                    notAfter,
                    serialNumber.ToByteArray()
                    );

                return Task.FromResult<X509Certificate2>(signedCert);
            }
        }

        /// <summary>
        /// Revoke the certificate. 
        /// The CRL number is increased by one and the new CRL is returned.
        /// </summary>
        public static X509CRL RevokeCertificate(
            X509Certificate2 issuerCertificate,
            List<X509CRL> issuerCrls,
            X509Certificate2Collection revokedCertificates,
            X509SignatureGenerator generator
            )
        {
            var crlSerialNumber = Org.BouncyCastle.Math.BigInteger.Zero;
            Org.BouncyCastle.X509.X509Certificate bcCertCA =
                new Org.BouncyCastle.X509.X509CertificateParser().ReadCertificate(issuerCertificate.RawData);
            Org.BouncyCastle.Crypto.ISignatureFactory signatureFactory =
                    new KeyVaultSignatureFactory(HashAlgorithmName.SHA256, generator);

            var crlGen = new Org.BouncyCastle.X509.X509V2CrlGenerator();
            crlGen.SetIssuerDN(bcCertCA.IssuerDN);

            DateTime now = DateTime.UtcNow;
            DateTime nextUpdate = now.AddMonths(12);
            if (nextUpdate > bcCertCA.NotAfter)
            {
                nextUpdate = bcCertCA.NotAfter;
            }
            crlGen.SetThisUpdate(now);
            crlGen.SetNextUpdate(nextUpdate);

            // merge all existing revocation list
            if (issuerCrls != null)
            {
                var parser = new Org.BouncyCastle.X509.X509CrlParser();
                foreach (X509CRL issuerCrl in issuerCrls)
                {
                    Org.BouncyCastle.X509.X509Crl crl = parser.ReadCrl(issuerCrl.RawData);
                    crlGen.AddCrl(crl);
                    var crlVersion = GetCrlNumber(crl);
                    if (crlVersion.IntValue > crlSerialNumber.IntValue)
                    {
                        crlSerialNumber = crlVersion;
                    }
                }
            }

            if (revokedCertificates == null || revokedCertificates.Count == 0)
            {
                // add a dummy revoked cert
                crlGen.AddCrlEntry(Org.BouncyCastle.Math.BigInteger.One, now, Org.BouncyCastle.Asn1.X509.CrlReason.Unspecified);
            }
            else
            {
                // add the revoked cert
                foreach (var revokedCertificate in revokedCertificates)
                {
                    crlGen.AddCrlEntry(GetSerialNumber(revokedCertificate), now, Org.BouncyCastle.Asn1.X509.CrlReason.PrivilegeWithdrawn);
                }
            }

            crlGen.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.AuthorityKeyIdentifier,
                                false,
                                new Org.BouncyCastle.X509.Extension.AuthorityKeyIdentifierStructure(bcCertCA));

            // set new serial number
            crlSerialNumber = crlSerialNumber.Add(Org.BouncyCastle.Math.BigInteger.One);
            crlGen.AddExtension(Org.BouncyCastle.Asn1.X509.X509Extensions.CrlNumber,
                                false,
                                new Org.BouncyCastle.Asn1.X509.CrlNumber(crlSerialNumber));

            // generate updated CRL
            Org.BouncyCastle.X509.X509Crl updatedCrl = crlGen.Generate(signatureFactory);

            return new X509CRL(updatedCrl.GetEncoded());
        }

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


        /// <summary>
        /// Read the Crl number from a X509Crl.
        /// </summary>
        private static Org.BouncyCastle.Math.BigInteger GetCrlNumber(Org.BouncyCastle.X509.X509Crl crl)
        {
            Org.BouncyCastle.Math.BigInteger crlNumber = Org.BouncyCastle.Math.BigInteger.One;
            try
            {
                Org.BouncyCastle.Asn1.Asn1Object asn1Object = GetExtensionValue(crl, Org.BouncyCastle.Asn1.X509.X509Extensions.CrlNumber);
                if (asn1Object != null)
                {
                    crlNumber = Org.BouncyCastle.Asn1.X509.CrlNumber.GetInstance(asn1Object).PositiveValue;
                }
            }
            finally
            {
            }
            return crlNumber;
        }

        /// <summary>
        /// Get the value of an extension oid.
        /// </summary>
        private static Org.BouncyCastle.Asn1.Asn1Object GetExtensionValue(
            Org.BouncyCastle.X509.IX509Extension extension,
            Org.BouncyCastle.Asn1.DerObjectIdentifier oid)
        {
            Org.BouncyCastle.Asn1.Asn1OctetString asn1Octet = extension.GetExtensionValue(oid);
            if (asn1Octet != null)
            {
                return Org.BouncyCastle.X509.Extension.X509ExtensionUtilities.FromExtensionValue(asn1Octet);
            }
            return null;
        }

        /// <summary>
        /// Get public key parameters from a X509Certificate2
        /// </summary>
        private static Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters GetPublicKeyParameter(X509Certificate2 certificate)
        {
            RSA rsa = null;
            try
            {
                rsa = certificate.GetRSAPublicKey();
                RSAParameters rsaParams = rsa.ExportParameters(false);
                return new Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters(
                    false,
                    new Org.BouncyCastle.Math.BigInteger(1, rsaParams.Modulus),
                    new Org.BouncyCastle.Math.BigInteger(1, rsaParams.Exponent));
            }
            finally
            {
                RsaUtils.RSADispose(rsa);
            }
        }

        /// <summary>
        /// Get the serial number from a certificate as BigInteger.
        /// </summary>
        private static Org.BouncyCastle.Math.BigInteger GetSerialNumber(X509Certificate2 certificate)
        {
            byte[] serialNumber = certificate.GetSerialNumber();
            Array.Reverse(serialNumber);
            return new Org.BouncyCastle.Math.BigInteger(1, serialNumber);
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

        /// <summary>
        /// helper to build alternate name domains list for certs.
        /// </summary>
        private static List<Org.BouncyCastle.Asn1.X509.GeneralName> CreateSubjectAlternateNameDomains(IList<String> domainNames)
        {
            // subject alternate name
            var generalNames = new List<Org.BouncyCastle.Asn1.X509.GeneralName>();
            for (int i = 0; i < domainNames.Count; i++)
            {
                int domainType = Org.BouncyCastle.Asn1.X509.GeneralName.OtherName;
                switch (Uri.CheckHostName(domainNames[i]))
                {
                    case UriHostNameType.Dns: domainType = Org.BouncyCastle.Asn1.X509.GeneralName.DnsName; break;
                    case UriHostNameType.IPv4:
                    case UriHostNameType.IPv6: domainType = Org.BouncyCastle.Asn1.X509.GeneralName.IPAddress; break;
                    default: continue;
                }
                generalNames.Add(new Org.BouncyCastle.Asn1.X509.GeneralName(domainType, domainNames[i]));
            }
            return generalNames;
        }

        public class KeyVaultSignatureGenerator : X509SignatureGenerator
        {
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
            }

            public override byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm)
            {
                HashAlgorithm hash;
                if (hashAlgorithm == HashAlgorithmName.SHA256)
                {
                    hash = SHA256.Create();
                }
                else if (hashAlgorithm == HashAlgorithmName.SHA384)
                {
                    hash = SHA384.Create();
                }
                else if (hashAlgorithm == HashAlgorithmName.SHA512)
                {
                    hash = SHA512.Create();
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(hashAlgorithm));
                }
                var digest = hash.ComputeHash(data);
                var resultKeyVaultPkcs = _keyVaultServiceClient.SignDigestAsync(_signingKey, digest, hashAlgorithm, RSASignaturePadding.Pkcs1).Result;
#if TESTANDVERIFYTHEKEYVAULTSIGNER
                // for testing only
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
#endif
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
                byte[] oidSequence;

                if (hashAlgorithm == HashAlgorithmName.SHA256)
                {
                    //const string RsaPkcs1Sha256 = "1.2.840.113549.1.1.11";
                    oidSequence = new byte[] { 48, 13, 6, 9, 42, 134, 72, 134, 247, 13, 1, 1, 11, 5, 0 };
                }
                else if (hashAlgorithm == HashAlgorithmName.SHA384)
                {
                    //const string RsaPkcs1Sha384 = "1.2.840.113549.1.1.12";
                    oidSequence = new byte[] { 48, 13, 6, 9, 42, 134, 72, 134, 247, 13, 1, 1, 12, 5, 0 };
                }
                else if (hashAlgorithm == HashAlgorithmName.SHA512)
                {
                    //const string RsaPkcs1Sha512 = "1.2.840.113549.1.1.13";
                    oidSequence = new byte[] { 48, 13, 6, 9, 42, 134, 72, 134, 247, 13, 1, 1, 13, 5, 0 };
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(hashAlgorithm));
                }
                return oidSequence;
            }
        }
    }

    /// <summary>
    /// Calculator factory class for signature generation in ASN.1 based profiles that use an AlgorithmIdentifier to preserve
    /// signature algorithm details.
    /// </summary>
    public class KeyVaultSignatureFactory : Org.BouncyCastle.Crypto.ISignatureFactory
    {
        private readonly Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier algID;
        private readonly HashAlgorithmName hashAlgorithm;
        private X509SignatureGenerator generator;

        /// <summary>
        /// Constructor which also specifies a source of randomness to be used if one is required.
        /// </summary>
        /// <param name="algorithm">The name of the signature algorithm to use.</param>
        /// <param name="privateKey">The private key to be used in the signing operation.</param>
        /// <param name="random">The source of randomness to be used in signature calculation.</param>
        public KeyVaultSignatureFactory(HashAlgorithmName hashAlgorithm, X509SignatureGenerator generator)
        {
            Org.BouncyCastle.Asn1.DerObjectIdentifier sigOid;
            if (hashAlgorithm == HashAlgorithmName.SHA256)
            {
                sigOid = Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Sha256WithRsaEncryption;
            }
            else if (hashAlgorithm == HashAlgorithmName.SHA384)
            {
                sigOid = Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Sha384WithRsaEncryption;
            }
            else if (hashAlgorithm == HashAlgorithmName.SHA512)
            {
                sigOid = Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Sha512WithRsaEncryption;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(hashAlgorithm));
            }
            this.hashAlgorithm = hashAlgorithm;
            this.generator = generator;
            this.algID = new Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier(sigOid);
        }

        public Object AlgorithmDetails
        {
            get { return this.algID; }
        }

        public Org.BouncyCastle.Crypto.IStreamCalculator CreateCalculator()
        {
            return new KeyVaultStreamCalculator(generator, hashAlgorithm);
        }
    }

    public class KeyVaultStreamCalculator : Org.BouncyCastle.Crypto.IStreamCalculator
    {
        private X509SignatureGenerator generator;
        private HashAlgorithmName hashAlgorithm;

        public KeyVaultStreamCalculator(
            X509SignatureGenerator generator,
            HashAlgorithmName hashAlgorithm)
        {
            Stream = new MemoryStream();
            this.generator = generator;
            this.hashAlgorithm = hashAlgorithm;
        }

        public Stream Stream { get; }

        public object GetResult()
        {
            var memStream = Stream as MemoryStream;
            var digest = memStream.ToArray();
            var signature = generator.SignData(digest, hashAlgorithm);
            return new MemoryBlockResult(signature);
        }
    }

    public class MemoryBlockResult : Org.BouncyCastle.Crypto.IBlockResult
    {
        private byte[] data;
        public MemoryBlockResult(byte[] data)
        {
            this.data = data;
        }
        public byte[] Collect()
        {
            return data;
        }
        public int Collect(byte[] destination, int offset)
        {
            throw new NotImplementedException();
        }
    }
}
