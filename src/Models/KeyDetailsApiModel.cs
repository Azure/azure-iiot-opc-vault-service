// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

namespace Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.Models {
    using Newtonsoft.Json;

    public class KeyDetailsApiModel {

        [JsonProperty(PropertyName = "EncodedBase64")]
        public string EncodedBase64 { get; set; }
    }
}
