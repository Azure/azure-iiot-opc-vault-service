
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Microsoft.Azure.IoTSolutions.OpcGds.Services
{

    public class KeyVaultServiceClient
    {
        // see RFC 2585
        const string ContentTypeCert = "application/pkix-cert";
        const string ContentTypeCrl = "application/pkix-crl";
        string _vaultBaseUrl;
        string _appId;
        IKeyVaultClient _keyVaultClient;
        ClientAssertionCertificate _assertionCert;

        public IKeyVaultClient KeyVaultClient { get => _keyVaultClient; }

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
            _appId = appId;
            _assertionCert = new ClientAssertionCertificate(appId, clientAssertionCertPfx);
            _keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(GetAccessTokenAsync));
        }

        /// <summary>
        /// Authentication callback.
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


        public async Task<string> GetIotHubSecretAsync()
        {
            SecretBundle secret = await _keyVaultClient.GetSecretAsync(_vaultBaseUrl + "/secrets/iothub").ConfigureAwait(false);
            return secret.Value;
        }

        public async Task<string> GetCertificateConfigurationGroupsAsync()
        {
            SecretBundle secret = await _keyVaultClient.GetSecretAsync(_vaultBaseUrl + "/secrets/groups").ConfigureAwait(false);
            return secret.Value;
        }

        public async Task<CertificateBundle> GetCertificateAsync(string name)
        {
            return await _keyVaultClient.GetCertificateAsync(_vaultBaseUrl, name).ConfigureAwait(false);
        }

        public async Task<X509Certificate2Collection> GetCertificateVersionsAsync(string id)
        {
            var certificates = new X509Certificate2Collection();
            try
            {
                var certItems = await _keyVaultClient.GetCertificateVersionsAsync(_vaultBaseUrl, id, 3).ConfigureAwait(false);
                while (certItems != null)
                {
                    foreach (var certItem in certItems)
                    {
                        var certBundle = await _keyVaultClient.GetCertificateAsync(certItem.Id).ConfigureAwait(false);
                        var cert = new X509Certificate2(certBundle.Cer);
                        certificates.Add(cert);
                    }
                    if (certItems.NextPageLink != null)
                    {
                        certItems = await _keyVaultClient.GetCertificateVersionsNextAsync(certItems.NextPageLink).ConfigureAwait(false);
                    }
                    else
                    {
                        certItems = null;
                    }
                }
            }
            catch (Exception )
            {
                //Utils.Trace("Error while loading the certificate versions for " + id);
                //Utils.Trace("Exception: " + ex.Message);
            }
            return certificates;
        }


        public async Task CreateCACertificateAsync(
            string name, 
            string subjectName, 
            int keySize)
        {
            CertificateAttributes attributes = new CertificateAttributes { Enabled = true };

            var policy = new CertificatePolicy
            {
                IssuerParameters = new IssuerParameters
                {
                    Name = "Self",
                },
                KeyProperties = new KeyProperties
                {
                    Exportable = true,
                    KeySize = keySize,
                    KeyType = "RSA"
                },
                SecretProperties = new SecretProperties
                {
                    ContentType = CertificateContentType.Pem
                },
                X509CertificateProperties = new X509CertificateProperties
                {
                    Subject = subjectName
                }
            };

            var pendingCertificate = await _keyVaultClient.CreateCertificateAsync(
                _vaultBaseUrl, name, policy, attributes);
            // TODO: wait for operation
            var pendingCertificateResponse = await _keyVaultClient.GetCertificateOperationAsync(
                _vaultBaseUrl, pendingCertificate.CertificateOperationIdentifier.Name);
        }

#if testcode
        public async Task<SecretBundle> ReadKeyWithCertAsync()
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

            try
            {
                var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessTokenAsync));

                SecretBundle secret = await _keyVaultClient.GetSecretAsync(_vaultBaseUrl + "/secrets/secret")
                    .ConfigureAwait(false);

                var certlist = await _keyVaultClient.GetCertificatesAsync(_vaultBaseUrl);
                var result = await _keyVaultClient.GetCertificateAsync(_vaultBaseUrl, "Default").ConfigureAwait(false);
                var cert = new X509Certificate2(result.Cer);
                var secretvalue = $"Secret: {secret.Value}";
                string principal = azureServiceTokenProvider.PrincipalUsed != null ?
                    $"Principal Used: {azureServiceTokenProvider.PrincipalUsed}" :
                    string.Empty;
                return secret;
            }
            catch (Exception exp)
            {
                var error = $"Something went wrong: {exp.Message}";
            }

            return null;
        }
#endif

        public async Task<KeyBundle> LoadSigningKeyAsync(string signingCertificateKey)
        {
            return await _keyVaultClient.GetKeyAsync(signingCertificateKey);
        }

        public async Task<X509Certificate2> LoadSigningCertificateAsync(string signingCertificateKey, X509Certificate2 publicCert)
        {
            var secret = await _keyVaultClient.GetSecretAsync(signingCertificateKey);
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

        public async Task SignDigestAsync(string signingKey, byte [] digest)
        {
            var result = await _keyVaultClient.SignAsync(signingKey, JsonWebKeySignatureAlgorithm.RS256, digest);
        }

        public async Task UploadCACertificate(string name, X509Certificate2 certificate)
        {
            CertificateAttributes attributes = new CertificateAttributes {
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

            var result = await _keyVaultClient.ImportCertificateAsync(
                _vaultBaseUrl,
                name,
                new X509Certificate2Collection(certificate),
                policy,
                attributes);
        }

        public async Task UploadCACrl(string name, X509Certificate2 certificate, Opc.Ua.X509CRL crl)
        {
            try
            {
                string secretIdentifier = CrlSecretName(name, certificate);
                SecretAttributes secretAttributes = new SecretAttributes()
                {
                    Enabled = true,
                    Expires = crl.NextUpdateTime,
                    NotBefore = crl.UpdateTime
                };

                var result = await _keyVaultClient.SetSecretAsync(_vaultBaseUrl, secretIdentifier, Convert.ToBase64String(crl.RawData), null, ContentTypeCrl, secretAttributes);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<Opc.Ua.X509CRL> LoadCACrl(string name, X509Certificate2 certificate)
        {
            string secretIdentifier = CrlSecretName(name, certificate);
            var secret = await _keyVaultClient.GetSecretAsync(_vaultBaseUrl, secretIdentifier);
            if (secret.ContentType == ContentTypeCrl)
            {
                var crlBlob = Convert.FromBase64String(secret.Value);
                return new Opc.Ua.X509CRL(crlBlob);
            }
            return null;
        }

        private string CrlSecretName(string name, X509Certificate2 certificate)
        {
            return name + "Crl" + certificate.Thumbprint;
        }
    }
}

