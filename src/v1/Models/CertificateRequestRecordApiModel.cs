// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Models
{
    public enum CertificateRequestState
    {
        [EnumMember(Value = "New")]
        New = 0,
        [EnumMember(Value = "Approved")]
        Approved = 1,
        [EnumMember(Value = "Rejected")]
        Rejected = 2,
        [EnumMember(Value = "Accepted")]
        Accepted = 3,
        [EnumMember(Value = "Deleted")]
        Deleted = 4,
        [EnumMember(Value = "Revoked")]
        Revoked = 5
    }

    public sealed class CertificateRequestRecordApiModel
    {
        [JsonProperty(PropertyName = "requestId", Order = 5)]
        public string RequestId { get; set; }

        [JsonProperty(PropertyName = "applicationId", Order = 10)]
        public string ApplicationId { get; set; }

        [JsonProperty(PropertyName = "state", Order = 15)]
        public string State { get; set; }

        [JsonProperty(PropertyName = "certificateGroupId", Order = 20)]
        public string CertificateGroupId { get; set; }

        [JsonProperty(PropertyName = "certificateTypeId", Order = 30)]
        public string CertificateTypeId { get; set; }

        [JsonProperty(PropertyName = "signingRequest", Order = 35)]
        public bool SigningRequest { get; set; }

        [JsonProperty(PropertyName = "subjectName", Order = 40)]
        public string SubjectName { get; set; }

        [JsonProperty(PropertyName = "domainNames", Order = 50)]
        public IList<string> DomainNames { get; set; }

        [JsonProperty(PropertyName = "privateKeyFormat", Order = 60)]
        public string PrivateKeyFormat { get; set; }

        public CertificateRequestRecordApiModel(
            string requestId,
            string applicationId,
            CosmosDB.Models.CertificateRequestState state,
            string certificateGroupId,
            string certificateTypeId,
            bool signingRequest,
            string subjectName,
            IList<string> domainNames,
            string privateKeyFormat)
        {
            this.RequestId = requestId;
            this.ApplicationId = applicationId;
            this.State = state.ToString();
            this.CertificateGroupId = certificateGroupId;
            this.CertificateTypeId = certificateTypeId;
            this.SigningRequest = signingRequest;
            this.SubjectName = subjectName;
            this.DomainNames = domainNames;
            this.PrivateKeyFormat = privateKeyFormat;
        }

    }
}
