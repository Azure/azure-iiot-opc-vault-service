// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.v1.Models
{
    using Newtonsoft.Json;

    public sealed class KeyVaultSecretApiModel
    {
        [JsonProperty(PropertyName = "Secret", Order = 10)]
        public string Secret { get; set; }

        public KeyVaultSecretApiModel(string secret)
        {
            this.Secret = secret;
        }
    }
}
