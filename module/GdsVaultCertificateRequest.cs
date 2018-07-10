// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

// TODO: remove cosmosdb direct access

using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api.Models;
using System;

namespace Opc.Ua.Gds.Server.GdsVault
{
    public class GdsVaultCertificateRequest : ICertificateRequest
    {
        private IOpcGdsVault _gdsVaultServiceClient { get; }
        public GdsVaultCertificateRequest(IOpcGdsVault gdsVaultServiceClient)
        {
            this._gdsVaultServiceClient = gdsVaultServiceClient;
        }

        #region ICertificateRequest
        public void Initialize()
        {

        }
        public ushort NamespaceIndex { get; set; }

        public NodeId CreateSigningRequest(
            NodeId applicationId,
            NodeId certificateGroupId,
            NodeId certificateTypeId,
            byte[] certificateRequest,
            string authorityId)
        {
            string id = GdsVaultClientHelper.GetIdentifierStringFromNodeId(applicationId, NamespaceIndex);

            var model = new CreateSigningRequestApiModel(
                id,
                certificateGroupId.ToString(),
                certificateTypeId.ToString(),
                Convert.ToBase64String(certificateRequest),
                authorityId
                );

            string requestId = _gdsVaultServiceClient.CreateSigningRequest(model);

            return GdsVaultClientHelper.GetNodeIdFromIdentifierString(requestId, NamespaceIndex);
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
            throw new NotImplementedException();
#if TODO

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
            request.CertificateGroupId = certificateGroupId.ToString();
            request.CertificateTypeId = certificateTypeId.ToString();
            request.SubjectName = subjectName;
            request.DomainNames = domainNames;
            request.PrivateKeyFormat = privateKeyFormat;
            request.PrivateKeyPassword = privateKeyPassword;
            request.CertificateSigningRequest = null;
            request.ApplicationId = id.ToString();
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
#endif
        }

        public void ApproveCertificateRequest(
            NodeId requestId,
            bool isRejected
            )
        {
            throw new NotImplementedException();
#if TODO

            Guid id = GetNodeIdGuid(requestId);

            var request = CertificateRequests.GetAsync(id).Result;
            if (request == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            if (request.State != Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models.CertificateRequestState.New)
            {
                throw new ServiceResultException(StatusCodes.BadInvalidState);
            }

            if (isRejected)
            {
                request.State = Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models.CertificateRequestState.Rejected;
                // erase information which is not required anymore
                request.PrivateKeyFormat = null;
                request.CertificateSigningRequest = null;
                request.PrivateKeyPassword = null;
            }
            else
            {
                request.State = Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models.CertificateRequestState.Approved;
            }

            request.ApproveRejectTime = DateTime.UtcNow;

            CertificateRequests.UpdateAsync(request.RequestId, request).Wait();
#endif
        }

        public void AcceptCertificateRequest(NodeId requestId, byte[] signedCertificate)
        {
            throw new NotImplementedException();
#if TODO

            Guid id = GetNodeIdGuid(requestId);

            var request = CertificateRequests.GetAsync(id).Result;
            if (request == null)
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            if (request.State != Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models.CertificateRequestState.Approved)
            {
                throw new ServiceResultException(StatusCodes.BadInvalidState);
            }

            request.State = Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models.CertificateRequestState.Accepted;

            // erase information which is not required anymore
            request.CertificateSigningRequest = null;
            request.PrivateKeyFormat = null;
            request.PrivateKeyPassword = null;
            request.AcceptTime = DateTime.UtcNow;
            request.Certificate = signedCertificate;

            CertificateRequests.UpdateAsync(request.RequestId, request).Wait();
#endif
        }

        public CertificateRequestState CompleteCertificateRequest(
            NodeId applicationId,
            NodeId requestId,
            out NodeId certificateGroupId,
            out NodeId certificateTypeId,
            out byte[] signedCertificate,
            out byte[] privateKey)
        {
            throw new NotImplementedException();
#if TODO

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
                case Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models.CertificateRequestState.New:
                    return CertificateRequestState.New;
                case Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models.CertificateRequestState.Rejected:
                    return CertificateRequestState.Rejected;
                case Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models.CertificateRequestState.Accepted:
                    return CertificateRequestState.Accepted;
                case Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models.CertificateRequestState.Approved:
                    break;
                default:
                    throw new ServiceResultException(StatusCodes.BadInvalidArgument);
            }

            if (request.ApplicationId != appId.ToString())
            {
                throw new ServiceResultException(StatusCodes.BadNodeIdUnknown);
            }

            certificateGroupId = request.CertificateGroupId;
            certificateTypeId = request.CertificateTypeId;
            signedCertificate = request.Certificate;
            privateKey = request.PrivateKey;

            return CertificateRequestState.Approved;
#endif
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
            throw new NotImplementedException();
#if TODO
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
                case Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models.CertificateRequestState.New:
                    return CertificateRequestState.New;
                case Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models.CertificateRequestState.Rejected:
                    return CertificateRequestState.Rejected;
                case Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models.CertificateRequestState.Accepted:
                    return CertificateRequestState.Accepted;
                case Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models.CertificateRequestState.Approved:
                    break;
                default:
                    throw new ServiceResultException(StatusCodes.BadInvalidArgument);
            }

            if (request.ApplicationId != appId.ToString())
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
#endif
        }
#endregion
    }
}
