﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------



using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.CosmosDB.Models;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Models
{
    public sealed class ReadRequestResultModel
    {
        public string RequestId { get; set; }

        public string ApplicationId { get; set; }

        public CertificateRequestState State { get; set; }

        public string CertificateGroupId { get; set; }

        public string CertificateTypeId { get; set; }

        public byte[] SigningRequest { get; set; }

        public string SubjectName { get; set; }
        public string[] DomainNames { get; set; }
        public string PrivateKeyFormat { get; set; }
        public string PrivateKeyPassword { get; set; }

        public ReadRequestResultModel(
                CertificateRequest request
                )
        {
            this.RequestId = request.RequestId.ToString();
            this.ApplicationId = request.ApplicationId;
            this.State = request.State;
            this.CertificateGroupId = request.CertificateGroupId;
            this.CertificateTypeId = request.CertificateTypeId;
            this.SigningRequest = request.SigningRequest;
            this.SubjectName = request.SubjectName;
            this.DomainNames = request.DomainNames;
            this.PrivateKeyFormat = request.PrivateKeyFormat;
            this.PrivateKeyPassword = request.PrivateKeyPassword;
        }

        public ReadRequestResultModel(
                string requestId,
                string applicationId,
                CertificateRequestState state,
                string certificateGroupId,
                string certificateTypeId,
                byte[] certificateRequest,
                string subjectName,
                string[] domainNames,
                string privateKeyFormat,
                string privateKeyPassword
                )
        {
            this.RequestId = requestId;
            this.ApplicationId = applicationId;
            this.State = state;
            this.CertificateGroupId = certificateGroupId;
            this.CertificateTypeId = certificateTypeId;
            this.SigningRequest = certificateRequest;
            this.SubjectName = subjectName;
            this.DomainNames = domainNames;
            this.PrivateKeyFormat = privateKeyFormat;
            this.PrivateKeyPassword = privateKeyPassword;
        }
    }
}

