// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

namespace Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.Models {
    using Newtonsoft.Json;
    using System;

    public class CrlDetailsApiModel {
        [JsonProperty(PropertyName = "Issuer")]
        public string Issuer { get; set; }

        [JsonProperty(PropertyName = "UpdateTime")]
        public DateTime UpdateTime { get; set; }

        [JsonProperty(PropertyName = "NextUpdateTime")]
        public DateTime NextUpdateTime { get; set; }

        [JsonProperty(PropertyName = "EncodedBase64")]
        public string EncodedBase64 { get; set; }
    }
}
