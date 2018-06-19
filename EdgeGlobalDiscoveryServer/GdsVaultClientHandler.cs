using Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.Client;
using Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.Client.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Opc.Ua.Gds.Server
{
    public class OpcGdsVaultConfig: IOpcGdsVaultConfig
    {
        public OpcGdsVaultConfig(string url)
        {
            OpcGdsVaultServiceApiUrl = url + "/v1";
        }
        public string OpcGdsVaultServiceApiUrl { get; }
    }

    public class OpcGdsVaultClientHandler
    {
        string _vaultBaseUrl;
        string _appId;
        IOpcGdsVaultClient _gdsVaultClient;
        IOpcGdsVaultConfig _gdsVaultConfig;
        ClientAssertionCertificate _assertionCert;

        public IOpcGdsVaultClient GdsVaultClient { get => _gdsVaultClient; }

        public OpcGdsVaultClientHandler(string vaultBaseUrl)
        {
            _vaultBaseUrl = vaultBaseUrl;
            _gdsVaultConfig = new OpcGdsVaultConfig(_vaultBaseUrl);
        }

        public void SetAssertionCertificate(
            string appId,
            X509Certificate2 clientAssertionCertPfx)
        {
            _appId = appId;
            _assertionCert = new ClientAssertionCertificate(appId, clientAssertionCertPfx);
            _gdsVaultClient = new OpcGdsVaultClient(
                _gdsVaultConfig,
                new AuthenticationCallback(GetAccessTokenAsync));
        }

        public void SetTokenProvider()
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            _gdsVaultClient = new OpcGdsVaultClient(
                _gdsVaultConfig,
                new AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
        }

        private async Task<string> GetAccessTokenAsync(string authority, string resource, string scope)
        {
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, _assertionCert);
            return result.AccessToken;
        }

        public async Task<string> GetIotHubSecretAsync()
        {
            var secret = await _gdsVaultClient.GetIotHubSecretAsync().ConfigureAwait(false);
            return secret.Secret;
        }

        public async Task<X509Certificate2Collection> GetCACertificateChainAsync(string id)
        {
            var result = new X509Certificate2Collection();
            var chainApiModel = await _gdsVaultClient.GetCACertificateChainAsync(id).ConfigureAwait(false);
            foreach (var certApiModel in chainApiModel.Chain)
            {
                var cert = new X509Certificate2(Convert.FromBase64String(certApiModel.Certificate));
                result.Add(cert);
            }
            return result;
        }

        public async Task<IList<Opc.Ua.X509CRL>> GetCACrlChainAsync(string id)
        {
            var result = new List<Opc.Ua.X509CRL>();
            var chainApiModel = await _gdsVaultClient.GetCACrlChainAsync(id).ConfigureAwait(false);
            foreach (var certApiModel in chainApiModel.Chain)
            {
                var crl = new Opc.Ua.X509CRL(Convert.FromBase64String(certApiModel.Crl));
                result.Add(crl);
            }
            return result;
        }

        public async Task<CertificateGroupConfigurationCollection> GetCertificateConfigurationGroupsAsync(string baseStorePath)
        {
            var groups = await _gdsVaultClient.GetCertificateGroupConfiguration().ConfigureAwait(false);
            var groupCollection = new CertificateGroupConfigurationCollection();
            foreach (var group in groups.Groups)
            {
                var newGroup = new CertificateGroupConfiguration()
                {
                    Id = group.Id,
                    CertificateType = group.CertificateType,
                    SubjectName = group.SubjectName,
                    BaseStorePath = baseStorePath + Path.DirectorySeparatorChar + group.Id,
                    DefaultCertificateHashSize = group.DefaultCertificateHashSize,
                    DefaultCertificateKeySize = group.DefaultCertificateKeySize,
                    DefaultCertificateLifetime = group.DefaultCertificateLifetime,
                    CACertificateHashSize = group.CACertificateHashSize,
                    CACertificateKeySize = group.CACertificateKeySize,
                    CACertificateLifetime = group.CACertificateLifetime
                };
                groupCollection.Add(newGroup);
            }
            return groupCollection;
        }

        public async Task<X509Certificate2> SigningRequestAsync(
            string id,
            ApplicationRecordDataType application,
            byte[] certificateRequest)
        {
            var sr = new SigningRequestApiModel()
            {
                ApplicationURI = application.ApplicationUri,
                Csr = Convert.ToBase64String(certificateRequest)
            };
            var certModel = await _gdsVaultClient.SigningRequestAsync(id, sr).ConfigureAwait(false);
            return new X509Certificate2(Convert.FromBase64String(certModel.Certificate));
        }

        public async Task<Opc.Ua.X509CRL> RevokeCertificateAsync(
            string id,
            X509Certificate2 certificate)
        {
            var certModel = new X509Certificate2ApiModel()
            {
                Certificate = Convert.ToBase64String(certificate.RawData),
                Subject = certificate.Subject,
                Thumbprint = certificate.Thumbprint
            };
            var crlModel = await _gdsVaultClient.RevokeCertificateAsync(id, certModel).ConfigureAwait(false);
            return new Opc.Ua.X509CRL(Convert.FromBase64String(crlModel.Crl));

        }

        public async Task<X509Certificate2KeyPair> NewKeyPairRequestAsync(
            string id,
            ApplicationRecordDataType application,
            string subjectName,
            string[] domainNames,
            string privateKeyFormat,
            string privateKeyPassword)
        {
            var certModel = new NewKeyPairRequestApiModel()
            {
                ApplicationURI = application.ApplicationUri,
                SubjectName = subjectName,
                DomainNames = domainNames,
                PrivateKeyFormat = privateKeyFormat,
                PrivateKeyPassword = privateKeyPassword
            };
            var nkpModel = await _gdsVaultClient.NewKeyPairRequestAsync(id, certModel).ConfigureAwait(false);
            return new X509Certificate2KeyPair(
                new X509Certificate2(Convert.FromBase64String(nkpModel.Certificate)),
                nkpModel.PrivateKeyFormat,
                Convert.FromBase64String(nkpModel.PrivateKey));
        }

    }
}

