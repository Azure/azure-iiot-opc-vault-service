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
        /// Get Issuer CA Certificate chain
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
        Task<HttpOperationResponse<X509Certificate2CollectionApiModel>> GetCertificateGroupIssuerCAChainWithHttpMessagesAsync(string group, int? maxResults = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get Issuer CA Certificate chain
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
        Task<HttpOperationResponse<X509Certificate2CollectionApiModel>> GetCertificateGroupIssuerCAChainNextWithHttpMessagesAsync(string group, string nextPageLink = default(string), int? maxResults = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get Issuer CA CRL chain
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
        Task<HttpOperationResponse<X509CrlCollectionApiModel>> GetCertificateGroupIssuerCACrlChainWithHttpMessagesAsync(string group, int? maxResults = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get Issuer CA CRL chain
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
        Task<HttpOperationResponse<X509CrlCollectionApiModel>> GetCertificateGroupIssuerCACrlChainNextWithHttpMessagesAsync(string group, string nextPageLink = default(string), int? maxResults = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<HttpOperationResponse<X509Certificate2ApiModel>> CreateCertificateGroupIssuerCACertWithHttpMessagesAsync(string group, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Create a certificate request with a certificate signing request
        /// (CSR).
        /// </summary>
        /// <remarks>
        /// The request is in the 'New' state after this call.
        /// Requires Writer or Manager role.
        /// </remarks>
        /// <param name='signingRequest'>
        /// The signing request parameters
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<string>> CreateSigningRequestWithHttpMessagesAsync(CreateSigningRequestApiModel signingRequest = default(CreateSigningRequestApiModel), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Create a a certificate request with a new key pair.
        /// </summary>
        /// <remarks>
        /// The request is in the 'New' state after this call.
        /// Requires Writer or Manager role.
        /// </remarks>
        /// <param name='newKeyPairRequest'>
        /// The new key pair request parameters
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<string>> CreateNewKeyPairRequestWithHttpMessagesAsync(CreateNewKeyPairRequestApiModel newKeyPairRequest = default(CreateNewKeyPairRequestApiModel), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Approve the certificate request.
        /// </summary>
        /// <remarks>
        /// Validates the request with the application database.
        /// If Approved:
        /// New Key Pair request: Creates the new key pair
        /// in the requested format, signs the certificate and stores the
        /// private key for later securely in KeyVault.
        /// Cert Signing Request: Creates and signs the certificate.
        /// Deletes the CSR from the database.
        /// Stores the signed certificate for later use in the Database.
        /// The request is in the 'Approved' or 'Rejected' state after this
        /// call.
        /// Requires Approver role.
        /// Approver needs signing rights in KeyVault.
        /// </remarks>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        /// <param name='rejected'>
        /// if the request is rejected(true) or approved(false)
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse> ApproveCertificateRequestWithHttpMessagesAsync(string requestId, bool rejected, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Accept request and delete the private key.
        /// </summary>
        /// <remarks>
        /// By accepting the request the requester takes ownership of the
        /// certificate
        /// and the private key, if requested. A private key with metadata is
        /// deleted from KeyVault.
        /// The public certificate remains in the database for sharing public
        /// key information
        /// or for later revocation once the application is deleted.
        /// The request is in the 'Accepted' state after this call.
        /// Requires Writer role.
        /// </remarks>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse> AcceptCertificateRequestWithHttpMessagesAsync(string requestId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get a specific certificate request.
        /// </summary>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<CertificateRequestRecordApiModel>> GetCertificateRequestWithHttpMessagesAsync(string requestId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete request. Mark the certificate for revocation.
        /// </summary>
        /// <remarks>
        /// The request must be in the 'Approved' or 'Accepted' state.
        /// By deleting the request it is actually not physically deleted, but
        /// marked
        /// for revocation. The public certificate is still available for
        /// revocation.
        /// The request is in the 'Deleted' state after this call.
        /// Requires Manager role.
        /// </remarks>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse> DeleteCertificateRequestWithHttpMessagesAsync(string requestId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Purge request. Physically delete the request.
        /// </summary>
        /// <remarks>
        /// The request must be in the 'Revoked','Rejected' or 'New' state.
        /// By purging the request it is actually physically deleted from the
        /// database, including the public key and other information.
        /// The request is purged after this call.
        /// Requires Manager role.
        /// </remarks>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse> PurgeCertificateRequestWithHttpMessagesAsync(string requestId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Revoke request. Create New CRL version with revoked certificate.
        /// </summary>
        /// <remarks>
        /// The request must be in the 'Deleted' state for revocation.
        /// The certificate issuer CA and CRL are looked up, the certificate
        /// serial number is added and a new CRL version is issued and updated
        /// in the certificate group storage.
        /// Preferably deleted certificates are revoked with the RevokeGroup
        /// call to batch multiple revoked certificates in a single CRL update.
        /// Requires Approver role.
        /// Approver needs signing rights in KeyVault.
        /// </remarks>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse> RevokeCertificateRequestWithHttpMessagesAsync(string requestId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Revoke all deleted certificate requests for a group.
        /// </summary>
        /// <remarks>
        /// Select all requests for a group in the 'Deleted' state are marked
        /// for revocation.
        /// The certificate issuer CA and CRL are looked up, all the
        /// certificate
        /// serial numbers are added and a new CRL version is issued and
        /// updated
        /// in the certificate group storage.
        /// Requires Approver role.
        /// Approver needs signing rights in KeyVault.
        /// </remarks>
        /// <param name='group'>
        /// The certificate group id
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
        /// Query for requests.
        /// </summary>
        /// <param name='appId'>
        /// optional, query for application id
        /// </param>
        /// <param name='requestState'>
        /// optional, query for request state. Possible values include: 'New',
        /// 'Approved', 'Rejected', 'Accepted', 'Deleted', 'Revoked'
        /// </param>
        /// <param name='maxResults'>
        /// optional, the maximum number of result per page
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<CertificateRequestRecordQueryResponseApiModel>> QueryCertificateRequestsWithHttpMessagesAsync(string appId = default(string), string requestState = default(string), int? maxResults = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Query for next page of requests.
        /// </summary>
        /// <param name='nextPageLink'>
        /// next page link from previous query
        /// </param>
        /// <param name='appId'>
        /// optional, query for application id
        /// </param>
        /// <param name='requestState'>
        /// optional, query for request state
        /// </param>
        /// <param name='maxResults'>
        /// optional, the maximum number of result per page
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<CertificateRequestRecordQueryResponseApiModel>> QueryCertificateRequestsNextWithHttpMessagesAsync(string nextPageLink = default(string), string appId = default(string), string requestState = default(string), int? maxResults = default(int?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Fetch certificate request approval result.
        /// </summary>
        /// <remarks>
        /// Can be called in any state.
        /// Returns only state information in 'New', 'Rejected', 'Deleted' and
        /// 'Revoked' state.
        /// Fetches private key in 'Approved' state, if requested.
        /// Fetches the public certificate in 'Approved' and 'Accepted' state.
        /// After a successful fetch in 'Approved' state, the request should be
        /// accepted to delete the private key.
        /// Requires Writer role.
        /// </remarks>
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
