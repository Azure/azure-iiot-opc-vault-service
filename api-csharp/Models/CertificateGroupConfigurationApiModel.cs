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

    public partial class CertificateGroupConfigurationApiModel
    {
        /// <summary>
        /// Initializes a new instance of the
        /// CertificateGroupConfigurationApiModel class.
        /// </summary>
        public CertificateGroupConfigurationApiModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// CertificateGroupConfigurationApiModel class.
        /// </summary>
        public CertificateGroupConfigurationApiModel(string name = default(string), string certificateType = default(string), string subjectName = default(string), int? defaultCertificateLifetime = default(int?), int? defaultCertificateKeySize = default(int?), int? defaultCertificateHashSize = default(int?), int? caCertificateLifetime = default(int?), int? caCertificateKeySize = default(int?), int? caCertificateHashSize = default(int?))
        {
            Name = name;
            CertificateType = certificateType;
            SubjectName = subjectName;
            DefaultCertificateLifetime = defaultCertificateLifetime;
            DefaultCertificateKeySize = defaultCertificateKeySize;
            DefaultCertificateHashSize = defaultCertificateHashSize;
            CaCertificateLifetime = caCertificateLifetime;
            CaCertificateKeySize = caCertificateKeySize;
            CaCertificateHashSize = caCertificateHashSize;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "certificateType")]
        public string CertificateType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "subjectName")]
        public string SubjectName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "defaultCertificateLifetime")]
        public int? DefaultCertificateLifetime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "defaultCertificateKeySize")]
        public int? DefaultCertificateKeySize { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "defaultCertificateHashSize")]
        public int? DefaultCertificateHashSize { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "caCertificateLifetime")]
        public int? CaCertificateLifetime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "caCertificateKeySize")]
        public int? CaCertificateKeySize { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "caCertificateHashSize")]
        public int? CaCertificateHashSize { get; set; }

    }
}
