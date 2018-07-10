// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models;
using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.v1.Models
{
    public sealed class ReadRequestApiModel
    {
        [JsonProperty(PropertyName = "State", Order = 5)]
        public string State { get; set; }

        [JsonProperty(PropertyName = "ApplicationId", Order = 10)]
        public string ApplicationId { get; set; }

        [JsonProperty(PropertyName = "RequestId", Order = 15)]
        public string RequestId { get; set; }

        [JsonProperty(PropertyName = "CertificateGroupId", Order = 20)]
        public string CertificateGroupId { get; set; }

        [JsonProperty(PropertyName = "CertificateTypeId", Order = 30)]
        public string CertificateTypeId { get; set; }

        [JsonProperty(PropertyName = "CertificateRequest", Order = 35)]
        public string CertificateRequest { get; set; }

        [JsonProperty(PropertyName = "SubjectName", Order = 40)]
        public string SubjectName { get; set; }

        [JsonProperty(PropertyName = "DomainNames", Order = 50)]
        public string [] DomainNames { get; set; }

        [JsonProperty(PropertyName = "PrivateKeyFormat", Order = 60)]
        public string PrivateKeyFormat { get; set; }

        [JsonProperty(PropertyName = "PrivateKeyPassword", Order = 70)]
        public string PrivateKeyPassword { get; set; }


        public ReadRequestApiModel(
            CertificateRequestState state,
            string applicationId,
            string requestId,
            string certificateGroupId,
            string certificateTypeId,
            byte[] certificateRequest,
            string subjectName,
            string[] domainNames,
            string privateKeyFormat,
            string privateKeyPassword)
        {
            this.State = state.ToString();
            this.ApplicationId = applicationId;
            this.CertificateGroupId = certificateGroupId;
            this.CertificateTypeId = certificateTypeId;
            this.CertificateRequest = Convert.ToBase64String(certificateRequest); 
            this.SubjectName = subjectName;
            this.DomainNames = domainNames;
            this.PrivateKeyFormat = privateKeyFormat;
            this.PrivateKeyPassword = privateKeyPassword;
        }

    }
}
