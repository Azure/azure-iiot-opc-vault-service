// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

namespace Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.Models {
    using Newtonsoft.Json;
    using System;

    public class CertificateDetailsApiModel {
        [JsonProperty(PropertyName = "Subject")]
        public string Subject { get; set; }

        [JsonProperty(PropertyName = "Issuer")]
        public string Issuer { get; set; }

        [JsonProperty(PropertyName = "Thumbprint")]
        public string Thumbprint { get; set; }

        [JsonProperty(PropertyName = "SerialNumber")]
        public string SerialNumber { get; set; }

        [JsonProperty(PropertyName = "NotBefore")]
        public DateTime NotBefore { get; set; }

        [JsonProperty(PropertyName = "NotAfter")]
        public DateTime NotAfter { get; set; }

        [JsonProperty(PropertyName = "EncodedBase64")]
        public string EncodedBase64 { get; set; }

    }
}
