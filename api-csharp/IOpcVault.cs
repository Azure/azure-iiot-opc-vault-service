// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.IIoT.OpcUa.Api.Vault
{
    using Microsoft.Rest;
    using Models;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// OPC UA Vault Service
    /// </summary>
    public partial interface IOpcVault : System.IDisposable
    {
        /// <summary>
        /// The base URI of the service.
        /// </summary>
        System.Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        JsonSerializerSettings SerializationSettings { get; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        JsonSerializerSettings DeserializationSettings { get; }

        /// <summary>
        /// Subscription credentials which uniquely identify client
        /// subscription.
        /// </summary>
        ServiceClientCredentials Credentials { get; }


        /// <summary>
        /// Register new application.
        /// </summary>
        /// <param name='application'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<ApplicationRecordApiModel>> RegisterApplicationWithHttpMessagesAsync(ApplicationRecordApiModel application = default(ApplicationRecordApiModel), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get application.
        /// </summary>
        /// <param name='applicationId'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<ApplicationRecordApiModel>> GetApplicationWithHttpMessagesAsync(string applicationId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Update application.
        /// </summary>
        /// <param name='applicationId'>
        /// </param>
        /// <param name='application'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<ApplicationRecordApiModel>> UpdateApplicationWithHttpMessagesAsync(string applicationId, ApplicationRecordApiModel application = default(ApplicationRecordApiModel), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete application.
        /// </summary>
        /// <param name='applicationId'>
        /// </param>
        /// <param name='force'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse> DeleteApplicationWithHttpMessagesAsync(string applicationId, bool? force = default(bool?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Approve or reject new application.
        /// </summary>
        /// <param name='applicationId'>
        /// </param>
        /// <param name='approved'>
        /// </param>
        /// <param name='force'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<ApplicationRecordApiModel>> ApproveApplicationWithHttpMessagesAsync(string applicationId, bool approved, bool? force = default(bool?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Unregister application.
        /// </summary>
        /// <param name='applicationId'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<ApplicationRecordApiModel>> UnregisterApplicationWithHttpMessagesAsync(string applicationId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Find applications
        /// </summary>
        /// <param name='uri'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<IList<ApplicationRecordApiModel>>> ListApplicationsWithHttpMessagesAsync(string uri, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Query applications
        /// </summary>
        /// <param name='query'>
        /// </param>
        /// <param name='anyState'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<QueryApplicationsResponseApiModel>> QueryApplicationsWithHttpMessagesAsync(QueryApplicationsApiModel query = default(QueryApplicationsApiModel), bool? anyState = default(bool?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Query applications
        /// </summary>
        /// <param name='query'>
        /// </param>
        /// <param name='anyState'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<QueryApplicationsPageResponseApiModel>> QueryApplicationsPageWithHttpMessagesAsync(QueryApplicationsPageApiModel query = default(QueryApplicationsPageApiModel), bool? anyState = default(bool?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<CertificateGroupListApiModel>> GetCertificateGroupsWithHttpMessagesAsync(Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get group configuration
        /// </summary>
        /// <param name='group'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<CertificateGroupConfigurationApiModel>> GetCertificateGroupConfigurationWithHttpMessagesAsync(string group, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Update group configuration
        /// </summary>
        /// <param name='group'>
        /// </param>
        /// <param name='config'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<CertificateGroupConfigurationApiModel>> UpdateCertificateGroupConfigurationWithHttpMessagesAsync(string group, CertificateGroupConfigurationApiModel config = default(CertificateGroupConfigurationApiModel), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete group configuration
        /// </summary>
        /// <param name='group'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse> DeleteCertificateGroupWithHttpMessagesAsync(string group, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Create new group configuration
        /// </summary>
        /// <param name='group'>
        /// </param>
        /// <param name='subject'>
        /// </param>
        /// <param name='certType'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<CertificateGroupConfigurationApiModel>> CreateCertificateGroupWithHttpMessagesAsync(string group, string subject, string certType, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get group configuration
        /// </summary>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<CertificateGroupConfigurationCollectionApiModel>> GetCertificateGroupsConfigurationWithHttpMessagesAsync(Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get CA Certificate chain
        /// </summary>
        /// <param name='group'>
        /// </param>
        /// <param name='maxResults'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<X509Certificate2CollectionApiModel>> GetCertificateGroupCAChainWithHttpMessagesAsync(string group, int? maxResults = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get CA Certificate chain
        /// </summary>
        /// <param name='group'>
        /// </param>
        /// <param name='nextPageLink'>
        /// </param>
        /// <param name='maxResults'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<X509Certificate2CollectionApiModel>> GetCertificateGroupCAChainNextWithHttpMessagesAsync(string group, string nextPageLink = default(string), int? maxResults = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get CA CRL chain
        /// </summary>
        /// <param name='group'>
        /// </param>
        /// <param name='maxResults'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<X509CrlCollectionApiModel>> GetCertificateGroupCACrlChainWithHttpMessagesAsync(string group, int? maxResults = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get CA CRL chain
        /// </summary>
        /// <param name='group'>
        /// </param>
        /// <param name='nextPageLink'>
        /// </param>
        /// <param name='maxResults'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<X509CrlCollectionApiModel>> GetCertificateGroupCACrlChainNextWithHttpMessagesAsync(string group, string nextPageLink = default(string), int? maxResults = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get trust list
        /// </summary>
        /// <param name='group'>
        /// </param>
        /// <param name='maxResults'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<TrustListApiModel>> GetCertificateGroupTrustListWithHttpMessagesAsync(string group, int? maxResults = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get trust list
        /// </summary>
        /// <param name='group'>
        /// </param>
        /// <param name='nextPageLink'>
        /// </param>
        /// <param name='maxResults'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<TrustListApiModel>> GetCertificateGroupTrustListNextWithHttpMessagesAsync(string group, string nextPageLink = default(string), int? maxResults = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Create new CA Certificate
        /// </summary>
        /// <param name='group'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<X509Certificate2ApiModel>> CreateCertificateGroupCACertWithHttpMessagesAsync(string group, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Start a new signing request.
        /// </summary>
        /// <param name='signingRequest'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<string>> CreateSigningRequestWithHttpMessagesAsync(CreateSigningRequestApiModel signingRequest = default(CreateSigningRequestApiModel), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Start a new key pair request.
        /// </summary>
        /// <param name='newKeyPairRequest'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<string>> CreateNewKeyPairRequestWithHttpMessagesAsync(CreateNewKeyPairRequestApiModel newKeyPairRequest = default(CreateNewKeyPairRequestApiModel), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Approve request.
        /// </summary>
        /// <param name='requestId'>
        /// </param>
        /// <param name='rejected'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse> ApproveCertificateRequestWithHttpMessagesAsync(string requestId, bool rejected, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Accept request.
        /// </summary>
        /// <param name='requestId'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse> AcceptCertificateRequestWithHttpMessagesAsync(string requestId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Read certificate request
        /// </summary>
        /// <param name='requestId'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<CertificateRequestRecordApiModel>> GetCertificateRequestWithHttpMessagesAsync(string requestId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete request.
        /// </summary>
        /// <param name='requestId'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse> DeleteCertificateRequestWithHttpMessagesAsync(string requestId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Purge request.
        /// </summary>
        /// <param name='requestId'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse> PurgeCertificateRequestWithHttpMessagesAsync(string requestId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Revoke request.
        /// </summary>
        /// <param name='requestId'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse> RevokeCertificateRequestWithHttpMessagesAsync(string requestId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Revoke all deleted requests.
        /// </summary>
        /// <param name='group'>
        /// </param>
        /// <param name='allVersions'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse> RevokeGroupWithHttpMessagesAsync(string group, bool? allVersions = default(bool?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Query certificate requests
        /// </summary>
        /// <param name='appId'>
        /// </param>
        /// <param name='requestState'>
        /// </param>
        /// <param name='maxResults'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<CertificateRequestRecordQueryResponseApiModel>> QueryCertificateRequestsWithHttpMessagesAsync(string appId = default(string), string requestState = default(string), int? maxResults = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Query certificate requests
        /// </summary>
        /// <param name='nextPageLink'>
        /// </param>
        /// <param name='appId'>
        /// </param>
        /// <param name='requestState'>
        /// </param>
        /// <param name='maxResults'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<CertificateRequestRecordQueryResponseApiModel>> QueryCertificateRequestsNextWithHttpMessagesAsync(string nextPageLink = default(string), string appId = default(string), string requestState = default(string), int? maxResults = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Fetch certificate request results
        /// </summary>
        /// <param name='requestId'>
        /// </param>
        /// <param name='applicationId'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<FetchRequestResultApiModel>> FetchCertificateRequestResultWithHttpMessagesAsync(string requestId, string applicationId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// returns the status
        /// </summary>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<StatusApiModel>> GetStatusWithHttpMessagesAsync(Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

    }
}
