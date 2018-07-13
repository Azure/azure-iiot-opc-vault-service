// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models;
using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.v1.Models
{
    public sealed class CompleteRequestApiModel
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

        [JsonProperty(PropertyName = "SignedCertificate", Order = 40)]
        public string SignedCertificate { get; set; }

        [JsonProperty(PropertyName = "PrivateKey", Order = 50)]
        public string PrivateKey { get; set; }

        [JsonProperty(PropertyName = "AuthorityId", Order = 60)]
        public string AuthorityId { get; set; }

        public CompleteRequestApiModel(
            CertificateRequestState state,
            string applicationId,
            string requestId,
            string certificateGroupId,
            string certificateTypeId,
            byte[] signedCertificate,
            byte[] privateKey,
            string authorityId
)
        {
            this.State = state.ToString();
            this.ApplicationId = applicationId;
            this.CertificateGroupId = certificateGroupId;
            this.CertificateTypeId = certificateTypeId;
            this.SignedCertificate = (signedCertificate != null) ? Convert.ToBase64String(signedCertificate) : null;
            this.PrivateKey = (privateKey != null) ? Convert.ToBase64String(privateKey) : null;
            this.AuthorityId = authorityId;
        }

    }
}
