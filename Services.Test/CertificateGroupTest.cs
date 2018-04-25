// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.OpcGdsVault.Common.Diagnostics;
using Microsoft.Azure.IoTSolutions.OpcGdsVault.Services.Runtime;
using Opc.Ua;
using Opc.Ua.Gds;
using Opc.Ua.Test;
using Services.Test.helpers;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.Services.Test
{
    public class ApplicationTestData
    {
        public ApplicationTestData()
        {
            Initialize();
        }

        private void Initialize()
        {
            ApplicationRecord = new ApplicationRecordDataType();
            CertificateGroupId = null;
            CertificateTypeId = null;
            CertificateRequestId = null;
            DomainNames = new StringCollection();
            Subject = null;
            PrivateKeyFormat = "PFX";
            PrivateKeyPassword = "";
            Certificate = null;
            PrivateKey = null;
            IssuerCertificates = null;
        }

        public ApplicationRecordDataType ApplicationRecord;
        public NodeId CertificateGroupId;
        public NodeId CertificateTypeId;
        public NodeId CertificateRequestId;
        public StringCollection DomainNames;
        public String Subject;
        public String PrivateKeyFormat;
        public String PrivateKeyPassword;
        public byte[] Certificate;
        public byte[] PrivateKey;
        public byte[][] IssuerCertificates;
    }

    public class CertificateGroupTest
    {
        ServicesConfig config = new ServicesConfig()
        {
            KeyVaultApiUrl = "https://iopgdshsm.vault.azure.net"
        };
        Logger logger = new Logger("Services.Test", LogLevel.Debug);

        public CertificateGroupTest(ITestOutputHelper log)
        {
            this._log = log;
            _randomSource = new RandomSource(randomStart);
            _dataGenerator = new DataGenerator(_randomSource);
        }


        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public async Task KeyVaultInit()
        {
            var keyVault = new CertificateGroup(config, logger);
            await keyVault.Init();
        }

        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public async Task KeyVaultListOfCertGroups()
        {
            var keyVault = new CertificateGroup(config, logger);
            var groups = await keyVault.GetCertificateGroupIds();
        }

        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public async Task KeyVaultGroupConfigurationCollection()
        {

            var keyVault = new CertificateGroup(config, logger);
            var groupCollection = await keyVault.GetCertificateGroupConfigurationCollection();
            Assert.NotNull(groupCollection);
            Assert.NotEmpty(groupCollection);
        }

        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public async Task KeyVaultGetCertificateAsync()
        {
            var keyVault = new CertificateGroup(config, logger);
            await keyVault.Init();
            var groups = await keyVault.GetCertificateGroupIds();
            foreach (var group in groups)
            {
                var caChain = await keyVault.GetCACertificateChainAsync(group);
                Assert.NotNull(caChain);
                Assert.True(caChain.Count >= 1);
                foreach (var caCert in caChain)
                {
                    Assert.False(caCert.HasPrivateKey);
                }
                var crlChain = await keyVault.GetCACrlChainAsync(group);
                Assert.NotNull(crlChain);
                Assert.True(crlChain.Count >= 1);
                for (int i = 0; i < caChain.Count; i++)
                {
                    crlChain[i].VerifySignature(caChain[i], true);
                    Assert.True(Utils.CompareDistinguishedName(crlChain[i].Issuer, caChain[i].Issuer));
                }
            }
        }

        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public async Task KeyVaultCreateCACertificateAsync()
        {
            var keyVault = new CertificateGroup(config, logger);
            var groups = await keyVault.GetCertificateGroupIds();
            foreach (var group in groups)
            {
                var result = await keyVault.CreateCACertificateAsync(group);
                Assert.NotNull(result);
                Assert.False(result.HasPrivateKey);
                Assert.True(Utils.CompareDistinguishedName(result.Issuer, result.Subject));
                var basicConstraints = FindBasicConstraintsExtension(result);
                Assert.NotNull(basicConstraints);
                Assert.True(basicConstraints.CertificateAuthority);
            }
        }

        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public async Task KeyVaultCreateNewKeyPairRequestAsync()
        {
            var keyVault = new CertificateGroup(config, logger);
            var groups = await keyVault.GetCertificateGroupIds();
            foreach (var group in groups)
            {
                var randomApp = RandomApplicationTestData();
                var newKeyPair = await keyVault.NewKeyPairRequestAsync(
                    group,
                    randomApp.ApplicationRecord.ApplicationUri,
                    randomApp.Subject,
                    randomApp.DomainNames.ToArray(),
                    randomApp.PrivateKeyFormat,
                    randomApp.PrivateKeyPassword);
                Assert.NotNull(newKeyPair);
                Assert.False(newKeyPair.Certificate.HasPrivateKey);
                Assert.True(Utils.CompareDistinguishedName(randomApp.Subject, newKeyPair.Certificate.Subject));
                Assert.False(Utils.CompareDistinguishedName(newKeyPair.Certificate.Issuer, newKeyPair.Certificate.Subject));

                X509Certificate2 newPrivateKeyCert = null;
                if (randomApp.PrivateKeyFormat == "PFX")
                {
                    newPrivateKeyCert = CertificateFactory.CreateCertificateFromPKCS12(newKeyPair.PrivateKey, randomApp.PrivateKeyPassword);
                }
                else if (randomApp.PrivateKeyFormat == "PEM")
                {
                    newPrivateKeyCert = CertificateFactory.CreateCertificateWithPEMPrivateKey(newKeyPair.Certificate, newKeyPair.PrivateKey, randomApp.PrivateKeyPassword);
                }
                else
                {
                    Assert.True(false, "Invalid private key format");
                }

                Assert.True(CertificateFactory.VerifyRSAKeyPair(newKeyPair.Certificate, newPrivateKeyCert, true));
                Assert.True(CertificateFactory.VerifyRSAKeyPair(newPrivateKeyCert, newPrivateKeyCert, true));


            }
        }

        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public async Task KeyVaultSigningRequestAsync()
        {
            var keyVault = new CertificateGroup(config, logger);
            var groups = await keyVault.GetCertificateGroupIds();
            foreach (var group in groups)
            {
                var certificateGroupConfiguration = await keyVault.GetCertificateGroupConfiguration(group);
                var randomApp = RandomApplicationTestData();
                var csrCertificate = CertificateFactory.CreateCertificate(
                    null, null, null,
                    randomApp.ApplicationRecord.ApplicationUri,
                    null,
                    randomApp.Subject,
                    randomApp.DomainNames.ToArray(),
                    certificateGroupConfiguration.DefaultCertificateKeySize,
                    DateTime.UtcNow.AddDays(-10),
                    certificateGroupConfiguration.DefaultCertificateLifetime,
                    certificateGroupConfiguration.DefaultCertificateHashSize
                    );
                byte[] certificateRequest = CertificateFactory.CreateSigningRequest(csrCertificate, randomApp.DomainNames);

                var newCert = await keyVault.SigningRequestAsync(
                    group,
                    randomApp.ApplicationRecord.ApplicationUri,
                    certificateRequest);

                Assert.NotNull(newCert);
                Assert.False(newCert.HasPrivateKey);
                Assert.True(Utils.CompareDistinguishedName(randomApp.Subject, newCert.Subject));
                Assert.False(Utils.CompareDistinguishedName(newCert.Issuer, newCert.Subject));
                Assert.True(Utils.CompareDistinguishedName(newCert.Issuer, certificateGroupConfiguration.SubjectName));
#if WRITECERT
                // save cert for debugging
                using (ICertificateStore store = CertificateStoreIdentifier.CreateStore(CertificateStoreType.Directory))
                {
                    Assert.NotNull(store);
                    store.Open("d:\\unittest");
                    await store.Add(newCert);
                }
#endif
                // test basic constraints
                var constraints = FindBasicConstraintsExtension(newCert);
                Assert.NotNull(constraints);
                Assert.True(constraints.Critical);
                Assert.False(constraints.CertificateAuthority);
                Assert.False(constraints.HasPathLengthConstraint);

                // key usage
                var keyUsage = FindKeyUsageExtension(newCert);
                Assert.NotNull(keyUsage);
                Assert.True(keyUsage.Critical);
                Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.CrlSign) == 0);
                Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.DataEncipherment) == X509KeyUsageFlags.DataEncipherment);
                Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.DecipherOnly) == 0);
                Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.DigitalSignature) == X509KeyUsageFlags.DigitalSignature);
                Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.EncipherOnly) == 0);
                Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.KeyAgreement) == 0);
                Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.KeyCertSign) == 0);
                Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.KeyEncipherment) == X509KeyUsageFlags.KeyEncipherment);
                Assert.True((keyUsage.KeyUsages & X509KeyUsageFlags.NonRepudiation) == X509KeyUsageFlags.NonRepudiation);

                // enhanced key usage
                var enhancedKeyUsage = FindEnhancedKeyUsageExtension(newCert);
                Assert.NotNull(enhancedKeyUsage);
                Assert.True(enhancedKeyUsage.Critical);

                // test for authority key
                X509AuthorityKeyIdentifierExtension authority = FindAuthorityKeyIdentifier(newCert);
                Assert.NotNull(authority);
                Assert.NotNull(authority.SerialNumber);
                Assert.NotNull(authority.KeyId);
                Assert.NotNull(authority.AuthorityNames);

                // get issuer cert used for signing
                var issuerCert = await keyVault.GetCACertificateChainAsync(group);
                Assert.NotNull(issuerCert);
                Assert.True(issuerCert.Count >= 1);

                // verify authority key in signed cert
                X509SubjectKeyIdentifierExtension subjectKeyId = FindSubjectKeyIdentifierExtension(issuerCert[0]);
                Assert.Equal(subjectKeyId.SubjectKeyIdentifier, authority.KeyId);
                Assert.Equal(issuerCert[0].SerialNumber, authority.SerialNumber);

                X509SubjectAltNameExtension subjectAlternateName = FindSubjectAltName(newCert);
                Assert.NotNull(subjectAlternateName);
                Assert.False(subjectAlternateName.Critical);
                var domainNames = Utils.GetDomainsFromCertficate(newCert);
                foreach (var domainName in randomApp.DomainNames)
                {
                    Assert.True(domainNames.Contains(domainName, StringComparer.OrdinalIgnoreCase));
                }
                Assert.True(subjectAlternateName.Uris.Count == 1);
                var applicationUri = Utils.GetApplicationUriFromCertificate(newCert);
                Assert.True(randomApp.ApplicationRecord.ApplicationUri == applicationUri);
            }
        }


        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public async Task KeyVaultCreateNewKeyPairAndRevokeCertificateAsync()
        {
            var keyVault = new CertificateGroup(config, logger);
            await keyVault.Init();
            var groups = await keyVault.GetCertificateGroupIds();
            foreach (var group in groups)
            {
                var randomApp = RandomApplicationTestData();
                var newCert = await keyVault.NewKeyPairRequestAsync(
                    group,
                    randomApp.ApplicationRecord.ApplicationUri,
                    randomApp.Subject,
                    randomApp.DomainNames.ToArray(),
                    randomApp.PrivateKeyFormat,
                    randomApp.PrivateKeyPassword
                    );
                Assert.NotNull(newCert);
                Assert.False(newCert.Certificate.HasPrivateKey);
                Assert.True(Utils.CompareDistinguishedName(randomApp.Subject, newCert.Certificate.Subject));
                Assert.False(Utils.CompareDistinguishedName(newCert.Certificate.Issuer, newCert.Certificate.Subject));
                var cert = new X509Certificate2(newCert.Certificate.RawData);
                var crl = await keyVault.RevokeCertificateAsync(group, cert);
                Assert.NotNull(crl);
                var caChain = await keyVault.GetCACertificateChainAsync(group);
                Assert.NotNull(caChain);
                var caCert = caChain[0];
                Assert.False(caCert.HasPrivateKey);
                crl.VerifySignature(caCert, true);
                Assert.True(Utils.CompareDistinguishedName(crl.Issuer, caCert.Issuer));
            }
        }

        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public async Task GetTrustListAsync()
        {
            var keyVault = new CertificateGroup(config, logger);
            await keyVault.Init();
            var groups = await keyVault.GetCertificateGroupIds();
            foreach (var group in groups)
            {
                await keyVault.GetTrustListAsync(group);
            }
        }


        private ApplicationTestData RandomApplicationTestData()
        {
            ApplicationType appType = (ApplicationType)_randomSource.NextInt32((int)ApplicationType.ClientAndServer);
            string pureAppName = _dataGenerator.GetRandomString("en");
            pureAppName = Regex.Replace(pureAppName, @"[^\w\d\s]", "");
            string pureAppUri = Regex.Replace(pureAppName, @"[^\w\d]", "");
            string appName = "UA " + pureAppName;
            StringCollection domainNames = RandomDomainNames();
            string localhost = domainNames[0];
            string privateKeyFormat = _randomSource.NextInt32(1) == 0 ? "PEM" : "PFX";
            string appUri = ("urn:localhost:opcfoundation.org:" + pureAppUri.ToLower()).Replace("localhost", localhost);
            string prodUri = "http://opcfoundation.org/UA/" + pureAppUri;
            StringCollection discoveryUrls = new StringCollection();
            StringCollection serverCapabilities = new StringCollection();
            switch (appType)
            {
                case ApplicationType.Client:
                    appName += " Client";
                    break;
                case ApplicationType.ClientAndServer:
                    appName += " Client and";
                    goto case ApplicationType.Server;
                case ApplicationType.Server:
                    appName += " Server";
                    int port = (_dataGenerator.GetRandomInt16() & 0x1fff) + 50000;
                    discoveryUrls = RandomDiscoveryUrl(domainNames, port, pureAppUri);
                    break;
            }
            ApplicationTestData testData = new ApplicationTestData
            {
                ApplicationRecord = new ApplicationRecordDataType
                {
                    ApplicationNames = new LocalizedTextCollection { new LocalizedText("en-us", appName) },
                    ApplicationUri = appUri,
                    ApplicationType = appType,
                    ProductUri = prodUri,
                    DiscoveryUrls = discoveryUrls,
                    ServerCapabilities = serverCapabilities
                },
                DomainNames = domainNames,
                Subject = String.Format("CN={0},DC={1},O=OPC Foundation", appName, localhost),
                PrivateKeyFormat = privateKeyFormat
            };
            return testData;
        }

        private string RandomLocalHost()
        {
            string localhost = Regex.Replace(_dataGenerator.GetRandomSymbol("en").Trim().ToLower(), @"[^\w\d]", "");
            if (localhost.Length >= 12)
            {
                localhost = localhost.Substring(0, 12);
            }
            return localhost;
        }

        private string[] RandomDomainNames()
        {
            int count = _randomSource.NextInt32(8) + 1;
            var result = new string[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = RandomLocalHost();
            }
            return result;
        }

        private StringCollection RandomDiscoveryUrl(StringCollection domainNames, int port, string appUri)
        {
            var result = new StringCollection();
            foreach (var name in domainNames)
            {
                int random = _randomSource.NextInt32(7);
                if ((result.Count == 0) || (random & 1) == 0)
                {
                    result.Add(String.Format("opc.tcp://{0}:{1}/{2}", name, (port++).ToString(), appUri));
                }
                if ((random & 2) == 0)
                {
                    result.Add(String.Format("http://{0}:{1}/{2}", name, (port++).ToString(), appUri));
                }
                if ((random & 4) == 0)
                {
                    result.Add(String.Format("https://{0}:{1}/{2}", name, (port++).ToString(), appUri));
                }
            }
            return result;
        }

        private X509BasicConstraintsExtension FindBasicConstraintsExtension(X509Certificate2 certificate)
        {
            for (int ii = 0; ii < certificate.Extensions.Count; ii++)
            {
                X509BasicConstraintsExtension extension = certificate.Extensions[ii] as X509BasicConstraintsExtension;
                if (extension != null)
                {
                    return extension;
                }
            }
            return null;
        }

        private X509KeyUsageExtension FindKeyUsageExtension(X509Certificate2 certificate)
        {
            for (int ii = 0; ii < certificate.Extensions.Count; ii++)
            {
                X509KeyUsageExtension extension = certificate.Extensions[ii] as X509KeyUsageExtension;
                if (extension != null)
                {
                    return extension;
                }
            }
            return null;
        }
        private X509EnhancedKeyUsageExtension FindEnhancedKeyUsageExtension(X509Certificate2 certificate)
        {
            for (int ii = 0; ii < certificate.Extensions.Count; ii++)
            {
                X509EnhancedKeyUsageExtension extension = certificate.Extensions[ii] as X509EnhancedKeyUsageExtension;
                if (extension != null)
                {
                    return extension;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the authority key identifier in the certificate.
        /// </summary>
        private X509AuthorityKeyIdentifierExtension FindAuthorityKeyIdentifier(X509Certificate2 certificate)
        {
            for (int ii = 0; ii < certificate.Extensions.Count; ii++)
            {
                X509Extension extension = certificate.Extensions[ii];

                switch (extension.Oid.Value)
                {
                    case X509AuthorityKeyIdentifierExtension.AuthorityKeyIdentifierOid:
                    case X509AuthorityKeyIdentifierExtension.AuthorityKeyIdentifier2Oid:
                        {
                            return new X509AuthorityKeyIdentifierExtension(extension, extension.Critical);
                        }
                }
            }

            return null;
        }

        private X509SubjectAltNameExtension FindSubjectAltName(X509Certificate2 certificate)
        {
            foreach (var extension in certificate.Extensions)
            {
                if (extension.Oid.Value == X509SubjectAltNameExtension.SubjectAltNameOid ||
                    extension.Oid.Value == X509SubjectAltNameExtension.SubjectAltName2Oid)
                {
                    return new X509SubjectAltNameExtension(extension, extension.Critical);
                }
            }
            return null;
        }


        ///
        /// Returns the subject key identifier in the certificate.
        /// </summary>
        private X509SubjectKeyIdentifierExtension FindSubjectKeyIdentifierExtension(X509Certificate2 certificate)
        {
            for (int ii = 0; ii < certificate.Extensions.Count; ii++)
            {
                X509SubjectKeyIdentifierExtension extension = certificate.Extensions[ii] as X509SubjectKeyIdentifierExtension;

                if (extension != null)
                {
                    return extension;
                }
            }

            return null;
        }

        /// <summary>The test logger</summary>
        private readonly ITestOutputHelper _log;
        private const int randomStart = 1;
        private RandomSource _randomSource;
        private DataGenerator _dataGenerator;

    }
}
