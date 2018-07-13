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
    using System.Linq;

    public partial class CertificateKeyPairApiModel
    {
        /// <summary>
        /// Initializes a new instance of the CertificateKeyPairApiModel class.
        /// </summary>
        public CertificateKeyPairApiModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CertificateKeyPairApiModel class.
        /// </summary>
        public CertificateKeyPairApiModel(string subject = default(string), string thumbprint = default(string), string certificate = default(string), string privateKeyFormat = default(string), string privateKey = default(string))
        {
            Subject = subject;
            Thumbprint = thumbprint;
            Certificate = certificate;
            PrivateKeyFormat = privateKeyFormat;
            PrivateKey = privateKey;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Subject")]
        public string Subject { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Thumbprint")]
        public string Thumbprint { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Certificate")]
        public string Certificate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "PrivateKeyFormat")]
        public string PrivateKeyFormat { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "PrivateKey")]
        public string PrivateKey { get; set; }

    }
}
