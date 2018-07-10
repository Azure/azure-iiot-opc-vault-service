// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.Azure.IIoT.Diagnostics;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.CosmosDB;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Models;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Runtime;
using Opc.Ua;
using System;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault
{
    internal sealed class CosmosDBCertificateRequest : ICertificateRequest
    {
        private readonly ILogger _log;
        private readonly IApplicationsDatabase _database;
        private readonly ICertificateGroup _group;
        private readonly string _endpoint;
        private SecureString _authKeyOrResourceToken;

        public CosmosDBCertificateRequest(
            IApplicationsDatabase database,
            IServicesConfig config,
            ILogger logger)
        {
            this._database = database;
            this._endpoint = config.CosmosDBEndpoint;
            this._authKeyOrResourceToken = new SecureString();
            foreach (char ch in config.CosmosDBToken)
            {
                this._authKeyOrResourceToken.AppendChar(ch);
            }
            _log = logger;
            _log.Debug("Creating new instance of `CosmosDBApplicationsDatabase` service " + config.CosmosDBEndpoint, () => { });
            Initialize();
        }


        #region ICertificateRequest

        public Task Initialize()
        {
            db = new DocumentDBRepository(_endpoint, _authKeyOrResourceToken);
            Applications = new DocumentDBCollection<Application>(db);
            CertificateRequests = new DocumentDBCollection<CertificateRequest>(db);
            CertificateStores = new DocumentDBCollection<CertificateStore>(db);
            return Task.CompletedTask;
        }
        public async Task<string> CreateSigningAsync(
            string applicationId,
            string certificateGroupId,
            string certificateTypeId,
            byte[] certificateRequest,
            string authorityId)
        {
            Guid appId = GetIdFromString(applicationId);

            // TODO: use IApplicationsDatabase
            var application = await Applications.GetAsync(appId);
            if (application == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            CertificateRequest request = null;
            bool isNew = false;

            // TODO: do we need updates?

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
            request.ApplicationId = applicationId;
            request.RequestTime = DateTime.UtcNow;

            if (isNew)
            {
                CertificateRequests.CreateAsync(request).Wait();
            }
            else
            {
                CertificateRequests.UpdateAsync(request.RequestId, request).Wait();
            }

            return request.RequestId.ToString();
        }

        public async Task<string> CreateNewKeyPairAsync(
            string applicationId,
            string certificateGroupId,
            string certificateTypeId,
            string subjectName,
            string[] domainNames,
            string privateKeyFormat,
            string privateKeyPassword,
            string authorityId)
        {
            Guid appId = GetIdFromString(applicationId);

            var application = await Applications.GetAsync(appId);
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
            request.ApplicationId = appId.ToString();
            request.RequestTime = DateTime.UtcNow;

            if (isNew)
            {
                await CertificateRequests.CreateAsync(request);
            }
            else
            {
                await CertificateRequests.UpdateAsync(request.RequestId, request);
            }

            return request.RequestId.ToString();
        }

        public async Task ApproveAsync(
            string requestId,
            bool isRejected
            )
        {
            Guid reqId = GetIdFromString(requestId);

            var request = await CertificateRequests.GetAsync(reqId);
            if (request == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            if (request.State != CertificateRequestState.New)
            {
                throw new ServiceResultException(StatusCodes.BadInvalidState);
            }

            if (isRejected)
            {
                request.State = CertificateRequestState.Rejected;
                // erase information which is not required anymore
                request.PrivateKeyFormat = null;
                request.CertificateSigningRequest = null;
                request.PrivateKeyPassword = null;
            }
            else
            {
                request.State = CertificateRequestState.Approved;
            }

            request.ApproveRejectTime = DateTime.UtcNow;

            await CertificateRequests.UpdateAsync(reqId, request);
        }

        public async Task AcceptAsync(
            string requestId)
        {
            Guid reqId = GetIdFromString(requestId);

            var request = await CertificateRequests.GetAsync(reqId);
            if (request == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            if (request.State != CertificateRequestState.Approved)
            {
                throw new ServiceResultException(StatusCodes.BadInvalidState);
            }

            request.State = CertificateRequestState.Accepted;

            // erase information which is not required anymore
            request.CertificateSigningRequest = null;
            request.PrivateKeyFormat = null;
            request.PrivateKeyPassword = null;
            request.AcceptTime = DateTime.UtcNow;

            await CertificateRequests.UpdateAsync(request.RequestId, request);
        }

        public async Task<CompleteRequestResultModel> CompleteAsync(
            string applicationId,
            string requestId)
        {
            Guid reqId = GetIdFromString(requestId);
            Guid appId = GetIdFromString(applicationId);

            var application = await Applications.GetAsync(appId);
            if (application == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            var request = await CertificateRequests.GetAsync(reqId);
            if (request == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            switch (request.State)
            {
                case CertificateRequestState.New:
                case CertificateRequestState.Rejected:
                case CertificateRequestState.Accepted:
                    return new CompleteRequestResultModel(request.State);
                case CertificateRequestState.Approved:
                    break;
                default:
                    throw new ServiceResultException(StatusCodes.BadInvalidArgument);
            }

            if (request.ApplicationId != appId.ToString())
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            return new CompleteRequestResultModel(
                request.State,
                applicationId,
                requestId,
                request.CertificateGroupId,
                request.CertificateTypeId,
                request.Certificate,
                request.PrivateKey);
        }
        public async Task<ReadRequestResultModel> ReadAsync(
            string requestId
            )
        {
            Guid reqId = GetIdFromString(requestId);

            var request = await CertificateRequests.GetAsync(reqId);
            if (request == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            switch (request.State)
            {
                case CertificateRequestState.New:
                case CertificateRequestState.Rejected:
                case CertificateRequestState.Accepted:
                case CertificateRequestState.Approved:
                    break;
                default:
                    throw new ServiceResultException(StatusCodes.BadInvalidArgument);
            }

            return new ReadRequestResultModel(
                request.State,
                request.ApplicationId,
                requestId,
                request.CertificateGroupId,
            request.CertificateTypeId,
            request.CertificateSigningRequest,
            request.SubjectName,
            request.DomainNames,
             request.PrivateKeyFormat,
             request.PrivateKeyPassword);

        }
        #endregion

        #region Private Members
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

        private Guid GetIdFromString(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            Guid guidId = new Guid(id);

            if (guidId == null || guidId == Guid.Empty)
            {
                throw new ArgumentException("The id must be provided.", nameof(id));
            }

            if (id == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            return guidId;
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
