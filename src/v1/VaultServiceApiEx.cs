// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Api.Vault.v1 {
    using Microsoft.Azure.IIoT.OpcUa.Api.Vault.v1.Models;
    using Microsoft.Rest;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable RECS0154 // Parameter is never used

    /// <summary>
    /// Extension methods for vault client to adapt to v1.
    /// </summary>
    public static class VaultServiceApiEx {

        /// <summary>
        /// Create client from service credentials
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="serviceClientCredentials"></param>
        /// <returns></returns>
        public static IVaultServiceApi CreateClient(Uri uri,
            ServiceClientCredentials serviceClientCredentials) {
            if (uri == null) {
                throw new ArgumentNullException(nameof(uri));
            }
            if (serviceClientCredentials == null) {
                throw new ArgumentNullException(nameof(serviceClientCredentials));
            }

            // TODO

            throw new NotImplementedException();
        }

        /// <summary>
        /// Register new application.
        /// </summary>
        /// <remarks>
        /// After registration an application is in the 'New' state and needs
        /// approval by a manager to be avavilable for certificate operation.
        /// Requires Writer role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='application'>
        /// The new application
        /// </param>
        public static ApplicationRecordApiModel RegisterApplication(this IVaultServiceApi client,
            ApplicationRecordApiModel application = default) {
            return client.RegisterApplicationAsync(application)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Register new application.
        /// </summary>
        /// <remarks>
        /// After registration an application is in the 'New' state and needs
        /// approval by a manager to be avavilable for certificate operation.
        /// Requires Writer role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='application'>
        /// The new application
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<ApplicationRecordApiModel> RegisterApplicationAsync(
            this IVaultServiceApi client, ApplicationRecordApiModel application = default,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Get application.
        /// </summary>
        /// <remarks>
        /// Returns the record of any application.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='applicationId'>
        /// The application id
        /// </param>
        public static ApplicationRecordApiModel GetApplication(this IVaultServiceApi client,
            string applicationId) {
            return client.GetApplicationAsync(applicationId)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get application.
        /// </summary>
        /// <remarks>
        /// Returns the record of any application.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='applicationId'>
        /// The application id
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<ApplicationRecordApiModel> GetApplicationAsync(
            this IVaultServiceApi client, string applicationId,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Update application.
        /// </summary>
        /// <remarks>
        /// Update the application with given application id, however state information
        /// is unchanged.
        /// Requires Writer role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='applicationId'>
        /// </param>
        /// <param name='application'>
        /// The updated application
        /// </param>
        public static ApplicationRecordApiModel UpdateApplication(this IVaultServiceApi client,
            string applicationId, ApplicationRecordApiModel application = default) {
            return client.UpdateApplicationAsync(applicationId, application)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Update application.
        /// </summary>
        /// <remarks>
        /// Update the application with given application id, however state information
        /// is unchanged.
        /// Requires Writer role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='applicationId'>
        /// </param>
        /// <param name='application'>
        /// The updated application
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<ApplicationRecordApiModel> UpdateApplicationAsync(
            this IVaultServiceApi client, string applicationId,
            ApplicationRecordApiModel application = default,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Delete application.
        /// </summary>
        /// <remarks>
        /// Deletes the application record.
        /// Certificate Requests associated with the application id are set in the
        /// deleted state,
        /// and will be revoked with the next CRL update.
        /// Requires Manager role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='applicationId'>
        /// The application id
        /// </param>
        /// <param name='force'>
        /// optional, skip sanity checks and force to delete application
        /// </param>
        public static void DeleteApplication(this IVaultServiceApi client,
            string applicationId, bool? force = default) {
            client.DeleteApplicationAsync(applicationId, force)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Delete application.
        /// </summary>
        /// <remarks>
        /// Deletes the application record.
        /// Certificate Requests associated with the application id are set in the
        /// deleted state,
        /// and will be revoked with the next CRL update.
        /// Requires Manager role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='applicationId'>
        /// The application id
        /// </param>
        /// <param name='force'>
        /// optional, skip sanity checks and force to delete application
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task DeleteApplicationAsync(this IVaultServiceApi client,
            string applicationId, bool? force = default,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
        }

        /// <summary>
        /// Approve or reject a new application.
        /// </summary>
        /// <remarks>
        /// A manager can approve a new application or force an application from any
        /// state.
        /// After approval the application is in the 'Approved' or 'Rejected' state.
        /// Requires Manager role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='applicationId'>
        /// The application id
        /// </param>
        /// <param name='approved'>
        /// approve or reject the new application
        /// </param>
        /// <param name='force'>
        /// optional, force application in new state
        /// </param>
        public static ApplicationRecordApiModel ApproveApplication(this IVaultServiceApi client,
            string applicationId, bool approved, bool? force = default) {
            return client.ApproveApplicationAsync(applicationId, approved, force)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Approve or reject a new application.
        /// </summary>
        /// <remarks>
        /// A manager can approve a new application or force an application from any
        /// state.
        /// After approval the application is in the 'Approved' or 'Rejected' state.
        /// Requires Manager role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='applicationId'>
        /// The application id
        /// </param>
        /// <param name='approved'>
        /// approve or reject the new application
        /// </param>
        /// <param name='force'>
        /// optional, force application in new state
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<ApplicationRecordApiModel> ApproveApplicationAsync(
            this IVaultServiceApi client, string applicationId, bool approved,
            bool? force = default, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Unregister application.
        /// </summary>
        /// <remarks>
        /// Unregisters the application record and all associated information.
        /// The application record remains in the database in 'Unregistered' state.
        /// Certificate Requests associated with the application id are set to the
        /// 'Deleted' state,
        /// and will be revoked with the next CRL update.
        /// Requires Writer role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='applicationId'>
        /// The application id
        /// </param>
        public static void UnregisterApplication(this IVaultServiceApi client, string applicationId) {
            client.UnregisterApplicationAsync(applicationId)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Unregister application.
        /// </summary>
        /// <remarks>
        /// Unregisters the application record and all associated information.
        /// The application record remains in the database in 'Unregistered' state.
        /// Certificate Requests associated with the application id are set to the
        /// 'Deleted' state,
        /// and will be revoked with the next CRL update.
        /// Requires Writer role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='applicationId'>
        /// The application id
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task UnregisterApplicationAsync(this IVaultServiceApi client,
            string applicationId, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
        }

        /// <summary>
        /// List applications with matching application Uri.
        /// </summary>
        /// <remarks>
        /// List approved applications that match the application Uri.
        /// Application Uris may have duplicates in the application database.
        /// The returned model can contain a next page link if more results are
        /// available.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='applicationUri'>
        /// The application Uri
        /// </param>
        /// <param name='nextPageLink'>
        /// optional, link to next page
        /// </param>
        /// <param name='pageSize'>
        /// optional, the maximum number of result per page
        /// </param>
        public static QueryApplicationsResponseApiModel ListApplications(
            this IVaultServiceApi client, string applicationUri, string nextPageLink = default,
            int? pageSize = default) {
            return client.ListApplicationsAsync(applicationUri, nextPageLink, pageSize)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// List applications with matching application Uri.
        /// </summary>
        /// <remarks>
        /// List approved applications that match the application Uri.
        /// Application Uris may have duplicates in the application database.
        /// The returned model can contain a next page link if more results are
        /// available.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='applicationUri'>
        /// The application Uri
        /// </param>
        /// <param name='nextPageLink'>
        /// optional, link to next page
        /// </param>
        /// <param name='pageSize'>
        /// optional, the maximum number of result per page
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<QueryApplicationsResponseApiModel> ListApplicationsAsync(
            this IVaultServiceApi client, string applicationUri, string nextPageLink = default,
            int? pageSize = default, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Query applications by id.
        /// </summary>
        /// <remarks>
        /// A query model which supports the OPC UA Global Discovery Server query.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='query'>
        /// </param>
        public static QueryApplicationsByIdResponseApiModel QueryApplicationsById(
            this IVaultServiceApi client, QueryApplicationsByIdApiModel query = default) {
            return client.QueryApplicationsByIdAsync(query)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Query applications by id.
        /// </summary>
        /// <remarks>
        /// A query model which supports the OPC UA Global Discovery Server query.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='query'>
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<QueryApplicationsByIdResponseApiModel> QueryApplicationsByIdAsync(
            this IVaultServiceApi client, QueryApplicationsByIdApiModel query = default,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Query applications.
        /// </summary>
        /// <remarks>
        /// List applications that match the query model.
        /// The returned model can contain a next page link if more results are
        /// available.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='query'>
        /// The Application query parameters
        /// </param>
        /// <param name='nextPageLink'>
        /// optional, link to next page
        /// </param>
        /// <param name='pageSize'>
        /// optional, the maximum number of result per page
        /// </param>
        public static QueryApplicationsResponseApiModel QueryApplications(this IVaultServiceApi client,
            QueryApplicationsApiModel query = default, string nextPageLink = default,
            int? pageSize = default) {
            return client.QueryApplicationsAsync(query, nextPageLink, pageSize)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Query applications.
        /// </summary>
        /// <remarks>
        /// List applications that match the query model.
        /// The returned model can contain a next page link if more results are
        /// available.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='query'>
        /// The Application query parameters
        /// </param>
        /// <param name='nextPageLink'>
        /// optional, link to next page
        /// </param>
        /// <param name='pageSize'>
        /// optional, the maximum number of result per page
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<QueryApplicationsResponseApiModel> QueryApplicationsAsync(
            this IVaultServiceApi client, QueryApplicationsApiModel query = default,
            string nextPageLink = default, int? pageSize = default,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Get Issuer Certificate for Authority Information Access endpoint.
        /// </summary>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='serial'>
        /// </param>
        /// <param name='cert'>
        /// </param>
        public static void GetIssuerCert(this IVaultServiceApi client, string serial, string cert) {
            client.GetIssuerCertAsync(serial, cert).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get Issuer Certificate for Authority Information Access endpoint.
        /// </summary>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='serial'>
        /// </param>
        /// <param name='cert'>
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task GetIssuerCertAsync(this IVaultServiceApi client, string serial,
            string cert, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
        }

        /// <summary>
        /// Get Issuer CRL in CRL Distribution Endpoint.
        /// </summary>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='serial'>
        /// </param>
        /// <param name='crl'>
        /// </param>
        public static void GetIssuerCrl(this IVaultServiceApi client, string serial, string crl) {
            client.GetIssuerCrlAsync(serial, crl).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get Issuer CRL in CRL Distribution Endpoint.
        /// </summary>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='serial'>
        /// </param>
        /// <param name='crl'>
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task GetIssuerCrlAsync(this IVaultServiceApi client, string serial,
            string crl, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
        }

        /// <summary>
        /// Get Certificate Group Names.
        /// </summary>
        /// <remarks>
        /// Returns a list of supported group names. The names are typically used as
        /// parameter.
        /// The Default group name is 'Default'.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        public static CertificateGroupListApiModel GetCertificateGroups(
            this IVaultServiceApi client) {
            return client.GetCertificateGroupsAsync()
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get Certificate Group Names.
        /// </summary>
        /// <remarks>
        /// Returns a list of supported group names. The names are typically used as
        /// parameter.
        /// The Default group name is 'Default'.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<CertificateGroupListApiModel> GetCertificateGroupsAsync(
            this IVaultServiceApi client, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Get group configuration.
        /// </summary>
        /// <remarks>
        /// The group configuration for a group is stored in KeyVault and contains
        /// information
        /// about the CA subject, the lifetime and the security algorithms used.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// The group name
        /// </param>
        public static CertificateGroupConfigurationApiModel GetCertificateGroupConfiguration(
            this IVaultServiceApi client, string group) {
            return client.GetCertificateGroupConfigurationAsync(group).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get group configuration.
        /// </summary>
        /// <remarks>
        /// The group configuration for a group is stored in KeyVault and contains
        /// information
        /// about the CA subject, the lifetime and the security algorithms used.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// The group name
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<CertificateGroupConfigurationApiModel> GetCertificateGroupConfigurationAsync(
            this IVaultServiceApi client, string group, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Update group configuration.
        /// </summary>
        /// <remarks>
        /// Updates the configuration for a certificate group.
        /// Use this function with care and only if you are aware of the security
        /// implications.
        /// - A change of the subject requires to issue a new CA certificate.
        /// - A change of the lifetime and security parameter of the issuer certificate
        /// takes
        /// effect on the next Issuer CA key generation.
        /// - A change in lifetime for issued certificates takes effect on the next
        /// request approval.
        /// In general, security parameters should not be changed after a security
        /// group is established.
        /// Instead, a new certificate group with new parameters should be created for
        /// all subsequent
        /// client.
        /// Requires manager role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// The group name
        /// </param>
        /// <param name='config'>
        /// The group configuration
        /// </param>
        public static CertificateGroupConfigurationApiModel UpdateCertificateGroupConfiguration(
            this IVaultServiceApi client, string group, CertificateGroupConfigurationApiModel config = default) {
            return client.UpdateCertificateGroupConfigurationAsync(group, config)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Update group configuration.
        /// </summary>
        /// <remarks>
        /// Updates the configuration for a certificate group.
        /// Use this function with care and only if you are aware of the security
        /// implications.
        /// - A change of the subject requires to issue a new CA certificate.
        /// - A change of the lifetime and security parameter of the issuer certificate
        /// takes
        /// effect on the next Issuer CA key generation.
        /// - A change in lifetime for issued certificates takes effect on the next
        /// request approval.
        /// In general, security parameters should not be changed after a security
        /// group is established.
        /// Instead, a new certificate group with new parameters should be created for
        /// all subsequent
        /// client.
        /// Requires manager role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// The group name
        /// </param>
        /// <param name='config'>
        /// The group configuration
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<CertificateGroupConfigurationApiModel> UpdateCertificateGroupConfigurationAsync(
            this IVaultServiceApi client, string group, CertificateGroupConfigurationApiModel config = default,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Delete a group configuration.
        /// </summary>
        /// <remarks>
        /// Deletes a group with configuration.
        /// After this operation the Issuer CA, CRLs and keys become inaccessible.
        /// Use this function with extreme caution.
        /// Requires manager role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// The group name
        /// </param>
        public static void DeleteCertificateGroup(this IVaultServiceApi client, string group) {
            client.DeleteCertificateGroupAsync(group).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Delete a group configuration.
        /// </summary>
        /// <remarks>
        /// Deletes a group with configuration.
        /// After this operation the Issuer CA, CRLs and keys become inaccessible.
        /// Use this function with extreme caution.
        /// Requires manager role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// The group name
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task DeleteCertificateGroupAsync(this IVaultServiceApi client,
            string group, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
        }

        /// <summary>
        /// Create new group configuration.
        /// </summary>
        /// <remarks>
        /// Creates a new group with configuration.
        /// The security parameters are preset with defaults.
        /// The group should be updated with final settings before the Issuer CA
        /// certificate is created for the first time.
        /// Requires manager role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// The group name
        /// </param>
        /// <param name='subject'>
        /// The Issuer CA subject
        /// </param>
        /// <param name='certType'>
        /// The certificate type
        /// </param>
        public static CertificateGroupConfigurationApiModel CreateCertificateGroup(
            this IVaultServiceApi client, string group, string subject, string certType) {
            return client.CreateCertificateGroupAsync(group, subject, certType)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Create new group configuration.
        /// </summary>
        /// <remarks>
        /// Creates a new group with configuration.
        /// The security parameters are preset with defaults.
        /// The group should be updated with final settings before the Issuer CA
        /// certificate is created for the first time.
        /// Requires manager role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// The group name
        /// </param>
        /// <param name='subject'>
        /// The Issuer CA subject
        /// </param>
        /// <param name='certType'>
        /// The certificate type
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<CertificateGroupConfigurationApiModel> CreateCertificateGroupAsync(
            this IVaultServiceApi client, string group, string subject, string certType,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Get all group configurations.
        /// </summary>
        /// <remarks>
        /// The group configurations for all groups are stored in KeyVault and contain
        /// information
        /// about the CA subject, the lifetime and the security algorithms used.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        public static CertificateGroupConfigurationCollectionApiModel GetCertificateGroupsConfiguration(
            this IVaultServiceApi client) {
            return client.GetCertificateGroupsConfigurationAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get all group configurations.
        /// </summary>
        /// <remarks>
        /// The group configurations for all groups are stored in KeyVault and contain
        /// information
        /// about the CA subject, the lifetime and the security algorithms used.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<CertificateGroupConfigurationCollectionApiModel> GetCertificateGroupsConfigurationAsync(
            this IVaultServiceApi client, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Get Issuer CA Certificate versions.
        /// </summary>
        /// <remarks>
        /// Returns all Issuer CA certificate versions.
        /// By default only the thumbprints, subject, lifetime and state are returned.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// The group name
        /// </param>
        /// <param name='withCertificates'>
        /// Optional, true to include the full certificates
        /// </param>
        /// <param name='nextPageLink'>
        /// optional, link to next page
        /// </param>
        /// <param name='pageSize'>
        /// optional, the maximum number of result per page
        /// </param>
        public static X509Certificate2CollectionApiModel GetCertificateGroupIssuerCAVersions(
            this IVaultServiceApi client, string group, bool? withCertificates = default,
            string nextPageLink = default, int? pageSize = default) {
            return client.GetCertificateGroupIssuerCAVersionsAsync(group, withCertificates,
                nextPageLink, pageSize).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get Issuer CA Certificate versions.
        /// </summary>
        /// <remarks>
        /// Returns all Issuer CA certificate versions.
        /// By default only the thumbprints, subject, lifetime and state are returned.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// The group name
        /// </param>
        /// <param name='withCertificates'>
        /// Optional, true to include the full certificates
        /// </param>
        /// <param name='nextPageLink'>
        /// optional, link to next page
        /// </param>
        /// <param name='pageSize'>
        /// optional, the maximum number of result per page
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<X509Certificate2CollectionApiModel> GetCertificateGroupIssuerCAVersionsAsync(
            this IVaultServiceApi client, string group, bool? withCertificates = default,
            string nextPageLink = default, int? pageSize = default,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Get Issuer CA Certificate chain.
        /// </summary>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// The group name
        /// </param>
        /// <param name='thumbPrint'>
        /// optional, the thumbrint of the Issuer CA Certificate
        /// </param>
        /// <param name='nextPageLink'>
        /// optional, link to next page
        /// </param>
        /// <param name='pageSize'>
        /// optional, the maximum number of result per page
        /// </param>
        public static X509Certificate2CollectionApiModel GetCertificateGroupIssuerCAChain(
            this IVaultServiceApi client, string group, string thumbPrint = default,
            string nextPageLink = default, int? pageSize = default) {
            return client.GetCertificateGroupIssuerCAChainAsync(group, thumbPrint,
                nextPageLink, pageSize).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get Issuer CA Certificate chain.
        /// </summary>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// The group name
        /// </param>
        /// <param name='thumbPrint'>
        /// optional, the thumbrint of the Issuer CA Certificate
        /// </param>
        /// <param name='nextPageLink'>
        /// optional, link to next page
        /// </param>
        /// <param name='pageSize'>
        /// optional, the maximum number of result per page
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<X509Certificate2CollectionApiModel> GetCertificateGroupIssuerCAChainAsync(
            this IVaultServiceApi client, string group, string thumbPrint = default,
            string nextPageLink = default, int? pageSize = default,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Get Issuer CA CRL chain.
        /// </summary>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// The group name
        /// </param>
        /// <param name='thumbPrint'>
        /// optional, the thumbrint of the Issuer CA Certificate
        /// </param>
        /// <param name='nextPageLink'>
        /// optional, link to next page
        /// </param>
        /// <param name='pageSize'>
        /// optional, the maximum number of result per page
        /// </param>
        public static X509CrlCollectionApiModel GetCertificateGroupIssuerCACrlChain(
            this IVaultServiceApi client, string group, string thumbPrint = default,
            string nextPageLink = default, int? pageSize = default) {
            return client.GetCertificateGroupIssuerCACrlChainAsync(group, thumbPrint,
                nextPageLink, pageSize).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get Issuer CA CRL chain.
        /// </summary>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// The group name
        /// </param>
        /// <param name='thumbPrint'>
        /// optional, the thumbrint of the Issuer CA Certificate
        /// </param>
        /// <param name='nextPageLink'>
        /// optional, link to next page
        /// </param>
        /// <param name='pageSize'>
        /// optional, the maximum number of result per page
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<X509CrlCollectionApiModel> GetCertificateGroupIssuerCACrlChainAsync(
            this IVaultServiceApi client, string group, string thumbPrint = default,
            string nextPageLink = default, int? pageSize = default,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Get Trust lists.
        /// </summary>
        /// <remarks>
        /// The trust lists contain lists for Issuer and Trusted certificates.
        /// The Issuer and Trusted list can each contain CA certificates with CRLs,
        /// signed certificates and self signed certificates.
        /// By default the trusted list contains all versions of Issuer CA certificates
        /// and their latest CRLs.
        /// The issuer list contains certificates and CRLs which might be needed to
        /// validate chains.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// </param>
        /// <param name='nextPageLink'>
        /// optional, link to next page
        /// </param>
        /// <param name='pageSize'>
        /// optional, the maximum number of result per page
        /// </param>
        public static TrustListApiModel GetCertificateGroupTrustList(this IVaultServiceApi client,
            string group, string nextPageLink = default, int? pageSize = default) {
            return client.GetCertificateGroupTrustListAsync(group, nextPageLink, pageSize)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get Trust lists.
        /// </summary>
        /// <remarks>
        /// The trust lists contain lists for Issuer and Trusted certificates.
        /// The Issuer and Trusted list can each contain CA certificates with CRLs,
        /// signed certificates and self signed certificates.
        /// By default the trusted list contains all versions of Issuer CA certificates
        /// and their latest CRLs.
        /// The issuer list contains certificates and CRLs which might be needed to
        /// validate chains.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// </param>
        /// <param name='nextPageLink'>
        /// optional, link to next page
        /// </param>
        /// <param name='pageSize'>
        /// optional, the maximum number of result per page
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<TrustListApiModel> GetCertificateGroupTrustListAsync(
            this IVaultServiceApi client, string group, string nextPageLink = default,
            int? pageSize = default, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Create a new Issuer CA Certificate.
        /// </summary>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// </param>
        public static X509Certificate2ApiModel CreateCertificateGroupIssuerCACert(
            this IVaultServiceApi client, string group) {
            return client.CreateCertificateGroupIssuerCACertAsync(group)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Create a new Issuer CA Certificate.
        /// </summary>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<X509Certificate2ApiModel> CreateCertificateGroupIssuerCACertAsync(
            this IVaultServiceApi client, string group, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Create a certificate request with a certificate signing request (CSR).
        /// </summary>
        /// <remarks>
        /// The request is in the 'New' state after this call.
        /// Requires Writer or Manager role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='signingRequest'>
        /// The signing request parameters
        /// </param>
        public static string CreateSigningRequest(this IVaultServiceApi client,
            CreateSigningRequestApiModel signingRequest = default) {
            return client.CreateSigningRequestAsync(signingRequest)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Create a certificate request with a certificate signing request (CSR).
        /// </summary>
        /// <remarks>
        /// The request is in the 'New' state after this call.
        /// Requires Writer or Manager role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='signingRequest'>
        /// The signing request parameters
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<string> CreateSigningRequestAsync(this IVaultServiceApi client,
            CreateSigningRequestApiModel signingRequest = default,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Create a certificate request with a new key pair.
        /// </summary>
        /// <remarks>
        /// The request is in the 'New' state after this call.
        /// Requires Writer or Manager role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='newKeyPairRequest'>
        /// The new key pair request parameters
        /// </param>
        public static string CreateNewKeyPairRequest(this IVaultServiceApi client,
            CreateNewKeyPairRequestApiModel newKeyPairRequest = default) {
            return client.CreateNewKeyPairRequestAsync(newKeyPairRequest)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Create a certificate request with a new key pair.
        /// </summary>
        /// <remarks>
        /// The request is in the 'New' state after this call.
        /// Requires Writer or Manager role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='newKeyPairRequest'>
        /// The new key pair request parameters
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<string> CreateNewKeyPairRequestAsync(
            this IVaultServiceApi client, CreateNewKeyPairRequestApiModel newKeyPairRequest = default,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Approve the certificate request.
        /// </summary>
        /// <remarks>
        /// Validates the request with the application database.
        /// - If Approved:
        /// - New Key Pair request: Creates the new key pair
        /// in the requested format, signs the certificate and stores the
        /// private key for later securely in KeyVault.
        /// - Cert Signing Request: Creates and signs the certificate.
        /// Deletes the CSR from the database.
        /// Stores the signed certificate for later use in the Database.
        /// The request is in the 'Approved' or 'Rejected' state after this call.
        /// Requires Approver role.
        /// Approver needs signing rights in KeyVault.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        /// <param name='rejected'>
        /// if the request is rejected(true) or approved(false)
        /// </param>
        public static void ApproveCertificateRequest(this IVaultServiceApi client,
            string requestId, bool rejected) {
            client.ApproveCertificateRequestAsync(requestId, rejected)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Approve the certificate request.
        /// </summary>
        /// <remarks>
        /// Validates the request with the application database.
        /// - If Approved:
        /// - New Key Pair request: Creates the new key pair
        /// in the requested format, signs the certificate and stores the
        /// private key for later securely in KeyVault.
        /// - Cert Signing Request: Creates and signs the certificate.
        /// Deletes the CSR from the database.
        /// Stores the signed certificate for later use in the Database.
        /// The request is in the 'Approved' or 'Rejected' state after this call.
        /// Requires Approver role.
        /// Approver needs signing rights in KeyVault.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        /// <param name='rejected'>
        /// if the request is rejected(true) or approved(false)
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task ApproveCertificateRequestAsync(this IVaultServiceApi client,
            string requestId, bool rejected, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
        }

        /// <summary>
        /// Accept request and delete the private key.
        /// </summary>
        /// <remarks>
        /// By accepting the request the requester takes ownership of the certificate
        /// and the private key, if requested. A private key with metadata is deleted
        /// from KeyVault.
        /// The public certificate remains in the database for sharing public key
        /// information
        /// or for later revocation once the application is deleted.
        /// The request is in the 'Accepted' state after this call.
        /// Requires Writer role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        public static void AcceptCertificateRequest(this IVaultServiceApi client,
            string requestId) {
            client.AcceptCertificateRequestAsync(requestId)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Accept request and delete the private key.
        /// </summary>
        /// <remarks>
        /// By accepting the request the requester takes ownership of the certificate
        /// and the private key, if requested. A private key with metadata is deleted
        /// from KeyVault.
        /// The public certificate remains in the database for sharing public key
        /// information
        /// or for later revocation once the application is deleted.
        /// The request is in the 'Accepted' state after this call.
        /// Requires Writer role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task AcceptCertificateRequestAsync(this IVaultServiceApi client,
            string requestId, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
        }

        /// <summary>
        /// Get a specific certificate request.
        /// </summary>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        public static CertificateRequestRecordApiModel GetCertificateRequest(
            this IVaultServiceApi client, string requestId) {
            return client.GetCertificateRequestAsync(requestId)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get a specific certificate request.
        /// </summary>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<CertificateRequestRecordApiModel> GetCertificateRequestAsync(
            this IVaultServiceApi client, string requestId,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Delete request. Mark the certificate for revocation.
        /// </summary>
        /// <remarks>
        /// If the request is in the 'Approved' or 'Accepted' state,
        /// the request is set in the 'Deleted' state.
        /// A deleted request is marked for revocation.
        /// The public certificate is still available for the revocation procedure.
        /// If the request is in the 'New' or 'Rejected' state,
        /// the request is set in the 'Removed' state.
        /// The request is in the 'Deleted' or 'Removed'state after this call.
        /// Requires Manager role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        public static void DeleteCertificateRequest(this IVaultServiceApi client,
            string requestId) {
            client.DeleteCertificateRequestAsync(requestId)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Delete request. Mark the certificate for revocation.
        /// </summary>
        /// <remarks>
        /// If the request is in the 'Approved' or 'Accepted' state,
        /// the request is set in the 'Deleted' state.
        /// A deleted request is marked for revocation.
        /// The public certificate is still available for the revocation procedure.
        /// If the request is in the 'New' or 'Rejected' state,
        /// the request is set in the 'Removed' state.
        /// The request is in the 'Deleted' or 'Removed'state after this call.
        /// Requires Manager role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task DeleteCertificateRequestAsync(this IVaultServiceApi client,
            string requestId, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
        }

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
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        public static void PurgeCertificateRequest(this IVaultServiceApi client,
            string requestId) {
            client.PurgeCertificateRequestAsync(requestId)
                .GetAwaiter().GetResult();
        }

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
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task PurgeCertificateRequestAsync(this IVaultServiceApi client,
            string requestId, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
        }

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
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        public static void RevokeCertificateRequest(this IVaultServiceApi client,
            string requestId) {
            client.RevokeCertificateRequestAsync(requestId)
                .GetAwaiter().GetResult();
        }

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
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='requestId'>
        /// The certificate request id
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task RevokeCertificateRequestAsync(this IVaultServiceApi client,
            string requestId, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
        }

        /// <summary>
        /// Revoke all deleted certificate requests for a group.
        /// </summary>
        /// <remarks>
        /// Select all requests for a group in the 'Deleted' state are marked
        /// for revocation.
        /// The certificate issuer CA and CRL are looked up, all the certificate
        /// serial numbers are added and a new CRL version is issued and updated
        /// in the certificate group storage.
        /// Requires Approver role.
        /// Approver needs signing rights in KeyVault.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// The certificate group id
        /// </param>
        /// <param name='allVersions'>
        /// optional, if all certs for all CA versions should be revoked. Default: true
        /// </param>
        public static void RevokeCertificateGroup(this IVaultServiceApi client,
            string group, bool? allVersions = default) {
            client.RevokeCertificateGroupAsync(group, allVersions)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Revoke all deleted certificate requests for a group.
        /// </summary>
        /// <remarks>
        /// Select all requests for a group in the 'Deleted' state are marked
        /// for revocation.
        /// The certificate issuer CA and CRL are looked up, all the certificate
        /// serial numbers are added and a new CRL version is issued and updated
        /// in the certificate group storage.
        /// Requires Approver role.
        /// Approver needs signing rights in KeyVault.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='group'>
        /// The certificate group id
        /// </param>
        /// <param name='allVersions'>
        /// optional, if all certs for all CA versions should be revoked. Default: true
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task RevokeCertificateGroupAsync(this IVaultServiceApi client,
            string group, bool? allVersions = default, CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
        }

        /// <summary>
        /// Query for certificate requests.
        /// </summary>
        /// <remarks>
        /// Get all certificate requests in paged form.
        /// The returned model can contain a link to the next page if more results are
        /// available.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='appId'>
        /// optional, query for application id
        /// </param>
        /// <param name='requestState'>
        /// optional, query for request state. Possible values include: 'new',
        /// 'approved', 'rejected', 'accepted', 'deleted', 'revoked', 'removed'
        /// </param>
        /// <param name='nextPageLink'>
        /// optional, link to next page
        /// </param>
        /// <param name='pageSize'>
        /// optional, the maximum number of result per page
        /// </param>
        public static CertificateRequestQueryResponseApiModel QueryCertificateRequests(
            this IVaultServiceApi client, string appId = default, string requestState = default,
            string nextPageLink = default, int? pageSize = default) {
            return client.QueryCertificateRequestsAsync(appId, requestState, nextPageLink, pageSize)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Query for certificate requests.
        /// </summary>
        /// <remarks>
        /// Get all certificate requests in paged form.
        /// The returned model can contain a link to the next page if more results are
        /// available.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='appId'>
        /// optional, query for application id
        /// </param>
        /// <param name='requestState'>
        /// optional, query for request state. Possible values include: 'new',
        /// 'approved', 'rejected', 'accepted', 'deleted', 'revoked', 'removed'
        /// </param>
        /// <param name='nextPageLink'>
        /// optional, link to next page
        /// </param>
        /// <param name='pageSize'>
        /// optional, the maximum number of result per page
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<CertificateRequestQueryResponseApiModel> QueryCertificateRequestsAsync(
            this IVaultServiceApi client, string appId = default, string requestState = default,
            string nextPageLink = default, int? pageSize = default,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Fetch certificate request approval result.
        /// </summary>
        /// <remarks>
        /// Can be called in any state.
        /// Returns only cert request information in 'New', 'Rejected',
        /// 'Deleted' and 'Revoked' state.
        /// Fetches private key in 'Approved' state, if requested.
        /// Fetches the public certificate in 'Approved' and 'Accepted' state.
        /// After a successful fetch in 'Approved' state, the request should be
        /// accepted to delete the private key.
        /// Requires Writer role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='requestId'>
        /// </param>
        /// <param name='applicationId'>
        /// </param>
        public static FetchRequestResultApiModel FetchCertificateRequestResult(
            this IVaultServiceApi client, string requestId, string applicationId) {
            return client.FetchCertificateRequestResultAsync(requestId, applicationId)
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Fetch certificate request approval result.
        /// </summary>
        /// <remarks>
        /// Can be called in any state.
        /// Returns only cert request information in 'New', 'Rejected',
        /// 'Deleted' and 'Revoked' state.
        /// Fetches private key in 'Approved' state, if requested.
        /// Fetches the public certificate in 'Approved' and 'Accepted' state.
        /// After a successful fetch in 'Approved' state, the request should be
        /// accepted to delete the private key.
        /// Requires Writer role.
        /// </remarks>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='requestId'>
        /// </param>
        /// <param name='applicationId'>
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<FetchRequestResultApiModel> FetchCertificateRequestResultAsync(
            this IVaultServiceApi client, string requestId, string applicationId,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }

        /// <summary>
        /// Get the status.
        /// </summary>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        public static StatusApiModel GetStatus(this IVaultServiceApi client) {
            return client.GetStatusAsync()
                .GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get the status.
        /// </summary>
        /// <param name='client'>
        /// The client group for this extension method.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<StatusApiModel> GetStatusAsync(this IVaultServiceApi client,
            CancellationToken cancellationToken = default) {
            await Task.FromException(new NotImplementedException());
            return null;
        }
    }
}
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore RECS0154 // Parameter is never used
