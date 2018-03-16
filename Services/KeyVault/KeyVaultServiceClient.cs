// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.Services
{

    public class KeyVaultServiceClient
    {
        const int MaxResults = 10;
        // see RFC 2585
        const string ContentTypeCert = "application/pkix-cert";
        const string ContentTypeCrl = "application/pkix-crl";
        // trust list tags
        const string TagIssuerList = "Issuer";
        const string TagTrustedList = "Trusted";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vaultBaseUrl">The Url of the Key Vault.</param>
        public KeyVaultServiceClient(string vaultBaseUrl)
        {
            _vaultBaseUrl = vaultBaseUrl;
        }

        /// <summary>
        /// Set appID and client certificate for keyVault authentication.
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="clientAssertionCertPfx"></param>
        public void SetAuthenticationAssertionCertificate(
            string appId,
            X509Certificate2 clientAssertionCertPfx)
        {
            _assertionCert = new ClientAssertionCertificate(appId, clientAssertionCertPfx);
            _keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(GetAccessTokenAsync));
        }

        /// <summary>
        /// Authentication for MSI or dev user callback.
        /// </summary>
        public void SetAuthenticationTokenProvider()
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            _keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
        }


        private async Task<string> GetAccessTokenAsync(string authority, string resource, string scope)
        {
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, _assertionCert);
            return result.AccessToken;
        }

        /// <summary>
        /// Read the IoTHub connection string from keyVault.
        /// </summary>
        public async Task<string> GetIotHubSecretAsync(CancellationToken ct = default(CancellationToken))
        {
            SecretBundle secret = await _keyVaultClient.GetSecretAsync(_vaultBaseUrl + "/secrets/iothub", ct).ConfigureAwait(false);
            return secret.Value;
        }

        /// <summary>
        /// Read the GDS CertificateConfigurationGroups as Json.
        /// </summary>
        public async Task<string> GetCertificateConfigurationGroupsAsync(CancellationToken ct = default(CancellationToken))
        {
            SecretBundle secret = await _keyVaultClient.GetSecretAsync(_vaultBaseUrl + "/secrets/groups", ct).ConfigureAwait(false);
            return secret.Value;
        }

        internal async Task<CertificateBundle> GetCertificateAsync(string name, CancellationToken ct = default(CancellationToken))
        {
            return await _keyVaultClient.GetCertificateAsync(_vaultBaseUrl, name, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Read all certificate versions of a CA certificate group.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<X509Certificate2Collection> GetCertificateVersionsAsync(string id, CancellationToken ct = default(CancellationToken))
        {
            var certificates = new X509Certificate2Collection();
            try
            {
                var certItems = await _keyVaultClient.GetCertificateVersionsAsync(_vaultBaseUrl, id, MaxResults, ct).ConfigureAwait(false);
                while (certItems != null)
                {
                    foreach (var certItem in certItems)
                    {
                        var certBundle = await _keyVaultClient.GetCertificateAsync(certItem.Id, ct).ConfigureAwait(false);
                        var cert = new X509Certificate2(certBundle.Cer);
                        certificates.Add(cert);
                    }
                    if (certItems.NextPageLink != null)
                    {
                        certItems = await _keyVaultClient.GetCertificateVersionsNextAsync(certItems.NextPageLink, ct).ConfigureAwait(false);
                    }
                    else
                    {
                        certItems = null;
                    }
                }
            }
            catch (Exception)
            {
                // TODO
                //Utils.Trace("Error while loading the certificate versions for " + id);
                //Utils.Trace("Exception: " + ex.Message);
            }
            return certificates;
        }

        /// <summary>
        /// Load the signing CA certificate for signing operations.
        /// </summary>
        internal async Task<X509Certificate2> LoadSigningCertificateAsync(string signingCertificateKey, X509Certificate2 publicCert, CancellationToken ct = default(CancellationToken))
        {
            var secret = await _keyVaultClient.GetSecretAsync(signingCertificateKey,ct );
            if (secret.ContentType == CertificateContentType.Pfx)
            {
                var certBlob = Convert.FromBase64String(secret.Value);
                return CertificateFactory.CreateCertificateFromPKCS12(certBlob, string.Empty);
            }
            else if (secret.ContentType == CertificateContentType.Pem)
            {
                Encoding encoder = Encoding.UTF8;
                var privateKey = encoder.GetBytes(secret.Value.ToCharArray());
                return CertificateFactory.CreateCertificateWithPEMPrivateKey(publicCert, privateKey, string.Empty);
            }

            throw new NotImplementedException("Unknown content type: " + secret.ContentType);
        }

        /// <summary>
        /// Sign a digest with the signing key.
        /// </summary>
        public async Task SignDigestAsync(string signingKey, byte[] digest, CancellationToken ct = default(CancellationToken))
        {
            var result = await _keyVaultClient.SignAsync(signingKey, JsonWebKeySignatureAlgorithm.RS256, digest, ct);
        }

        /// <summary>
        /// Imports a new CA certificate in group id, tags it for trusted or issuer store.
        /// </summary>
        public async Task ImportCACertificate(string id, X509Certificate2 certificate, bool trusted, CancellationToken ct = default(CancellationToken))
        {
            CertificateAttributes attributes = new CertificateAttributes
            {
                Enabled = true,
                Expires = certificate.NotAfter,
                NotBefore = certificate.NotBefore,
            };

            var policy = new CertificatePolicy
            {
                IssuerParameters = new IssuerParameters
                {
                    Name = "Self",
                },
                KeyProperties = new KeyProperties
                {
                    Exportable = true,
                    KeySize = certificate.GetRSAPublicKey().KeySize,
                    KeyType = "RSA"
                },
                SecretProperties = new SecretProperties
                {
                    ContentType = CertificateContentType.Pfx
                },
                X509CertificateProperties = new X509CertificateProperties
                {
                    Subject = certificate.Subject
                }
            };

            Dictionary<string, string> tags = new Dictionary<string, string>();
            tags[id] = trusted ? TagTrustedList : TagIssuerList;

            var result = await _keyVaultClient.ImportCertificateAsync(
                _vaultBaseUrl,
                id,
                new X509Certificate2Collection(certificate),
                policy,
                attributes,
                tags,
                ct)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Imports a new CRL for group id.
        /// </summary>
        public async Task ImportCACrl(string id, X509Certificate2 certificate, Opc.Ua.X509CRL crl, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                string secretIdentifier = CrlSecretName(id, certificate);
                SecretAttributes secretAttributes = new SecretAttributes()
                {
                    Enabled = true,
                    Expires = crl.NextUpdateTime,
                    NotBefore = crl.UpdateTime
                };

                // do not set tag for a CRL, the CA cert is already tagged.

                var result = await _keyVaultClient.SetSecretAsync(
                    _vaultBaseUrl, 
                    secretIdentifier, 
                    Convert.ToBase64String(crl.RawData), 
                    null, 
                    ContentTypeCrl, 
                    secretAttributes,
                    ct)
                    .ConfigureAwait(false);
            }
            catch (Exception)
            {
                // TODO: add logging (is this a fatal error?)
            }
        }

        /// <summary>
        /// Load CRL for CA cert in group.
        /// </summary>
        public async Task<Opc.Ua.X509CRL> LoadCACrl(string id, X509Certificate2 certificate, CancellationToken ct = default(CancellationToken))
        {
            string secretIdentifier = CrlSecretName(id, certificate);
            var secret = await _keyVaultClient.GetSecretAsync(_vaultBaseUrl, secretIdentifier, ct).ConfigureAwait(false);
            if (secret.ContentType == ContentTypeCrl)
            {
                var crlBlob = Convert.FromBase64String(secret.Value);
                return new Opc.Ua.X509CRL(crlBlob);
            }
            return null;
        }

        /// <summary>
        /// Creates a trust list with all certs and crls in issuer and trusted list.
        /// i) First load all certs and crls tagged with id==Issuer or id==Trusted.
        /// ii) Then walk all CA cert versions and load all certs tagged with id==Issuer or id==Trusted. 
        ///     Crl is loaded too if CA cert is tagged.
        /// </summary>
        public async Task<Models.KeyVaultTrustList> GetTrustListAsync(string id, CancellationToken ct = default(CancellationToken))
        {
            var trustList = new Models.KeyVaultTrustList(id);
            var secretItems = await _keyVaultClient.GetSecretsAsync(_vaultBaseUrl, MaxResults, ct).ConfigureAwait(false);

            while (secretItems != null)
            {
                foreach (var secretItem in secretItems.Where(s => s.Tags != null))
                {
                    string tag;
                    secretItem.Tags.TryGetValue(id, out tag);
                    bool issuer = tag == TagIssuerList;
                    bool trusted = tag == TagTrustedList;
                    if ((issuer || trusted) &&
                        (secretItem.ContentType == ContentTypeCert ||
                         secretItem.ContentType == ContentTypeCrl))
                    {
                        X509CRL crl = null;
                        X509Certificate2 cert = null;
                        if (secretItem.ContentType == ContentTypeCert)
                        {
                            var certCollection = issuer ? trustList.IssuerCertificates : trustList.TrustedCertificates;
                            cert = await LoadCertSecret(secretItem.Identifier.Name, ct).ConfigureAwait(false);
                            certCollection.Add(cert);
                        }
                        else
                        {
                            var crlCollection = issuer ? trustList.IssuerCrls : trustList.TrustedCrls;
                            crl = await LoadCrlSecret(secretItem.Identifier.Name, ct).ConfigureAwait(false);
                            crlCollection.Add(crl);
                        }
                    }
                }

                if (secretItems.NextPageLink != null)
                {
                    secretItems = await _keyVaultClient.GetSecretsNextAsync(secretItems.NextPageLink, ct).ConfigureAwait(false);
                }
                else
                {
                    secretItems = null;
                }
            }

            var certItems = await _keyVaultClient.GetCertificateVersionsAsync(_vaultBaseUrl, id, MaxResults, ct).ConfigureAwait(false);
            while (certItems != null)
            {
                foreach (var certItem in certItems.Where(c => c.Tags != null))
                {
                    string tag;
                    certItem.Tags.TryGetValue(id, out tag);
                    bool issuer = tag == TagIssuerList;
                    bool trusted = tag == TagTrustedList;

                    if (issuer || trusted)
                    {
                        var certBundle = await _keyVaultClient.GetCertificateAsync(certItem.Id, ct).ConfigureAwait(false);
                        var cert = new X509Certificate2(certBundle.Cer);
                        var crl = await LoadCACrl(id, cert, ct);
                        if (certItem.Tags[id] == TagIssuerList)
                        {
                            trustList.IssuerCertificates.Add(cert);
                            trustList.IssuerCrls.Add(crl);
                        }
                        else
                        {
                            trustList.TrustedCertificates.Add(cert);
                            trustList.TrustedCrls.Add(crl);
                        }
                    }
                }
                if (certItems.NextPageLink != null)
                {
                    certItems = await _keyVaultClient.GetCertificateVersionsNextAsync(certItems.NextPageLink, ct).ConfigureAwait(false);
                }
                else
                {
                    certItems = null;
                }
            }

            return trustList;
        }

        private string CrlSecretName(string name, X509Certificate2 certificate)
        {
            return name + "Crl" + certificate.Thumbprint;
        }

        private async Task<X509CRL> LoadCrlSecret(string secretIdentifier, CancellationToken ct = default(CancellationToken))
        {
            var secret = await _keyVaultClient.GetSecretAsync(_vaultBaseUrl, secretIdentifier, ct).ConfigureAwait(false); ;
            if (secret.ContentType == ContentTypeCrl)
            {
                var crlBlob = Convert.FromBase64String(secret.Value);
                return new Opc.Ua.X509CRL(crlBlob);
            }
            return null;
        }

        private async Task<X509Certificate2> LoadCertSecret(string secretIdentifier, CancellationToken ct = default(CancellationToken))
        {
            var secret = await _keyVaultClient.GetSecretAsync(_vaultBaseUrl, secretIdentifier, ct).ConfigureAwait(false); ;
            if (secret.ContentType == ContentTypeCrl)
            {
                var certBlob = Convert.FromBase64String(secret.Value);
                return new X509Certificate2(certBlob);
            }
            return null;
        }

        private string _vaultBaseUrl;
        private IKeyVaultClient _keyVaultClient;
        private ClientAssertionCertificate _assertionCert;
    }
}

