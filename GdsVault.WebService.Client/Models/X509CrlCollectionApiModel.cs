// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.Client.Models
{
    public sealed class X509CrlCollectionApiModel
    {
        [JsonProperty(PropertyName = "Chain", Order = 10)]
        public X509CrlApiModel[] Chain { get; set; }
    }
}
