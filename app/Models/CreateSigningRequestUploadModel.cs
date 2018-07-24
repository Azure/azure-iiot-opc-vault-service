// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

using Microsoft.AspNetCore.Http;
using System;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class CreateSigningRequestUploadModel
    {
        /// <summary>
        /// Initializes a new instance of the CreateSigningRequestApiModel
        /// class.
        /// </summary>
        public CreateSigningRequestUploadModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CreateSigningRequestApiModel
        /// class.
        /// </summary>
        public CreateSigningRequestUploadModel(string applicationId = default(string), string certificateGroupId = default(string), string certificateTypeId = default(string), string certificateRequest = default(string), string authorityId = default(string))
        {
            ApplicationId = applicationId;
            CertificateGroupId = certificateGroupId;
            CertificateTypeId = certificateTypeId;
            CertificateRequest = certificateRequest;
            AuthorityId = authorityId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ApplicationId")]
        public string ApplicationId { get; set; }

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
        [JsonProperty(PropertyName = "CertificateRequest")]
        public string CertificateRequest { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "AuthorityId")]
        public string AuthorityId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "CertificateRequestFile")]
        public IFormFile CertificateRequestFile { get; set; }

    }
}
