// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.OpcGds.Services.Exceptions;
using Newtonsoft.Json;
using Opc.Ua;
using Opc.Ua.Gds.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoTSolutions.OpcGds.Services.Models
{
    public class KeyVaultTrustList
    {
        public readonly string Id;
        public X509Certificate2Collection IssuerCertificates;
        public ICollection<X509CRL> IssuerCrls;
        public X509Certificate2Collection TrustedCertificates;
        public ICollection<X509CRL> TrustedCrls;

        public KeyVaultTrustList(string id)
        {
            Id = id;
            IssuerCertificates = new X509Certificate2Collection();
            IssuerCrls = new List<X509CRL>();
            TrustedCertificates = new X509Certificate2Collection();
            TrustedCrls = new List<X509CRL>();
        }
    }


    public sealed class KeyVaultCertificateGroup : Opc.Ua.Gds.Server.CertificateGroup
    {
        public X509CRL Crl;

        private KeyVaultCertificateGroup(
            KeyVaultServiceClient keyVaultServiceClient,
            CertificateGroupConfiguration certificateGroupConfiguration
            )
            :
            base(null, certificateGroupConfiguration)
        {
            _keyVaultServiceClient = keyVaultServiceClient;
            Certificate = null;
            Crl = null;
        }

        public static KeyVaultCertificateGroup Create(
            KeyVaultServiceClient keyVaultServiceClient,
            CertificateGroupConfiguration certificateGroupConfiguration)
        {
            return new KeyVaultCertificateGroup(keyVaultServiceClient, certificateGroupConfiguration);
        }

        public static async Task<KeyVaultCertificateGroup> Create(
            KeyVaultServiceClient keyVaultServiceClient,
            string id)
        {
            var certificateGroupConfiguration = await GetCertificateGroupConfiguration(keyVaultServiceClient, id);
            return new KeyVaultCertificateGroup(keyVaultServiceClient, certificateGroupConfiguration);
        }

        public static async Task<string[]> GetCertificateGroupIds(
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
            try
            {
                Utils.Trace(Utils.TraceMasks.Information, "InitializeCertificateGroup: {0}", m_subjectName);
                var result = await _keyVaultServiceClient.GetCertificateAsync(Configuration.Id).ConfigureAwait(false);
                Certificate = new X509Certificate2(result.Cer);
                if (Opc.Ua.Utils.CompareDistinguishedName(Certificate.Subject, Configuration.SubjectName))
                {
                    _caCertSecretIdentifier = result.SecretIdentifier.Identifier;
                    Crl = await _keyVaultServiceClient.LoadCACrl(Configuration.Id, Certificate);
                }
                else
                {
                    throw new InvalidConfigurationException("Key Vault certificate subject(" + Certificate.Subject + ") does not match cert group subject " + Configuration.SubjectName);
                }
            }
            catch (Exception e)
            {
                _caCertSecretIdentifier = null;
                Certificate = null;
                Crl = null;
                throw e;
            }
        }

        public async Task<bool> CreateCACertificateAsync()
        {
            DateTime yesterday = DateTime.UtcNow.AddDays(-1);
            try
            {
                var caCert = CertificateFactory.CreateCertificate(
                    null,
                    null,
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
                Crl = CertificateFactory.RevokeCertificate(caCert, null, null);

                // upload ca cert with private key
                await _keyVaultServiceClient.ImportCACertificate(Configuration.Id, caCert, true).ConfigureAwait(false);
                await _keyVaultServiceClient.ImportCACrl(Configuration.Id, Certificate, Crl).ConfigureAwait(false);

            }
            catch
            {
                return false;
            }
            return true;
        }

        public override async Task RevokeCertificateAsync(
            X509Certificate2 certificate)
        {
            await LoadPublicAssets().ConfigureAwait(false);
            var issuerCert = await LoadSigningKeyAsync(null, null).ConfigureAwait(false);
            var certificates = new X509Certificate2Collection() { certificate };
            var crls = new List<X509CRL>() { Crl };
            Crl = CertificateFactory.RevokeCertificate(issuerCert, crls, certificates);
            await _keyVaultServiceClient.ImportCACrl(Configuration.Id, Certificate, Crl).ConfigureAwait(false);
        }

        public async Task<X509Certificate2> GetCACertificateAsync(string id)
        {
            await LoadPublicAssets().ConfigureAwait(false);
            return Certificate;
        }

        public async Task<X509CRL> GetCACrlAsync(string id)
        {
            await LoadPublicAssets().ConfigureAwait(false);
            return Crl;
        }
        #endregion

        #region Public Overrides
        public override async Task<X509Certificate2> LoadSigningKeyAsync(
            X509Certificate2 signingCertificate,
            string signingKeyPassword)
        {
            await LoadPublicAssets();
            return await _keyVaultServiceClient.LoadSigningCertificateAsync(
                _caCertSecretIdentifier,
                Certificate);
        }
        #endregion
        #region Private Methods
        private async Task LoadPublicAssets()
        {
            if (Certificate == null || _caCertSecretIdentifier == null)
            {
                await Init();
            }
        }
        #endregion
        #region Private Fields
        private KeyVaultServiceClient _keyVaultServiceClient;
        private string _caCertSecretIdentifier;
        #endregion

    }
}
