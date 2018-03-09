// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.OpcGds.Services.Exceptions;
using Newtonsoft.Json;
using Opc.Ua;
using Opc.Ua.Gds.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoTSolutions.OpcGds.Services.Models
{
    public sealed class KeyVaultCertificateGroupProvider : Opc.Ua.Gds.Server.CertificateGroup
    {
        public X509CRL Crl;

        private KeyVaultCertificateGroupProvider(
            KeyVaultServiceClient keyVaultServiceClient,
            CertificateGroupConfiguration certificateGroupConfiguration
            ) 
            :
            base (Path.GetTempPath() + "authorities" + Path.DirectorySeparatorChar + certificateGroupConfiguration.Id, certificateGroupConfiguration)
        {
            _keyVaultServiceClient = keyVaultServiceClient;
            Crl = null;
        }

        public static KeyVaultCertificateGroupProvider Create(
            KeyVaultServiceClient keyVaultServiceClient,
            CertificateGroupConfiguration certificateGroupConfiguration)
        {
            return new KeyVaultCertificateGroupProvider(keyVaultServiceClient, certificateGroupConfiguration);
        }

        public static async Task<KeyVaultCertificateGroupProvider> Create(
            KeyVaultServiceClient keyVaultServiceClient,
            string id)
        {
            var certificateGroupConfiguration = await GetCertificateGroupConfiguration(keyVaultServiceClient, id);
            return new KeyVaultCertificateGroupProvider(keyVaultServiceClient, certificateGroupConfiguration);
        }

        public static async Task<string []> GetCertificateGroupIds(
            KeyVaultServiceClient keyVaultServiceClient)
        {
            string json = await keyVaultServiceClient.GetCertificateConfigurationGroupsAsync().ConfigureAwait(false);
            List<Opc.Ua.Gds.Server.CertificateGroupConfiguration> certificateGroupCollection = JsonConvert.DeserializeObject<List<Opc.Ua.Gds.Server.CertificateGroupConfiguration>>(json);
            List<string> groups = certificateGroupCollection.Select(cg => cg.Id).ToList();
            return groups.ToArray();
        }

        public static async Task<CertificateGroupConfiguration> GetCertificateGroupConfiguration(
            KeyVaultServiceClient keyVaultServiceClient,
            string id)
        {
            string json = await keyVaultServiceClient.GetCertificateConfigurationGroupsAsync().ConfigureAwait(false);
            List<Opc.Ua.Gds.Server.CertificateGroupConfiguration> certificateGroupCollection = JsonConvert.DeserializeObject<List<Opc.Ua.Gds.Server.CertificateGroupConfiguration>>(json);
            return certificateGroupCollection.SingleOrDefault(cg => String.Equals(cg.Id, id, StringComparison.OrdinalIgnoreCase));
        }

        #region ICertificateGroupProvider
        public override async Task Init()
        {
            Utils.Trace(Utils.TraceMasks.Information, "InitializeCertificateGroup: {0}", m_subjectName);
            var result = await _keyVaultServiceClient.GetCertificateAsync(Configuration.Id).ConfigureAwait(false);
            Certificate = new X509Certificate2(result.Cer);
            if (Opc.Ua.Utils.CompareDistinguishedName(Certificate.Subject, Configuration.SubjectName))
            {
                _caCertSecretIdentifier = result.SecretIdentifier.Identifier;
            }
            else
            {
                Certificate = null;
                throw new InvalidConfigurationException("Key Vault certificate subject(" + Certificate.Subject + ") does not match cert group subject " + Configuration.SubjectName);
            }
        }

        public async Task<bool> CreateCACertificateAsync()
        {
            DateTime yesterday = DateTime.UtcNow.AddDays(-1);
            try
            {
                var caCert = CertificateFactory.CreateCertificate(
                    CertificateStoreType.Directory,
                    m_authoritiesStorePath,
                    null,
                    null,
                    null,
                    Configuration.SubjectName,
                    null,
                    Configuration.DefaultCertificateKeySize,
                    yesterday,
                    Configuration.DefaultCertificateLifetime,
                    Configuration.DefaultCertificateHashSize,
                    true,
                    null,
                    null);

                // save only public key
                Certificate = new X509Certificate2(caCert.RawData);

                // initialize revocation list
                Crl = await CertificateFactory.RevokeCertificateAsync(m_authoritiesStorePath, Certificate, null).ConfigureAwait(false);

                // upload ca cert with private key
                await _keyVaultServiceClient.UploadCACertificate(Configuration.Id, caCert).ConfigureAwait(false);
                //await this.keyVaultServiceClient.UploadCACrl(certificateGroup.Id, certificateGroup.Crl);

            }
            catch 
            {
                return false;
            }
            finally
            {
                using (ICertificateStore store = CertificateStoreIdentifier.OpenStore(m_authoritiesStorePath))
                {
                    store.DeleteCRL(Crl);
                    await store.Delete(Certificate.Thumbprint);
                }
            }
            return true;
        }

        #endregion

        #region Public Overrides
        public override async Task<X509Certificate2> LoadSigningKeyAsync(
            X509Certificate2 signingCertificate, 
            string signingKeyPassword)
        {
            if (Certificate == null || _caCertSecretIdentifier == null)
            {
                await Init();
            }
            return await _keyVaultServiceClient.LoadSigningCertificateAsync(
                _caCertSecretIdentifier,
                Certificate);
        }
        #endregion
        #region Private Fields
        private KeyVaultServiceClient _keyVaultServiceClient;
        private string _caCertSecretIdentifier;
        #endregion

    }
}
