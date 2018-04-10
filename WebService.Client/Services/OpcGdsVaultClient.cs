// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.Client
{
    using Microsoft.Azure.IoTSolutions.Common.Diagnostics;
    using Microsoft.Azure.IoTSolutions.Common.Http;
    using Microsoft.Azure.IoTSolutions.Common.Utils;
    using Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.Client.Models;
    using Newtonsoft.Json;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Implementation of v1 service adapter.
    /// </summary>
    public class OpcGdsVaultClient : IOpcGdsVaultClient
    {

        /// <summary>
        /// Create service client
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public OpcGdsVaultClient(
            IHttpClient httpClient,
            IOpcGdsVaultConfig config,
            ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serviceUri = config.OpcGdsVaultServiceApiUrl;

            if (string.IsNullOrEmpty(_serviceUri))
            {
                _serviceUri = "http://localhost:58801/v1";
                _logger.Error(
                    "No gds vaultservice Uri specified.Using default " +
                    _serviceUri + ". If this is not your intention, or to " +
                    "remove this error, please configure the Url " +
                    "in the appsettings.json file.",
                    () => { });
            }
        }


        public OpcGdsVaultClient(
            IOpcGdsVaultConfig config,
            AuthenticationCallback authenticationCallback)
        {
            ILogger logger = new Logger("processid", LogLevel.Error);
            _logger = logger;
            _httpClient = new HttpClient(logger);
            _serviceUri = config.OpcGdsVaultServiceApiUrl;
            if (string.IsNullOrEmpty(_serviceUri))
            {
                _serviceUri = "http://localhost:58801/v1";
                _logger.Error(
                    "No gds vaultservice Uri specified.Using default " +
                    _serviceUri + ". If this is not your intention, or to " +
                    "remove this error, please configure the Url " +
                    "in the appsettings.json file.",
                    () => { });
            }
        }

        /// <summary>
        /// Returns service status
        /// </summary>
        /// <returns></returns>
        public async Task<StatusResponseApiModel> GetServiceStatusAsync()
        {
            var request = NewRequest($"{_serviceUri}/status");
            var response = await _httpClient.GetAsync(request);
            response.Validate();
            return JsonConvertEx.DeserializeObject<StatusResponseApiModel>(response.Content);
        }


        public async Task<KeyVaultSecretApiModel> GetIotHubSecretAsync()
        {
            var request = NewRequest($"{_serviceUri}/groups/iothub");
            var response = await _httpClient.GetAsync(request);
            response.Validate();
            return JsonConvertEx.DeserializeObject<KeyVaultSecretApiModel>(response.Content);
        }

        public async Task<CertificateGroupConfigurationCollectionApiModel> GetCertificateGroupConfiguration()
        {
            var request = NewRequest($"{_serviceUri}/groups/config");
            var response = await _httpClient.GetAsync(request);
            response.Validate();
            return JsonConvertEx.DeserializeObject<CertificateGroupConfigurationCollectionApiModel>(response.Content);
        }

        public async Task<X509Certificate2CollectionApiModel> GetCACertificateChainAsync(string id)
        {
            var request = NewRequest($"{_serviceUri}/groups/{id}/cacert");
            var response = await _httpClient.GetAsync(request);
            response.Validate();
            return JsonConvertEx.DeserializeObject<X509Certificate2CollectionApiModel>(response.Content);
        }

        public async Task<X509CrlCollectionApiModel> GetCACrlChainAsync(string id)
        {
            var request = NewRequest($"{_serviceUri}/groups/{id}/cacrl");
            var response = await _httpClient.GetAsync(request);
            response.Validate();
            return JsonConvertEx.DeserializeObject<X509CrlCollectionApiModel>(response.Content);
        }

        public Task<X509CrlApiModel> RevokeCertificateAsync(string id, X509Certificate2ApiModel model)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (string.IsNullOrEmpty(model.Certificate))
            {
                throw new ArgumentException(nameof(model.Certificate));
            }
            if (string.IsNullOrEmpty(model.Subject))
            {
                throw new ArgumentException(nameof(model.Subject));
            }
            if (string.IsNullOrEmpty(model.Thumbprint))
            {
                throw new ArgumentException(nameof(model.Thumbprint));
            }

            return Retry.WithExponentialBackoff(_logger, async () =>
            {
                var request = NewRequest($"{_serviceUri}/groups/{id}/revoke");
                request.SetContent(model);
                var response = await _httpClient.PostAsync(request);
                response.Validate();
                return JsonConvertEx.DeserializeObject<X509CrlApiModel>(response.Content);
            });
        }

        public async Task<X509Certificate2ApiModel> CreateCACertificateAsync(string id)
        {
            var request = NewRequest($"{_serviceUri}/groups/{id}/create/");
            var response = await _httpClient.GetAsync(request);
            response.Validate();
            return JsonConvertEx.DeserializeObject<X509Certificate2ApiModel>(response.Content);
        }

        public Task<X509Certificate2ApiModel> SigningRequestAsync(string id, SigningRequestApiModel model)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (string.IsNullOrEmpty(model.ApplicationURI))
            {
                throw new ArgumentException(nameof(model.ApplicationURI));
            }
            if (string.IsNullOrEmpty(model.Csr))
            {
                throw new ArgumentException(nameof(model.Csr));
            }
            return Retry.WithExponentialBackoff(_logger, async () =>
            {
                var request = NewRequest($"{_serviceUri}/groups/{id}/sign");
                request.SetContent(model);
                var response = await _httpClient.PostAsync(request);
                response.Validate();
                return JsonConvertEx.DeserializeObject<X509Certificate2ApiModel>(response.Content);
            });
        }

        public Task<CertificateKeyPairApiModel> NewKeyPairRequestAsync(string id, NewKeyPairRequestApiModel model)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (string.IsNullOrEmpty(model.ApplicationURI))
            {
                throw new ArgumentException(nameof(model.ApplicationURI));
            }
            if (model.DomainNames == null || string.IsNullOrEmpty(model.DomainNames[0]))
            {
                throw new ArgumentException(nameof(model.DomainNames));
            }
            if (string.IsNullOrEmpty(model.PrivateKeyFormat))
            {
                throw new ArgumentException(nameof(model.PrivateKeyFormat));
            }
            if (string.IsNullOrEmpty(model.SubjectName))
            {
                throw new ArgumentException(nameof(model.SubjectName));
            }

            return Retry.WithExponentialBackoff(_logger, async () =>
            {
                var request = NewRequest($"{_serviceUri}/groups/{id}/newkey");
                request.SetContent(model);
                var response = await _httpClient.PostAsync(request);
                response.Validate();
                return JsonConvertEx.DeserializeObject<CertificateKeyPairApiModel>(response.Content);
            });
        }

        /// <summary>
        /// Helper to create new request
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static HttpRequest NewRequest(string uri)
        {
            var request = new HttpRequest();
            request.SetUriFromString(uri);
            if (uri.ToLowerInvariant().StartsWith("https:",
                StringComparison.Ordinal))
            {
                request.Options.AllowInsecureSSLServer = true;
            }
            return request;
        }

        private const string CONTINUATION_TOKEN_NAME = "x-ms-continuation";
        private readonly IHttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _serviceUri;
    }

}
