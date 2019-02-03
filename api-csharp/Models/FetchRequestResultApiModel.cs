// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.IIoT.OpcUa.Api.Vault.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class FetchRequestResultApiModel
    {
        /// <summary>
        /// Initializes a new instance of the FetchRequestResultApiModel class.
        /// </summary>
        public FetchRequestResultApiModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the FetchRequestResultApiModel class.
        /// </summary>
        /// <param name="state">Possible values include: 'new', 'approved',
        /// 'rejected', 'accepted', 'deleted', 'revoked'</param>
        public FetchRequestResultApiModel(CertificateRequestState state, string requestId = default(string), string applicationId = default(string), string certificateGroupId = default(string), string certificateTypeId = default(string), string signedCertificate = default(string), string privateKeyFormat = default(string), string privateKey = default(string), string authorityId = default(string))
        {
            RequestId = requestId;
            ApplicationId = applicationId;
            State = state;
            CertificateGroupId = certificateGroupId;
            CertificateTypeId = certificateTypeId;
            SignedCertificate = signedCertificate;
            PrivateKeyFormat = privateKeyFormat;
            PrivateKey = privateKey;
            AuthorityId = authorityId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "requestId")]
        public string RequestId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "applicationId")]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'new', 'approved',
        /// 'rejected', 'accepted', 'deleted', 'revoked'
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        public CertificateRequestState State { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "certificateGroupId")]
        public string CertificateGroupId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "certificateTypeId")]
        public string CertificateTypeId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "signedCertificate")]
        public string SignedCertificate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "privateKeyFormat")]
        public string PrivateKeyFormat { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "privateKey")]
        public string PrivateKey { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "authorityId")]
        public string AuthorityId { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
        }
    }
}
