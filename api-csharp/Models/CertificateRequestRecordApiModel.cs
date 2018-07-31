// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator 2.4.43.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class CertificateRequestRecordApiModel
    {
        /// <summary>
        /// Initializes a new instance of the CertificateRequestRecordApiModel
        /// class.
        /// </summary>
        public CertificateRequestRecordApiModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CertificateRequestRecordApiModel
        /// class.
        /// </summary>
        public CertificateRequestRecordApiModel(string requestId = default(string), string applicationId = default(string), string state = default(string), string certificateGroupId = default(string), string certificateTypeId = default(string), string signingRequest = default(string), string subjectName = default(string), IList<string> domainNames = default(IList<string>), string privateKeyFormat = default(string))
        {
            RequestId = requestId;
            ApplicationId = applicationId;
            State = state;
            CertificateGroupId = certificateGroupId;
            CertificateTypeId = certificateTypeId;
            SigningRequest = signingRequest;
            SubjectName = subjectName;
            DomainNames = domainNames;
            PrivateKeyFormat = privateKeyFormat;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "RequestId")]
        public string RequestId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ApplicationId")]
        public string ApplicationId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "State")]
        public string State { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "CertificateGroupId")]
        public string CertificateGroupId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "CertificateTypeId")]
        public string CertificateTypeId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "SigningRequest")]
        public string SigningRequest { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "SubjectName")]
        public string SubjectName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "DomainNames")]
        public IList<string> DomainNames { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "PrivateKeyFormat")]
        public string PrivateKeyFormat { get; set; }

    }
}
