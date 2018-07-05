// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.CosmosDB;
using Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace Opc.Ua.Gds.Server.Database.CosmosDB
{
    public class CosmosDBApplicationsDatabase : ApplicationsDatabaseBase, ICertificateRequest
    {
        private readonly string Endpoint;
        private SecureString AuthKeyOrResourceToken;

        public CosmosDBApplicationsDatabase(string endpoint, string authKeyOrResourceToken)
        {
            this.Endpoint = endpoint;
            this.AuthKeyOrResourceToken = new SecureString();
            foreach (char ch in authKeyOrResourceToken)
            {
                this.AuthKeyOrResourceToken.AppendChar(ch);
            }
        }

        public CosmosDBApplicationsDatabase(string endpoint, SecureString authKeyOrResourceToken)
        {
            this.Endpoint = endpoint;
            this.AuthKeyOrResourceToken = authKeyOrResourceToken;
        }

        #region IApplicationsDatabase 
        public override void Initialize()
        {
            db = new DocumentDBRepository(Endpoint, AuthKeyOrResourceToken);
            Applications = new DocumentDBCollection<Application>(db);
            CertificateRequests = new DocumentDBCollection<CertificateRequest>(db);
            CertificateStores = new DocumentDBCollection<CertificateStore>(db);
        }

        public override NodeId RegisterApplication(
                ApplicationRecordDataType application
                )
        {
            NodeId appNodeId = base.RegisterApplication(application);
            if (NodeId.IsNull(appNodeId))
            {
                appNodeId = new NodeId(Guid.NewGuid(), NamespaceIndex);
            }

            Guid applicationId = GetNodeIdGuid(appNodeId);
            string capabilities = base.ServerCapabilities(application);

            if (applicationId != Guid.Empty)
            {
                bool isNew = false;
                var record = Applications.GetAsync(applicationId).Result;
                if (record == null)
                {
                    applicationId = Guid.NewGuid();
                    record = new Application()
                    {
                        ApplicationId = applicationId
                    };
                    isNew = true;
                }

                record.ApplicationUri = application.ApplicationUri;
                record.ApplicationName = application.ApplicationNames[0].Text;
                record.ApplicationType = (int)application.ApplicationType;
                record.ProductUri = application.ProductUri;
                record.ServerCapabilities = capabilities;

                if (application.DiscoveryUrls != null)
                {
                    record.DiscoveryUrls = application.DiscoveryUrls.ToArray();
                }

                if (application.ApplicationNames != null && application.ApplicationNames.Count > 0)
                {
                    var applicationNames = new List<ApplicationName>();
                    foreach (var applicationName in application.ApplicationNames)
                    {
                        applicationNames.Add(new ApplicationName()
                        {
                            Locale = applicationName.Locale,
                            Text = applicationName.Text
                        });
                    }
                    record.ApplicationNames = applicationNames.ToArray();
                }

                if (isNew)
                {
                    // find new ID for QueryServers
                    var maxAppID = Applications.GetAsync("SELECT TOP 1 * FROM Applications a ORDER BY a.ID DESC").Result.SingleOrDefault();
                    record.ID = (maxAppID != null) ? maxAppID.ID + 1 : 1;
                    Applications.CreateAsync(record).Wait();
                }
                else
                {
                    Applications.UpdateAsync(record.ApplicationId, record).Wait();
                }
            }
            return new NodeId(applicationId, NamespaceIndex);
        }


        public override void UnregisterApplication(
            NodeId applicationId,
            out byte[] certificate,
            out byte[] httpsCertificate)
        {
            certificate = null;
            httpsCertificate = null;

            Guid id = GetNodeIdGuid(applicationId);

            List<byte[]> certificates = new List<byte[]>();

            var application = Applications.GetAsync(id).Result;
            if (application == null)
            {
                throw new ArgumentException("A record with the specified application id does not exist.", nameof(applicationId));
            }

            certificate = application.Certificate;
            httpsCertificate = application.HttpsCertificate;

            var certificateRequests = CertificateRequests.GetAsync(ii => ii.ApplicationId == id).Result;
            foreach (var entry in new List<CertificateRequest>(certificateRequests))
            {
                CertificateRequests.DeleteAsync(entry.RequestId).Wait();
            }

            Applications.DeleteAsync(application.ApplicationId).Wait();

        }

        public override ApplicationRecordDataType GetApplication(
            NodeId applicationId
            )
        {
            Guid id = GetNodeIdGuid(applicationId);

            var result = Applications.GetAsync(id).Result;
            if (result == null)
            {
                return null;
            }

            var names = new List<LocalizedText>();
            foreach (var applicationName in result.ApplicationNames)
            {
                names.Add(new LocalizedText(applicationName.Locale, applicationName.Text));
            }

            StringCollection discoveryUrls = null;

            var endpoints = result.DiscoveryUrls;
            if (endpoints != null)
            {
                discoveryUrls = new StringCollection();

                foreach (var endpoint in endpoints)
                {
                    discoveryUrls.Add(endpoint);
                }
            }

            var capabilities = new StringCollection();
            if (!String.IsNullOrWhiteSpace(result.ServerCapabilities))
            {
                capabilities.AddRange(result.ServerCapabilities.Split(','));
            }

            return new ApplicationRecordDataType()
            {
                ApplicationId = new NodeId(result.ApplicationId, NamespaceIndex),
                ApplicationUri = result.ApplicationUri,
                ApplicationType = (ApplicationType)result.ApplicationType,
                ApplicationNames = new LocalizedTextCollection(names),
                ProductUri = result.ProductUri,
                DiscoveryUrls = discoveryUrls,
                ServerCapabilities = capabilities
            };
        }

        public override ApplicationRecordDataType[] FindApplications(
            string applicationUri
            )
        {
            var results = Applications.GetAsync(ii => ii.ApplicationUri == applicationUri).Result;

            List<ApplicationRecordDataType> records = new List<ApplicationRecordDataType>();

            foreach (var result in results)
            {
                LocalizedText[] names = null;

                if (result.ApplicationName != null)
                {
                    names = new LocalizedText[] { result.ApplicationName };
                }

                StringCollection discoveryUrls = null;

                var endpoints = result.DiscoveryUrls;
                if (endpoints != null)
                {
                    discoveryUrls = new StringCollection();

                    foreach (var endpoint in endpoints)
                    {
                        discoveryUrls.Add(endpoint);
                    }
                }

                string[] capabilities = null;

                if (result.ServerCapabilities != null)
                {
                    capabilities = result.ServerCapabilities.Split(',');
                }

                records.Add(new ApplicationRecordDataType()
                {
                    ApplicationId = new NodeId(result.ApplicationId, NamespaceIndex),
                    ApplicationUri = result.ApplicationUri,
                    ApplicationType = (ApplicationType)result.ApplicationType,
                    ApplicationNames = new LocalizedTextCollection(names),
                    ProductUri = result.ProductUri,
                    DiscoveryUrls = discoveryUrls,
                    ServerCapabilities = capabilities
                });
            }

            return records.ToArray();
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

        public override ServerOnNetwork[] QueryServers(
            uint startingRecordId,
            uint maxRecordsToReturn,
            string applicationName,
            string applicationUri,
            string productUri,
            string[] serverCapabilities,
            out DateTime lastCounterResetTime)
        {
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
        }

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
        #endregion
        #region ICertificateRequest
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
        #endregion
        #region Public Members
        public virtual void Save()
        {
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
