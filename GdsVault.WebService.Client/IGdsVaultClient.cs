// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.Client
{
    using Microsoft.Azure.IoTSolutions.GdsVault.WebService.Client.Models;
    using System.Threading.Tasks;

    public delegate Task<string> AuthenticationCallback(string authority, string resource, string scope);

    /// <summary>
    /// Represents OPC twin service api functions
    /// </summary>
    public interface IGdsVaultClient {


        /// <summary>
        /// Returns status of the service
        /// </summary>
        Task<StatusResponseApiModel> GetServiceStatusAsync();

        Task<KeyVaultSecretApiModel> GetIotHubSecretAsync();

        Task<CertificateGroupConfigurationCollectionApiModel> GetCertificateGroupConfiguration();

        Task<X509Certificate2CollectionApiModel> GetCACertificateChainAsync(string id);

        Task<X509CrlCollectionApiModel> GetCACrlChainAsync(string id);

        Task<TrustListApiModel> GetTrustListAsync(string id);

        Task<X509CrlApiModel> RevokeCertificateAsync(string id, X509Certificate2ApiModel model);

        Task<X509Certificate2ApiModel> CreateCACertificateAsync(string id);

        Task<X509Certificate2ApiModel> SigningRequestAsync(string id, SigningRequestApiModel model);

        Task<CertificateKeyPairApiModel> NewKeyPairRequestAsync(string id, NewKeyPairRequestApiModel model);

    }
}