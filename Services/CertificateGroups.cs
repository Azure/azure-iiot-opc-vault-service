// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.GdsVault.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.GdsVault.Services.Models;
using Microsoft.Azure.IoTSolutions.GdsVault.Services.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoTSolutions.GdsVault.Services
{
    public interface ICertificateGroups
    {
        Task<string[]> GetCertificateGroupIds();
        Task<Opc.Ua.Gds.Server.CertificateGroupConfiguration> GetCertificateGroupConfiguration(string id);
        Task<Opc.Ua.Gds.Server.CertificateGroupConfigurationCollection> GetCertificateGroupConfigurationCollection();

        Task<X509Certificate2> SigningRequestAsync(
            string id,
            string applicationUri,
            byte[] certificateRequest
            );
        Task<Opc.Ua.X509CRL> RevokeCertificateAsync(
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
            string[] domainNames
            );
    }

    public sealed class CertificateGroups : ICertificateGroups
    {
        public CertificateGroups(
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
            var certificateGroupCollection = await GetCertificateGroupConfigurationCollection().ConfigureAwait(false);
            foreach (var certificateGroupConfiguration in certificateGroupCollection)
            {
                KeyVaultCertificateGroup certificateGroup = null;
                try
                {
                    certificateGroup = KeyVaultCertificateGroup.Create(_keyVaultServiceClient, certificateGroupConfiguration);
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

        public async Task<string[]> GetCertificateGroupIds()
        {
            return await KeyVaultCertificateGroup.GetCertificateGroupIds(_keyVaultServiceClient).ConfigureAwait(false); ;
        }

        public async Task<Opc.Ua.Gds.Server.CertificateGroupConfiguration> GetCertificateGroupConfiguration(string id)
        {
            return await KeyVaultCertificateGroup.GetCertificateGroupConfiguration(_keyVaultServiceClient, id).ConfigureAwait(false);
        }

        public async Task<Opc.Ua.Gds.Server.CertificateGroupConfigurationCollection> GetCertificateGroupConfigurationCollection()
        {
            string json = await _keyVaultServiceClient.GetCertificateConfigurationGroupsAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<Opc.Ua.Gds.Server.CertificateGroupConfigurationCollection>(json);
        }

        public async Task<Opc.Ua.X509CRL> RevokeCertificateAsync(string id, X509Certificate2 certificate)
        {
            var certificateGroup = await KeyVaultCertificateGroup.Create(_keyVaultServiceClient, id).ConfigureAwait(false);
            await certificateGroup.RevokeCertificateAsync(certificate).ConfigureAwait(false); ;
            return certificateGroup.Crl;
        }

        public async Task<X509Certificate2> CreateCACertificateAsync(string id)
        {
            var certificateGroup = await KeyVaultCertificateGroup.Create(_keyVaultServiceClient, id).ConfigureAwait(false);
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
            var certificateGroup = await KeyVaultCertificateGroup.Create(_keyVaultServiceClient, id).ConfigureAwait(false); ;
            var app = new Opc.Ua.Gds.ApplicationRecordDataType
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
            string[] domainNames
            )
        {
            var certificateGroup = await KeyVaultCertificateGroup.Create(_keyVaultServiceClient, id).ConfigureAwait(false); ;
            var app = new Opc.Ua.Gds.ApplicationRecordDataType();
            app.ApplicationNames = new Opc.Ua.LocalizedTextCollection();
            app.ApplicationUri = applicationUri;
            return await certificateGroup.NewKeyPairRequestAsync(app, subjectName, domainNames).ConfigureAwait(false); ;
        }

        public async Task<X509Certificate2> GetCACertificateAsync(string id)
        {
            var certificateGroup = await KeyVaultCertificateGroup.Create(_keyVaultServiceClient, id).ConfigureAwait(false); ;
            return await certificateGroup.GetCACertificateAsync(id).ConfigureAwait(false);
        }

        public async Task<Opc.Ua.X509CRL> GetCACrlAsync(string id)
        {
            var certificateGroup = await KeyVaultCertificateGroup.Create(_keyVaultServiceClient, id).ConfigureAwait(false); ;
            return await certificateGroup.GetCACrlAsync(id).ConfigureAwait(false);
        }

        public async Task GetTrustListAsync(string id)
        {
            await _keyVaultServiceClient.GetTrustListAsync(id).ConfigureAwait(false);
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
