// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.Azure.IIoT.Diagnostics;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.CosmosDB;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault
{
    internal sealed class CosmosDBApplicationsDatabase : IApplicationsDatabase 
    {
        private readonly ILogger _log;
        private readonly string Endpoint;
        private SecureString AuthKeyOrResourceToken;

        public CosmosDBApplicationsDatabase(
            IServicesConfig config,
            ILogger logger)
        {
            this.Endpoint = config.CosmosDBEndpoint;
            this.AuthKeyOrResourceToken = new SecureString();
            foreach (char ch in config.CosmosDBToken)
            {
                this.AuthKeyOrResourceToken.AppendChar(ch);
            }
            _log = logger;
            _log.Debug("Creating new instance of `CosmosDBApplicationsDatabase` service " + config.CosmosDBEndpoint, () => { });
            Initialize();
        }

        #region IApplicationsDatabase 

        public async Task<string> RegisterApplicationAsync(Application application)
        {
            bool isNew = false;
            Guid applicationId = VerifyRegisterApplication(application);
            if (applicationId == null || Guid.Empty == applicationId)
            {
                isNew = true;
            }

            string capabilities = ServerCapabilities(application);

            if (applicationId != Guid.Empty)
            {
                Application record = await Applications.GetAsync(applicationId);
                if (record == null)
                {
                    application.ApplicationId = Guid.NewGuid();
                    isNew = true;
                }
            }

            if (isNew)
            {
                // find new ID for QueryServers
                var maxAppIDEnum = await Applications.GetAsync("SELECT TOP 1 * FROM Applications a ORDER BY a.ID DESC");
                var maxAppID = maxAppIDEnum.SingleOrDefault();
                application.ID = (maxAppID != null) ? maxAppID.ID + 1 : 1;
                application.ApplicationId = Guid.NewGuid();
                var result = await Applications.CreateAsync(application);
                applicationId = new Guid(result.Id);
            }
            else
            {
                await Applications.UpdateAsync(applicationId, application);
            }

            return applicationId.ToString();
        }

        public async Task<string> UpdateApplicationAsync(string id, Application application)
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentException("The id must be provided", "id");
            }

            if (application == null)
            {
                throw new ArgumentException("The application must be provided", "application");
            }

            Guid recordId = VerifyRegisterApplication(application);
            Guid applicationId = new Guid(id);

            if (recordId == null || recordId == Guid.Empty)
            {
                application.ApplicationId = applicationId;
            }

            string capabilities = ServerCapabilities(application);

            if (applicationId != Guid.Empty)
            {
                var record = await Applications.GetAsync(applicationId);
                if (record == null)
                {
                    throw new ArgumentException("A record with the specified application id does not exist.", nameof(id));
                }

                record.ApplicationUri = application.ApplicationUri;
                record.ApplicationName = application.ApplicationName;
                record.ApplicationType = application.ApplicationType;
                record.ProductUri = application.ProductUri;
                record.ServerCapabilities = capabilities;
                record.ApplicationNames = application.ApplicationNames;
                record.DiscoveryUrls = application.DiscoveryUrls;

                await Applications.UpdateAsync(applicationId, record);
            }
            return applicationId.ToString();
        }

        public async Task UnregisterApplicationAsync(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentException("The id must be provided", "id");
            }

            Guid appId = new Guid(id);

            List<byte[]> certificates = new List<byte[]>();

            var application = await Applications.GetAsync(appId);
            if (application == null)
            {
                throw new ArgumentException("A record with the specified application id does not exist.", nameof(appId));
            }

            var certificateRequests = await CertificateRequests.GetAsync(ii => ii.ApplicationId == appId);
            foreach (var entry in new List<CertificateRequest>(certificateRequests))
            {
                await CertificateRequests.DeleteAsync(entry.RequestId);
            }

            await Applications.DeleteAsync(appId);
        }

        public async Task<Application> GetApplicationAsync(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentException("The id must be provided", "id");
            }

            Guid appId = new Guid(id);
            return await Applications.GetAsync(appId);
        }

        public async Task<Application[]> FindApplicationAsync(string applicationUri)
        {
            if (String.IsNullOrEmpty(applicationUri))
            {
                throw new ArgumentException("The applicationUri must be provided", "applicationUri");
            }

            var results = await Applications.GetAsync(ii => ii.ApplicationUri == applicationUri);
            return results.ToArray();
        }

        public Task<Application[]> QueryApplicationsAsync(
            uint startingRecordId,
            uint maxRecordsToReturn,
            string applicationName,
            string applicationUri,
            uint applicationType,
            string productUri,
            string[] serverCapabilities,
            out DateTime lastCounterResetTime,
            out uint nextRecordId
            )
        {
            lastCounterResetTime = DateTime.MinValue;
            nextRecordId = 0;
            return Task.FromResult<Application[]>(null);
        }

        public Task<Application[]> QueryServersAsync(
            uint startingRecordId,
            uint maxRecordsToReturn,
            string applicationName,
            string applicationUri,
            string productUri,
            string[] serverCapabilities,
            out DateTime lastCounterResetTime
            )
        {
            lastCounterResetTime = DateTime.MinValue;
            return Task.FromResult<Application[]>(null);
        }

#if mist
            List<ServerOnNetwork> records = new List<ServerOnNetwork>();
            const uint defaultRecordsPerQuery = 10;

            lastCounterResetTime = queryCounterResetTime;

            bool matchQuery = false;
            bool complexQuery =
                !String.IsNullOrEmpty(applicationName) ||
                !String.IsNullOrEmpty(applicationUri) ||
                !String.IsNullOrEmpty(productUri) ||
                (serverCapabilities != null && serverCapabilities.Length > 0);

            if (complexQuery)
            {
                // TODO: implement query with server side match...
                matchQuery =
                    IsMatchPattern(applicationName) ||
                    IsMatchPattern(applicationUri) ||
                    IsMatchPattern(productUri);
            }

            bool lastQuery = false;
            do
            {
                uint queryRecords = complexQuery ? defaultRecordsPerQuery : maxRecordsToReturn;
                // TODO: implement query with server side match...
                string query = CreateServerQuery(startingRecordId, queryRecords);
                var applications = Applications.GetAsync(query).Result;
                lastQuery = queryRecords == 0 || applications.Count() < queryRecords || applications.Count() == 0;

                foreach (var result in applications)
                {
                    startingRecordId = result.ID + 1;

                    if (!String.IsNullOrEmpty(applicationName))
                    {
                        if (!Match(result.ApplicationName, applicationName))
                        {
                            continue;
                        }
                    }

                    if (!String.IsNullOrEmpty(applicationUri))
                    {
                        if (!Match(result.ApplicationUri, applicationUri))
                        {
                            continue;
                        }
                    }

                    if (!String.IsNullOrEmpty(productUri))
                    {
                        if (!Match(result.ProductUri, productUri))
                        {
                            continue;
                        }
                    }

                    string[] capabilities = null;
                    if (!String.IsNullOrEmpty(result.ServerCapabilities))
                    {
                        capabilities = result.ServerCapabilities.Split(',');
                    }

                    if (serverCapabilities != null && serverCapabilities.Length > 0)
                    {
                        bool match = true;
                        for (int ii = 0; ii < serverCapabilities.Length; ii++)
                        {
                            if (capabilities == null || !capabilities.Contains(serverCapabilities[ii]))
                            {
                                match = false;
                                break;
                            }
                        }

                        if (!match)
                        {
                            continue;
                        }
                    }

                    if (result.DiscoveryUrls != null)
                    {
                        foreach (var discoveryUrl in result.DiscoveryUrls)
                        {
                            records.Add(new ServerOnNetwork()
                            {
                                RecordId = result.ID,
                                ServerName = result.ApplicationName,
                                DiscoveryUrl = discoveryUrl,
                                ServerCapabilities = capabilities
                            });
                        }
                    }

                    if (--maxRecordsToReturn == 0)
                    {
                        break;
                    }
                }
            } while (maxRecordsToReturn > 0 && !lastQuery);

            return records.ToArray();
#endif
#if mist
        public override bool SetApplicationCertificate(
            NodeId applicationId,
            byte[] certificate,
            bool isHttpsCertificate
            )
        {
            Guid id = GetNodeIdGuid(applicationId);

            var result = Applications.GetAsync(id).Result;
            if (result == null)
            {
                return false;
            }

            if (isHttpsCertificate)
            {
                result.HttpsCertificate = certificate;
            }
            else
            {
                result.Certificate = certificate;
            }

            Applications.UpdateAsync(result.ApplicationId, result).Wait();

            return true;
        }

        public override bool SetApplicationTrustLists(
            NodeId applicationId,
            NodeId trustListId,
            NodeId httpsTrustListId
            )
        {
            Guid id = GetNodeIdGuid(applicationId);

            var result = Applications.GetAsync(id).Result;
            if (result == null)
            {
                return false;
            }

            result.TrustListId = null;
            result.HttpsTrustListId = null;

            if (trustListId != null)
            {
                string storePath = trustListId.ToString();

                var result2 = CertificateStores.GetAsync(x => x.Path == storePath).Result.SingleOrDefault();
                if (result2 != null)
                {
                    result.TrustListId = result2.TrustListId;
                }
            }

            if (httpsTrustListId != null)
            {
                string storePath = httpsTrustListId.ToString();
                var result2 = CertificateStores.GetAsync(x => x.Path == storePath).Result.SingleOrDefault();
                if (result2 != null)
                {
                    result.HttpsTrustListId = result2.TrustListId;
                }
            }

            Applications.UpdateAsync(result.ApplicationId, result).Wait();

            return true;
        }
#endif
        #endregion

        #region ICertificateRequest
#if mist
        public NodeId CreateSigningRequest(
            NodeId applicationId,
            NodeId certificateGroupId,
            NodeId certificateTypeId,
            byte[] certificateRequest,
            string authorityId)
        {
            Guid id = GetNodeIdGuid(applicationId);

            var application = Applications.GetAsync(id).Result;
            if (application == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            CertificateRequest request = null;
            bool isNew = false;

            if (request == null)
            {
                request = new CertificateRequest() { RequestId = Guid.NewGuid(), AuthorityId = authorityId };
                isNew = true;
            }

            request.State = (int)CertificateRequestState.New;
            request.CertificateGroupId = certificateGroupId;
            request.CertificateTypeId = certificateTypeId;
            request.SubjectName = null;
            request.DomainNames = null;
            request.PrivateKeyFormat = null;
            request.PrivateKeyPassword = null;
            request.CertificateSigningRequest = certificateRequest;
            request.ApplicationId = id;
            request.RequestTime = DateTime.UtcNow;

            if (isNew)
            {
                CertificateRequests.CreateAsync(request).Wait();
            }
            else
            {
                CertificateRequests.UpdateAsync(request.RequestId, request).Wait();
            }

            return new NodeId(request.RequestId, NamespaceIndex);
        }

        public NodeId CreateNewKeyPairRequest(
            NodeId applicationId,
            NodeId certificateGroupId,
            NodeId certificateTypeId,
            string subjectName,
            string[] domainNames,
            string privateKeyFormat,
            string privateKeyPassword,
            string authorityId)
        {
            Guid id = GetNodeIdGuid(applicationId);

            var application = Applications.GetAsync(id).Result;

            if (application == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            CertificateRequest request = null;
            bool isNew = false;

            if (request == null)
            {
                request = new CertificateRequest()
                {
                    RequestId = Guid.NewGuid(),
                    AuthorityId = authorityId
                };
                isNew = true;
            }

            request.State = (int)CertificateRequestState.New;
            request.CertificateGroupId = certificateGroupId;
            request.CertificateTypeId = certificateTypeId;
            request.SubjectName = subjectName;
            request.DomainNames = domainNames;
            request.PrivateKeyFormat = privateKeyFormat;
            request.PrivateKeyPassword = privateKeyPassword;
            request.CertificateSigningRequest = null;
            request.ApplicationId = id;
            request.RequestTime = DateTime.UtcNow;

            if (isNew)
            {
                CertificateRequests.CreateAsync(request).Wait();
            }
            else
            {
                CertificateRequests.UpdateAsync(request.RequestId, request).Wait();
            }

            return new NodeId(request.RequestId, NamespaceIndex);
        }

        public void ApproveCertificateRequest(
            NodeId requestId,
            bool isRejected
            )
        {
            Guid id = GetNodeIdGuid(requestId);

            var request = CertificateRequests.GetAsync(id).Result;
            if (request == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            if (request.State != Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Models.CertificateRequestState.New)
            {
                throw new ServiceResultException(StatusCodes.BadInvalidState);
            }

            if (isRejected)
            {
                request.State = Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Models.CertificateRequestState.Rejected;
                // erase information which is not required anymore
                request.PrivateKeyFormat = null;
                request.CertificateSigningRequest = null;
                request.PrivateKeyPassword = null;
            }
            else
            {
                request.State = Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Models.CertificateRequestState.Approved;
            }

            request.ApproveRejectTime = DateTime.UtcNow;

            CertificateRequests.UpdateAsync(request.RequestId, request).Wait();
        }

        public void AcceptCertificateRequest(NodeId requestId, byte[] signedCertificate)
        {
            Guid id = GetNodeIdGuid(requestId);

            var request = CertificateRequests.GetAsync(id).Result;
            if (request == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            if (request.State != Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Models.CertificateRequestState.Approved)
            {
                throw new ServiceResultException(StatusCodes.BadInvalidState);
            }

            request.State = Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Models.CertificateRequestState.Accepted;

            // erase information which is not required anymore
            request.CertificateSigningRequest = null;
            request.PrivateKeyFormat = null;
            request.PrivateKeyPassword = null;
            request.AcceptTime = DateTime.UtcNow;
            request.Certificate = signedCertificate;

            CertificateRequests.UpdateAsync(request.RequestId, request).Wait();
        }

        public CertificateRequestState CompleteCertificateRequest(
            NodeId applicationId,
            NodeId requestId,
            out NodeId certificateGroupId,
            out NodeId certificateTypeId,
            out byte[] signedCertificate,
            out byte[] privateKey)
        {
            certificateGroupId = null;
            certificateTypeId = null;
            signedCertificate = null;
            privateKey = null;
            Guid reqId = GetNodeIdGuid(requestId);
            Guid appId = GetNodeIdGuid(applicationId);

            var application = Applications.GetAsync(appId).Result;
            if (application == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            var request = CertificateRequests.GetAsync(reqId).Result;
            if (request == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            switch (request.State)
            {
                case Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Models.CertificateRequestState.New:
                    return CertificateRequestState.New;
                case Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Models.CertificateRequestState.Rejected:
                    return CertificateRequestState.Rejected;
                case Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Models.CertificateRequestState.Accepted:
                    return CertificateRequestState.Accepted;
                case Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Models.CertificateRequestState.Approved:
                    break;
                default:
                    throw new ServiceResultException(StatusCodes.BadInvalidArgument);
            }

            if (request.ApplicationId != appId)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            certificateGroupId = request.CertificateGroupId;
            certificateTypeId = request.CertificateTypeId;
            signedCertificate = request.Certificate;
            privateKey = request.PrivateKey;

            return CertificateRequestState.Approved;
        }
        public CertificateRequestState ReadRequest(
            NodeId applicationId,
            NodeId requestId,
            out NodeId certificateGroupId,
            out NodeId certificateTypeId,
            out byte[] certificateRequest,
            out string subjectName,
            out string[] domainNames,
            out string privateKeyFormat,
            out string privateKeyPassword)
        {
            certificateGroupId = null;
            certificateTypeId = null;
            certificateRequest = null;
            subjectName = null;
            domainNames = null;
            privateKeyFormat = null;
            privateKeyPassword = null;
            Guid reqId = GetNodeIdGuid(requestId);
            Guid appId = GetNodeIdGuid(applicationId);

            var application = Applications.GetAsync(appId).Result;
            if (application == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            var request = CertificateRequests.GetAsync(reqId).Result;
            if (request == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            switch (request.State)
            {
                case Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Models.CertificateRequestState.New:
                    return CertificateRequestState.New;
                case Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Models.CertificateRequestState.Rejected:
                    return CertificateRequestState.Rejected;
                case Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Models.CertificateRequestState.Accepted:
                    return CertificateRequestState.Accepted;
                case Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Models.CertificateRequestState.Approved:
                    break;
                default:
                    throw new ServiceResultException(StatusCodes.BadInvalidArgument);
            }

            if (request.ApplicationId != appId)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            certificateGroupId = request.CertificateGroupId;
            certificateTypeId = request.CertificateTypeId;
            certificateRequest = request.CertificateSigningRequest;
            subjectName = request.SubjectName;
            domainNames = request.DomainNames;
            privateKeyFormat = request.PrivateKeyFormat;
            privateKeyPassword = request.PrivateKeyPassword;

            return CertificateRequestState.Approved;
        }
#endif
        #endregion

        #region Private Members
        private void Initialize()
        {
            db = new DocumentDBRepository(Endpoint, AuthKeyOrResourceToken);
            Applications = new DocumentDBCollection<Application>(db);
            CertificateRequests = new DocumentDBCollection<CertificateRequest>(db);
            CertificateStores = new DocumentDBCollection<CertificateStore>(db);
        }

        private string CreateServerQuery(uint startingRecordId, uint maxRecordsToQuery)
        {
            string query;
            if (maxRecordsToQuery != 0)
            {
                query = String.Format("SELECT TOP {0}", maxRecordsToQuery);
            }
            else
            {
                query = String.Format("SELECT");
            }
            query += String.Format(" * FROM Applications a WHERE a.ID >= {0} ORDER BY a.ID", startingRecordId);
            return query;
        }
        private Guid VerifyRegisterApplication(Application application)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (application.ApplicationUri == null)
            {
                throw new ArgumentNullException("ApplicationUri");
            }

            if (!Uri.IsWellFormedUriString(application.ApplicationUri, UriKind.Absolute))
            {
                throw new ArgumentException(application.ApplicationUri + " is not a valid URI.", "ApplicationUri");
            }

            if (application.ApplicationType < (int)Opc.Ua.ApplicationType.Server || 
                application.ApplicationType > (int)Opc.Ua.ApplicationType.DiscoveryServer)
            {
                throw new ArgumentException(application.ApplicationType.ToString() + " is not a valid ApplicationType.", "ApplicationType");
            }

            if (application.ApplicationNames == null || application.ApplicationNames.Length == 0 || String.IsNullOrEmpty(application.ApplicationNames[0].Text))
            {
                throw new ArgumentException("At least one ApplicationName must be provided.", "ApplicationNames");
            }

            if (String.IsNullOrEmpty(application.ProductUri))
            {
                throw new ArgumentException("A ProductUri must be provided.", "ProductUri");
            }

            if (!Uri.IsWellFormedUriString(application.ProductUri, UriKind.Absolute))
            {
                throw new ArgumentException(application.ProductUri + " is not a valid URI.", "ProductUri");
            }

            if (application.DiscoveryUrls != null)
            {
                foreach (var discoveryUrl in application.DiscoveryUrls)
                {
                    if (String.IsNullOrEmpty(discoveryUrl))
                    {
                        continue;
                    }

                    if (!Uri.IsWellFormedUriString(discoveryUrl, UriKind.Absolute))
                    {
                        throw new ArgumentException(discoveryUrl + " is not a valid URL.", "DiscoveryUrls");
                    }
                }
            }

            if (application.ApplicationType != (int)Opc.Ua.ApplicationType.Client)
            {
                if (application.DiscoveryUrls == null || application.DiscoveryUrls.Length == 0)
                {
                    throw new ArgumentException("At least one DiscoveryUrl must be provided.", "DiscoveryUrls");
                }

                if (application.ServerCapabilities == null || application.ServerCapabilities.Length == 0)
                {
                    throw new ArgumentException("At least one Server Capability must be provided.", "ServerCapabilities");
                }
            }
            else
            {
                if (application.DiscoveryUrls != null && application.DiscoveryUrls.Length > 0)
                {
                    throw new ArgumentException("DiscoveryUrls must not be specified for clients.", "DiscoveryUrls");
                }
            }

            // TODO check type
            //if (application.ApplicationId == Guid.Empty)
            //{
            //    throw new ArgumentException("The ApplicationId has invalid type {0}", application.ApplicationId.ToString());
            //}

            return application.ApplicationId;
        }

        public static string ServerCapabilities(Application application)
        {
            if (application.ApplicationType != (int)Opc.Ua.ApplicationType.Client)
            {
                if (application.ServerCapabilities == null || application.ServerCapabilities.Length == 0)
                {
                    throw new ArgumentException("At least one Server Capability must be provided.", "ServerCapabilities");
                }
            }

            StringBuilder capabilities = new StringBuilder();
            if (application.ServerCapabilities != null)
            {
                var sortedCaps = application.ServerCapabilities.Split(",").ToList();
                sortedCaps.Sort();
                foreach (var capability in sortedCaps)
                {
                    if (String.IsNullOrEmpty(capability))
                    {
                        continue;
                    }

                    if (capabilities.Length > 0)
                    {
                        capabilities.Append(',');
                    }

                    capabilities.Append(capability);
                }
            }

            return capabilities.ToString();
        }

#endregion
#region Private Fields
        private DateTime queryCounterResetTime = DateTime.UtcNow;
        private DocumentDBRepository db;
        private IDocumentDBCollection<Application> Applications;
        private IDocumentDBCollection<CertificateRequest> CertificateRequests;
        private IDocumentDBCollection<CertificateStore> CertificateStores;
#endregion
    }
}
