// Copyright (c) Microsoft. All rights reserved.


namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.Client.Models
{
    using Newtonsoft.Json;

    public sealed class KeyVaultSecretApiModel
    {
        [JsonProperty(PropertyName = "Secret", Order = 10)]
        public string Secret { get; set; }
    }
}
