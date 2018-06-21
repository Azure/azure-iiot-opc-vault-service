// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.Client.Models
{
    public sealed class X509Certificate2CollectionApiModel
    {
        [JsonProperty(PropertyName = "Chain", Order = 10)]
        public X509Certificate2ApiModel[] Chain { get; set; }
    }
}
