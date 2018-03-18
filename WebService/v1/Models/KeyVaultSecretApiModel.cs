// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.v1.Models
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
