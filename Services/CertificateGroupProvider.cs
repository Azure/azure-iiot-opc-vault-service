// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.OpcGds.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.OpcGds.Services.Models;
using Microsoft.Azure.IoTSolutions.OpcGds.Services.Runtime;
using Microsoft.Azure.KeyVault.Models;
using Newtonsoft.Json;
using Opc.Ua;
using Opc.Ua.Gds;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoTSolutions.OpcGds.Services
{
    public interface ICertificateGroupProvider
    {
        Task<X509Certificate2> SigningRequestAsync(
            string id,
            string applicationUri,
            byte[] certificateRequest
            );
        Task<X509CRL> RevokeCertificateAsync(
            string id,
            X509Certificate2 certificate
            );
        Task<X509Certificate2> CreateCACertificateAsync(
            string id
            );
        Task<X509Certificate2> NewKeyPairRequestAsync(
            string id,
            string applicationUri,
            string subjectName,
            string[] domainNames,
            string privateKeyFormat,
            string privateKeyPassword
            );
    }

    public sealed class CertificateGroupProvider : ICertificateGroupProvider
    {
        public CertificateGroupProvider(
            IServicesConfig config,
            ILogger logger)
        {
            // TODO: use config
            _keyVaultServiceClient = new KeyVaultServiceClient("https://iopgds.vault.azure.net");
            _keyVaultServiceClient.SetAuthenticationTokenProvider();
            _log = logger;
            _log.Debug("Creating new instance of `KeyVault` service", () => { });
        }

        public async Task Init()
        {
            string json = await _keyVaultServiceClient.GetCertificateConfigurationGroupsAsync();
            List<Opc.Ua.Gds.Server.CertificateGroupConfiguration> certificateGroupCollection = JsonConvert.DeserializeObject<List<Opc.Ua.Gds.Server.CertificateGroupConfiguration>>(json);

            foreach (var certificateGroupConfiguration in certificateGroupCollection)
            {
                KeyVaultCertificateGroupProvider certificateGroup = null;
                try
                {
                    certificateGroup = KeyVaultCertificateGroupProvider.Create(_keyVaultServiceClient, certificateGroupConfiguration);
                    await certificateGroup.Init().ConfigureAwait(false);
                    await certificateGroup.LoadSigningKeyAsync(null, null);
                    continue;
                }
                catch (Exception ex)
                {
                    if (certificateGroup == null)
                    {
                        throw ex;
                    }
                }

                if (!await certificateGroup.CreateCACertificateAsync().ConfigureAwait(false))
                {

                }
            }
        }

        public async Task<X509CRL> RevokeCertificateAsync(string id, X509Certificate2 certificate)
        {
            var certificateGroup = await KeyVaultCertificateGroupProvider.Create(_keyVaultServiceClient, id).ConfigureAwait(false);
            await certificateGroup.RevokeCertificateAsync(certificate).ConfigureAwait(false); ;
            return certificateGroup.Crl;
        }

        public async Task<X509Certificate2> CreateCACertificateAsync(string id)
        {
            var certificateGroup = await KeyVaultCertificateGroupProvider.Create(_keyVaultServiceClient, id).ConfigureAwait(false);
            if (await certificateGroup.CreateCACertificateAsync().ConfigureAwait(false))
            {
                return certificateGroup.Certificate;
            }
            return null;
        }

        public async Task<X509Certificate2> SigningRequestAsync(
            string id,
            string applicationUri,
            byte[] certificateRequest
            )
        {
            var certificateGroup = await KeyVaultCertificateGroupProvider.Create(_keyVaultServiceClient, id).ConfigureAwait(false); ;
            ApplicationRecordDataType app = new ApplicationRecordDataType
            {
                ApplicationNames = new Opc.Ua.LocalizedTextCollection(),
                ApplicationUri = applicationUri
            };
            return await certificateGroup.SigningRequestAsync(app, null, certificateRequest).ConfigureAwait(false); ;
        }

        public async Task<X509Certificate2> NewKeyPairRequestAsync(
            string id,
            string applicationUri,
            string subjectName,
            string[] domainNames,
            string privateKeyFormat,
            string privateKeyPassword
            )
        {
            var certificateGroup = await KeyVaultCertificateGroupProvider.Create(_keyVaultServiceClient, id).ConfigureAwait(false); ;
            ApplicationRecordDataType app = new ApplicationRecordDataType();
            app.ApplicationNames = new Opc.Ua.LocalizedTextCollection();
            app.ApplicationUri = applicationUri;
            return await certificateGroup.NewKeyPairRequestAsync(app, subjectName, domainNames).ConfigureAwait(false); ;
        }

        public async Task<string[]> GetCertificateGroupIds()
        {
            return await KeyVaultCertificateGroupProvider.GetCertificateGroupIds(_keyVaultServiceClient).ConfigureAwait(false); ;
        }

        public async Task<Opc.Ua.Gds.Server.CertificateGroupConfiguration> GetCertificateGroupConfiguration(string id)
        {
            return await KeyVaultCertificateGroupProvider.GetCertificateGroupConfiguration(_keyVaultServiceClient, id).ConfigureAwait(false);
        }

        public async Task<X509Certificate2> GetCACertificateAsync(string id)
        {
            var certificateGroup = await KeyVaultCertificateGroupProvider.Create(_keyVaultServiceClient, id).ConfigureAwait(false); ;
            return await certificateGroup.GetCACertificateAsync(id).ConfigureAwait(false);
        }

        public async Task<X509CRL> GetCACrlAsync(string id)
        {
            var certificateGroup = await KeyVaultCertificateGroupProvider.Create(_keyVaultServiceClient, id).ConfigureAwait(false); ;
            return await certificateGroup.GetCACrlAsync(id).ConfigureAwait(false);
        }

        private async Task<string> GetIotHubSecretAsync()
        {
            return await _keyVaultServiceClient.GetIotHubSecretAsync().ConfigureAwait(false);
        }

        private async Task<X509Certificate2Collection> GetCertificateVersionsAsync(string id)
        {
            return await _keyVaultServiceClient.GetCertificateVersionsAsync(id).ConfigureAwait(false);
        }

        private readonly KeyVaultServiceClient _keyVaultServiceClient;
        private readonly ILogger _log;

    }
}
