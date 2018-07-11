// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

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
            string id = GdsVaultClientHelper.GetIdentifierStringFromNodeId(applicationId, NamespaceIndex);

            var model = new CreateNewKeyPairRequestApiModel(
                id,
                certificateGroupId.ToString(),
                certificateTypeId.ToString(),
                subjectName,
                domainNames,
                privateKeyFormat,
                privateKeyPassword,
                authorityId
                );

            string requestId = _gdsVaultServiceClient.CreateNewKeyPairRequest(model);

            return GdsVaultClientHelper.GetNodeIdFromIdentifierString(requestId, NamespaceIndex);
        }

        public void ApproveCertificateRequest(
            NodeId requestId,
            bool isRejected
            )
        {
            // intentionally ignore the auto approval, it is implemented in the GdsVault service
        }

        public void AcceptCertificateRequest(NodeId requestId, byte[] signedCertificate)
        {
            string reqId = GdsVaultClientHelper.GetIdentifierStringFromNodeId(requestId, NamespaceIndex);
            _gdsVaultServiceClient.AcceptCertificateRequest(reqId);
        }

        public CertificateRequestState CompleteCertificateRequest(
            NodeId applicationId,
            NodeId requestId,
            out NodeId certificateGroupId,
            out NodeId certificateTypeId,
            out byte[] signedCertificate,
            out byte[] privateKey)
        {
            string reqId = GdsVaultClientHelper.GetIdentifierStringFromNodeId(requestId, NamespaceIndex);
            string appId = GdsVaultClientHelper.GetIdentifierStringFromNodeId(applicationId, NamespaceIndex);

            certificateGroupId = null;
            certificateTypeId = null;
            signedCertificate = null;
            privateKey = null;

            var request = _gdsVaultServiceClient.CompleteCertificateRequest(reqId, appId);

            var state = (CertificateRequestState)Enum.Parse(typeof(CertificateRequestState), request.State);

            if (state == CertificateRequestState.Approved)
            {
                certificateGroupId = request.CertificateGroupId;
                certificateTypeId = request.CertificateTypeId;
                signedCertificate = Convert.FromBase64String(request.SignedCertificate);
                privateKey = Convert.FromBase64String(request.PrivateKey);
            }

            return state;
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
        }
#endregion
    }
}
