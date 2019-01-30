// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Azure.IIoT.Auth.Clients;
using Microsoft.Azure.IIoT.Diagnostics;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.Models;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.Runtime;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.Test.Helpers;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Opc.Ua;
using TestCaseOrdering;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Vault.Test
{

    [TestCaseOrderer("TestCaseOrdering.PriorityOrderer", "Microsoft.Azure.IIoT.OpcUa.Services.Vault.Test")]
    public class CertificateGroupTest
    {
        IConfigurationRoot _configuration;

        ServicesConfig _serviceConfig = new ServicesConfig();
        IClientConfig _clientConfig = new ClientConfig();
        TraceLogger _logger = new TraceLogger(new LogConfig());

        public CertificateGroupTest(ITestOutputHelper log)
        {
            _log = log;
            _randomGenerator = new ApplicationTestDataGenerator();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("testsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            _configuration = builder.Build();
            _configuration.Bind("OpcVault", _serviceConfig);
            _configuration.Bind("Auth", _clientConfig);
            SkipOnInvalidConfiguration();
        }

        [SkippableFact, Trait(Constants.Type, Constants.UnitTest), TestPriority(100)]
        private async Task KeyVaultPurgeCACertificateAsync()
        {
            KeyVaultCertificateGroup keyVault = new KeyVaultCertificateGroup(_serviceConfig, _clientConfig, _logger);
            await keyVault.PurgeAsync();
        }


        [SkippableFact, Trait(Constants.Type, Constants.UnitTest), TestPriority(200)]
        public async Task KeyVaultCreateCACertificateAsync()
        {
            KeyVaultCertificateGroup keyVault = new KeyVaultCertificateGroup(_serviceConfig, _clientConfig, _logger);
            string[] groups = await keyVault.GetCertificateGroupIds();
            foreach (string group in groups)
            {
                X509Certificate2 result = await keyVault.CreateIssuerCACertificateAsync(group);
                Assert.NotNull(result);
                Assert.False(result.HasPrivateKey);
                Assert.True(Opc.Ua.Utils.CompareDistinguishedName(result.Issuer, result.Subject));
                X509BasicConstraintsExtension basicConstraints = X509TestUtils.FindBasicConstraintsExtension(result);
                Assert.NotNull(basicConstraints);
                Assert.True(basicConstraints.CertificateAuthority);
                Assert.True(basicConstraints.Critical);
                var subjectKeyId = result.Extensions.OfType<X509SubjectKeyIdentifierExtension>().Single();
                Assert.False(subjectKeyId.Critical);
                var authorityKeyIdentifier = X509TestUtils.FindAuthorityKeyIdentifier(result);
                Assert.NotNull(authorityKeyIdentifier);
                Assert.False(authorityKeyIdentifier.Critical);
                Assert.Equal(authorityKeyIdentifier.SerialNumber, result.SerialNumber, ignoreCase: true);
                Assert.Equal(authorityKeyIdentifier.KeyId, subjectKeyId.SubjectKeyIdentifier, ignoreCase: true);
            }
        }


        [SkippableFact, Trait(Constants.Type, Constants.UnitTest), TestPriority(300)]
        public async Task KeyVaultInit()
        {
            SkipOnInvalidConfiguration();
            KeyVaultCertificateGroup keyVault = new KeyVaultCertificateGroup(_serviceConfig, _clientConfig, _logger);
            await keyVault.Init();
        }

        [SkippableFact, Trait(Constants.Type, Constants.UnitTest), TestPriority(400)]
        public async Task KeyVaultListOfCertGroups()
        {
            SkipOnInvalidConfiguration();
            KeyVaultCertificateGroup keyVault = new KeyVaultCertificateGroup(_serviceConfig, _clientConfig, _logger);
            string[] groups = await keyVault.GetCertificateGroupIds();
        }

        [SkippableFact, Trait(Constants.Type, Constants.UnitTest), TestPriority(400)]
        public async Task KeyVaultGroupConfigurationCollection()
        {
            SkipOnInvalidConfiguration();
            KeyVaultCertificateGroup keyVault = new KeyVaultCertificateGroup(_serviceConfig, _clientConfig, _logger);
            var groupCollection = await keyVault.GetCertificateGroupConfigurationCollection();
            Assert.NotNull(groupCollection);
            Assert.NotEmpty(groupCollection);
        }

        [SkippableFact, Trait(Constants.Type, Constants.UnitTest), TestPriority(400)]
        public async Task KeyVaultGetCertificateAsync()
        {
            SkipOnInvalidConfiguration();
            KeyVaultCertificateGroup keyVault = new KeyVaultCertificateGroup(_serviceConfig, _clientConfig, _logger);
            await keyVault.Init();
            string[] groups = await keyVault.GetCertificateGroupIds();
            foreach (string group in groups)
            {
                X509Certificate2Collection caChain = await keyVault.GetIssuerCACertificateChainAsync(group);
                Assert.NotNull(caChain);
                Assert.True(caChain.Count >= 1);
                foreach (X509Certificate2 caCert in caChain)
                {
                    Assert.False(caCert.HasPrivateKey);
                }
                System.Collections.Generic.IList<X509CRL> crlChain = await keyVault.GetIssuerCACrlChainAsync(group);
                Assert.NotNull(crlChain);
                Assert.True(crlChain.Count >= 1);
                for (int i = 0; i < caChain.Count; i++)
                {
                    crlChain[i].VerifySignature(caChain[i], true);
                    Assert.True(Opc.Ua.Utils.CompareDistinguishedName(crlChain[i].Issuer, caChain[i].Issuer));
                }
            }
        }

        [SkippableFact, Trait(Constants.Type, Constants.UnitTest), TestPriority(500)]
        public async Task<X509CertificateCollection> KeyVaultNewKeyPairRequestAsync()
        {
            SkipOnInvalidConfiguration();
            X509CertificateCollection certCollection = new X509CertificateCollection();
            KeyVaultCertificateGroup keyVault = new KeyVaultCertificateGroup(_serviceConfig, _clientConfig, _logger);
            string[] groups = await keyVault.GetCertificateGroupIds();
            foreach (string group in groups)
            {
                ApplicationTestData randomApp = _randomGenerator.RandomApplicationTestData();
                Guid requestId = Guid.NewGuid();
                Opc.Ua.Gds.Server.X509Certificate2KeyPair newKeyPair = await keyVault.NewKeyPairRequestAsync(
                    group,
                    requestId.ToString(),
                    randomApp.ApplicationRecord.ApplicationUri,
                    randomApp.Subject,
                    randomApp.DomainNames.ToArray(),
                    randomApp.PrivateKeyFormat,
                    randomApp.PrivateKeyPassword);
                Assert.NotNull(newKeyPair);
                Assert.False(newKeyPair.Certificate.HasPrivateKey);
                Assert.True(Opc.Ua.Utils.CompareDistinguishedName(randomApp.Subject, newKeyPair.Certificate.Subject));
                Assert.False(Opc.Ua.Utils.CompareDistinguishedName(newKeyPair.Certificate.Issuer, newKeyPair.Certificate.Subject));
                X509Certificate2Collection issuerCerts = await keyVault.GetIssuerCACertificateChainAsync(group);
                Assert.NotNull(issuerCerts);
                Assert.True(issuerCerts.Count >= 1);

                X509TestUtils.VerifyApplicationCertIntegrity(
                    newKeyPair.Certificate,
                    newKeyPair.PrivateKey,
                    randomApp.PrivateKeyPassword,
                    randomApp.PrivateKeyFormat,
                    issuerCerts
                    );
                certCollection.Add(newKeyPair.Certificate);

                // disable and delete private key from KeyVault (requires set/delete rights)
                await keyVault.AcceptPrivateKeyAsync(group, requestId.ToString());
                await keyVault.DeletePrivateKeyAsync(group, requestId.ToString());
            }
            return certCollection;
        }

        [SkippableFact, Trait(Constants.Type, Constants.UnitTest), TestPriority(500)]
        public async Task<X509CertificateCollection> KeyVaultSigningRequestAsync()
        {
            SkipOnInvalidConfiguration();
            X509CertificateCollection certCollection = new X509CertificateCollection();
            KeyVaultCertificateGroup keyVault = new KeyVaultCertificateGroup(_serviceConfig, _clientConfig, _logger);
            string[] groups = await keyVault.GetCertificateGroupIds();
            foreach (string group in groups)
            {
                var certificateGroupConfiguration = await keyVault.GetCertificateGroupConfiguration(group);
                ApplicationTestData randomApp = _randomGenerator.RandomApplicationTestData();
                X509Certificate2 csrCertificate = CertificateFactory.CreateCertificate(
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

                X509Certificate2 newCert = await keyVault.SigningRequestAsync(
                    group,
                    randomApp.ApplicationRecord.ApplicationUri,
                    certificateRequest);
                // get issuer cert used for signing
                X509Certificate2Collection issuerCerts = await keyVault.GetIssuerCACertificateChainAsync(group);
#if WRITECERT
                // save cert for debugging
                using (ICertificateStore store = CertificateStoreIdentifier.CreateStore(CertificateStoreType.Directory))
                {
                    Assert.NotNull(store);
                    store.Open("d:\\unittest");
                    await store.Add(newCert);
                    foreach (var cert in issuerCerts) await store.Add(cert);
                }
#endif
                Assert.NotNull(issuerCerts);
                Assert.True(issuerCerts.Count >= 1);
                X509TestUtils.VerifySignedApplicationCert(randomApp, newCert, issuerCerts);
                certCollection.Add(newCert);
            }
            return certCollection;
        }


        [SkippableFact, Trait(Constants.Type, Constants.UnitTest), TestPriority(600)]
        public async Task KeyVaultNewKeyPairAndRevokeCertificateAsync()
        {
            SkipOnInvalidConfiguration();
            KeyVaultCertificateGroup keyVault = new KeyVaultCertificateGroup(_serviceConfig, _clientConfig, _logger);
            await keyVault.Init();
            string[] groups = await keyVault.GetCertificateGroupIds();
            foreach (string group in groups)
            {
                ApplicationTestData randomApp = _randomGenerator.RandomApplicationTestData();
                Guid requestId = Guid.NewGuid();
                Opc.Ua.Gds.Server.X509Certificate2KeyPair newCert = await keyVault.NewKeyPairRequestAsync(
                    group,
                    requestId.ToString(),
                    randomApp.ApplicationRecord.ApplicationUri,
                    randomApp.Subject,
                    randomApp.DomainNames.ToArray(),
                    randomApp.PrivateKeyFormat,
                    randomApp.PrivateKeyPassword
                    );
                Assert.NotNull(newCert);
                Assert.False(newCert.Certificate.HasPrivateKey);
                Assert.True(Opc.Ua.Utils.CompareDistinguishedName(randomApp.Subject, newCert.Certificate.Subject));
                Assert.False(Opc.Ua.Utils.CompareDistinguishedName(newCert.Certificate.Issuer, newCert.Certificate.Subject));
                X509Certificate2 cert = new X509Certificate2(newCert.Certificate.RawData);
                X509CRL crl = await keyVault.RevokeCertificateAsync(group, cert);
                Assert.NotNull(crl);
                X509Certificate2Collection caChain = await keyVault.GetIssuerCACertificateChainAsync(group);
                Assert.NotNull(caChain);
                X509Certificate2 caCert = caChain[0];
                Assert.False(caCert.HasPrivateKey);
                crl.VerifySignature(caCert, true);
                Assert.True(Opc.Ua.Utils.CompareDistinguishedName(crl.Issuer, caCert.Issuer));
                // disable and delete private key from KeyVault (requires set/delete rights)
                await keyVault.AcceptPrivateKeyAsync(group, requestId.ToString());
                await keyVault.DeletePrivateKeyAsync(group, requestId.ToString());
            }
        }

        [SkippableFact, Trait(Constants.Type, Constants.UnitTest), TestPriority(600)]
        public async Task KeyVaultNewKeyPairLoadThenDeletePrivateKeyAsync()
        {
            SkipOnInvalidConfiguration();
            KeyVaultCertificateGroup keyVault = new KeyVaultCertificateGroup(_serviceConfig, _clientConfig, _logger);
            await keyVault.Init();
            string[] groups = await keyVault.GetCertificateGroupIds();
            foreach (string group in groups)
            {
                ApplicationTestData randomApp = _randomGenerator.RandomApplicationTestData();
                Guid requestId = Guid.NewGuid();
                Opc.Ua.Gds.Server.X509Certificate2KeyPair newKeyPair = await keyVault.NewKeyPairRequestAsync(
                    group,
                    requestId.ToString(),
                    randomApp.ApplicationRecord.ApplicationUri,
                    randomApp.Subject,
                    randomApp.DomainNames.ToArray(),
                    randomApp.PrivateKeyFormat,
                    randomApp.PrivateKeyPassword
                    );
                Assert.NotNull(newKeyPair);
                Assert.False(newKeyPair.Certificate.HasPrivateKey);
                Assert.True(Opc.Ua.Utils.CompareDistinguishedName(randomApp.Subject, newKeyPair.Certificate.Subject));
                Assert.False(Opc.Ua.Utils.CompareDistinguishedName(newKeyPair.Certificate.Issuer, newKeyPair.Certificate.Subject));

                X509Certificate2Collection issuerCerts = await keyVault.GetIssuerCACertificateChainAsync(group);
                Assert.NotNull(issuerCerts);
                Assert.True(issuerCerts.Count >= 1);

                X509TestUtils.VerifyApplicationCertIntegrity(
                    newKeyPair.Certificate,
                    newKeyPair.PrivateKey,
                    randomApp.PrivateKeyPassword,
                    randomApp.PrivateKeyFormat,
                    issuerCerts
                    );

                // test to load the key from KeyVault
                var privateKey = await keyVault.LoadPrivateKeyAsync(group, requestId.ToString(), randomApp.PrivateKeyFormat);
                X509Certificate2 privateKeyX509;
                if (randomApp.PrivateKeyFormat == "PFX")
                {
                    privateKeyX509 = CertificateFactory.CreateCertificateFromPKCS12(privateKey, randomApp.PrivateKeyPassword);
                }
                else
                {
                    privateKeyX509 = CertificateFactory.CreateCertificateWithPEMPrivateKey(newKeyPair.Certificate, privateKey, randomApp.PrivateKeyPassword);
                }
                Assert.True(privateKeyX509.HasPrivateKey);

                X509TestUtils.VerifyApplicationCertIntegrity(
                    newKeyPair.Certificate,
                    privateKey,
                    randomApp.PrivateKeyPassword,
                    randomApp.PrivateKeyFormat,
                    issuerCerts
                    );

                await keyVault.AcceptPrivateKeyAsync(group, requestId.ToString());
                await Assert.ThrowsAsync<KeyVaultErrorException>(async () =>
                {
                    privateKey = await keyVault.LoadPrivateKeyAsync(group, requestId.ToString(), randomApp.PrivateKeyFormat);
                });
                await keyVault.AcceptPrivateKeyAsync(group, requestId.ToString());
                await keyVault.DeletePrivateKeyAsync(group, requestId.ToString());
                await Assert.ThrowsAsync<KeyVaultErrorException>(async () =>
                {
                    await keyVault.DeletePrivateKeyAsync(group, requestId.ToString());
                });
            }
        }

        [SkippableFact, Trait(Constants.Type, Constants.UnitTest), TestPriority(3000)]
        public async Task GetTrustListAsync()
        {
            SkipOnInvalidConfiguration();
            KeyVaultCertificateGroup keyVault = new KeyVaultCertificateGroup(_serviceConfig, _clientConfig, _logger);
            await keyVault.Init();
            string[] groups = await keyVault.GetCertificateGroupIds();
            foreach (string group in groups)
            {
                var trustList = await keyVault.GetTrustListAsync(group, 2);
                string nextPageLink = trustList.NextPageLink;
                while (nextPageLink != null)
                {
                    var nextTrustList = await keyVault.GetTrustListAsync(group, 2, nextPageLink);
                    trustList.AddRange(nextTrustList);
                    nextPageLink = nextTrustList.NextPageLink;
                }
                var validator = X509TestUtils.CreateValidatorAsync(trustList);
            }
        }

        [SkippableFact, Trait(Constants.Type, Constants.UnitTest), TestPriority(2000)]
        public async Task CreateCAAndAppCertificatesThenRevokeAll()
        {
            X509Certificate2Collection certCollection = new X509Certificate2Collection();
            for (int i = 0; i < 3; i++)
            {
                await KeyVaultCreateCACertificateAsync();
                for (int v = 0; v < 10; v++)
                {
                    certCollection.AddRange(await KeyVaultSigningRequestAsync());
                    certCollection.AddRange(await KeyVaultNewKeyPairRequestAsync());
                }
            }

            KeyVaultCertificateGroup keyVault = new KeyVaultCertificateGroup(_serviceConfig, _clientConfig, _logger);
            await keyVault.Init();
            string[] groups = await keyVault.GetCertificateGroupIds();

            // validate all certificates
            foreach (string group in groups)
            {
                var trustList = await keyVault.GetTrustListAsync(group);
                string nextPageLink = trustList.NextPageLink;
                while (nextPageLink != null)
                {
                    var nextTrustList = await keyVault.GetTrustListAsync(group, null, nextPageLink);
                    trustList.AddRange(nextTrustList);
                    nextPageLink = nextTrustList.NextPageLink;
                }
                var validator = await X509TestUtils.CreateValidatorAsync(trustList);
                foreach (var cert in certCollection)
                {
                    validator.Validate(cert);
                }
            }

            // now revoke all certifcates
            var revokeCertificates = new X509Certificate2Collection(certCollection);
            foreach (string group in groups)
            {
                var unrevokedCertificates = await keyVault.RevokeCertificatesAsync(group, revokeCertificates);
                Assert.True(unrevokedCertificates.Count <= revokeCertificates.Count);
                revokeCertificates = unrevokedCertificates;
            }
            Assert.Empty(revokeCertificates);

            // reload updated trust list from KeyVault
            var trustListAllGroups = new KeyVaultTrustListModel("all");
            foreach (string group in groups)
            {
                var trustList = await keyVault.GetTrustListAsync(group);
                string nextPageLink = trustList.NextPageLink;
                while (nextPageLink != null)
                {
                    var nextTrustList = await keyVault.GetTrustListAsync(group, null, nextPageLink);
                    trustList.AddRange(nextTrustList);
                    nextPageLink = nextTrustList.NextPageLink;
                }
                trustListAllGroups.AddRange(trustList);
            }

            // verify certificates are revoked
            {
                var validator = await X509TestUtils.CreateValidatorAsync(trustListAllGroups);
                foreach (var cert in certCollection)
                {
                    Assert.Throws<Opc.Ua.ServiceResultException>(() =>
                    {
                        validator.Validate(cert);
                    });
                }
            }
        }

        private void SkipOnInvalidConfiguration()
        {
            Skip.If(
                _serviceConfig.KeyVaultBaseUrl == null ||
                _serviceConfig.KeyVaultResourceId == null ||
                _clientConfig.AppId == null ||
                _clientConfig.AppSecret == null,
                "Missing valid KeyVault configuration");
        }

        /// <summary>The test logger</summary>
        private readonly ITestOutputHelper _log;
        private ApplicationTestDataGenerator _randomGenerator;
    }


}
