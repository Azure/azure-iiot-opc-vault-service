// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.Client
{
    using Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for GdsVault.
    /// </summary>
    public static partial class GdsVaultExtensions
    {
            /// <summary>
            /// Register new application.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='application'>
            /// </param>
            public static string RegisterApplication(this IGdsVault operations, ApplicationRecordApiModel application = default(ApplicationRecordApiModel))
            {
                return operations.RegisterApplicationAsync(application).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Register new application.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='application'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<string> RegisterApplicationAsync(this IGdsVault operations, ApplicationRecordApiModel application = default(ApplicationRecordApiModel), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.RegisterApplicationWithHttpMessagesAsync(application, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get application
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            public static ApplicationRecordApiModel GetApplication(this IGdsVault operations, string id)
            {
                return operations.GetApplicationAsync(id).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get application
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ApplicationRecordApiModel> GetApplicationAsync(this IGdsVault operations, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetApplicationWithHttpMessagesAsync(id, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Update application.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='application'>
            /// </param>
            public static string UpdateApplication(this IGdsVault operations, string id, ApplicationRecordApiModel application = default(ApplicationRecordApiModel))
            {
                return operations.UpdateApplicationAsync(id, application).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Update application.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='application'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<string> UpdateApplicationAsync(this IGdsVault operations, string id, ApplicationRecordApiModel application = default(ApplicationRecordApiModel), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.UpdateApplicationWithHttpMessagesAsync(id, application, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Unregister application
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            public static void UnregisterApplication(this IGdsVault operations, string id)
            {
                operations.UnregisterApplicationAsync(id).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Unregister application
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task UnregisterApplicationAsync(this IGdsVault operations, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.UnregisterApplicationWithHttpMessagesAsync(id, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <summary>
            /// Find applications
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='uri'>
            /// </param>
            public static IList<ApplicationRecordApiModel> FindApplication(this IGdsVault operations, string uri)
            {
                return operations.FindApplicationAsync(uri).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Find applications
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='uri'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<ApplicationRecordApiModel>> FindApplicationAsync(this IGdsVault operations, string uri, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.FindApplicationWithHttpMessagesAsync(uri, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Query applications
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='query'>
            /// </param>
            public static QueryApplicationsResponseApiModel QueryApplications(this IGdsVault operations, QueryApplicationsApiModel query = default(QueryApplicationsApiModel))
            {
                return operations.QueryApplicationsAsync(query).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Query applications
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='query'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<QueryApplicationsResponseApiModel> QueryApplicationsAsync(this IGdsVault operations, QueryApplicationsApiModel query = default(QueryApplicationsApiModel), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.QueryApplicationsWithHttpMessagesAsync(query, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static CertificateGroupListApiModel GetCertificateGroupIds(this IGdsVault operations)
            {
                return operations.GetCertificateGroupIdsAsync().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<CertificateGroupListApiModel> GetCertificateGroupIdsAsync(this IGdsVault operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetCertificateGroupIdsWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get group configuration
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            public static CertificateGroupConfigurationApiModel GetCertificateGroupConfiguration(this IGdsVault operations, string id)
            {
                return operations.GetCertificateGroupConfigurationAsync(id).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get group configuration
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<CertificateGroupConfigurationApiModel> GetCertificateGroupConfigurationAsync(this IGdsVault operations, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetCertificateGroupConfigurationWithHttpMessagesAsync(id, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get group configuration
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static CertificateGroupConfigurationCollectionApiModel GetCertificateGroupConfigurationCollection(this IGdsVault operations)
            {
                return operations.GetCertificateGroupConfigurationCollectionAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get group configuration
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<CertificateGroupConfigurationCollectionApiModel> GetCertificateGroupConfigurationCollectionAsync(this IGdsVault operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetCertificateGroupConfigurationCollectionWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get CA Certificate chain
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            public static X509Certificate2CollectionApiModel GetCACertificateChain(this IGdsVault operations, string id)
            {
                return operations.GetCACertificateChainAsync(id).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get CA Certificate chain
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<X509Certificate2CollectionApiModel> GetCACertificateChainAsync(this IGdsVault operations, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetCACertificateChainWithHttpMessagesAsync(id, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get CA CRL chain
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            public static X509CrlCollectionApiModel GetCACrlChain(this IGdsVault operations, string id)
            {
                return operations.GetCACrlChainAsync(id).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get CA CRL chain
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<X509CrlCollectionApiModel> GetCACrlChainAsync(this IGdsVault operations, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetCACrlChainWithHttpMessagesAsync(id, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get trust list
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            public static TrustListApiModel GetTrustList(this IGdsVault operations, string id)
            {
                return operations.GetTrustListAsync(id).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get trust list
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<TrustListApiModel> GetTrustListAsync(this IGdsVault operations, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetTrustListWithHttpMessagesAsync(id, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Create new CA Certificate
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            public static X509Certificate2ApiModel CreateCACertificate(this IGdsVault operations, string id)
            {
                return operations.CreateCACertificateAsync(id).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Create new CA Certificate
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<X509Certificate2ApiModel> CreateCACertificateAsync(this IGdsVault operations, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CreateCACertificateWithHttpMessagesAsync(id, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Revoke Certificate
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='cert'>
            /// </param>
            public static X509CrlApiModel RevokeCertificate(this IGdsVault operations, string id, X509Certificate2ApiModel cert = default(X509Certificate2ApiModel))
            {
                return operations.RevokeCertificateAsync(id, cert).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Revoke Certificate
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='cert'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<X509CrlApiModel> RevokeCertificateAsync(this IGdsVault operations, string id, X509Certificate2ApiModel cert = default(X509Certificate2ApiModel), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.RevokeCertificateWithHttpMessagesAsync(id, cert, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Certificate Signing Request
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='sr'>
            /// </param>
            public static X509Certificate2ApiModel SigningRequest(this IGdsVault operations, string id, SigningRequestApiModel sr = default(SigningRequestApiModel))
            {
                return operations.SigningRequestAsync(id, sr).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Certificate Signing Request
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='sr'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<X509Certificate2ApiModel> SigningRequestAsync(this IGdsVault operations, string id, SigningRequestApiModel sr = default(SigningRequestApiModel), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.SigningRequestWithHttpMessagesAsync(id, sr, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Create New Key Pair
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='nkpr'>
            /// </param>
            public static CertificateKeyPairApiModel NewKeyPairRequest(this IGdsVault operations, string id, NewKeyPairRequestApiModel nkpr = default(NewKeyPairRequestApiModel))
            {
                return operations.NewKeyPairRequestAsync(id, nkpr).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Create New Key Pair
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='nkpr'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<CertificateKeyPairApiModel> NewKeyPairRequestAsync(this IGdsVault operations, string id, NewKeyPairRequestApiModel nkpr = default(NewKeyPairRequestApiModel), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.NewKeyPairRequestWithHttpMessagesAsync(id, nkpr, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static StatusApiModel V1StatusGet(this IGdsVault operations)
            {
                return operations.V1StatusGetAsync().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<StatusApiModel> V1StatusGetAsync(this IGdsVault operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.V1StatusGetWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}