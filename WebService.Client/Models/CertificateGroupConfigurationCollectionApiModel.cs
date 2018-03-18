// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.Client.Models
{
    public sealed class CertificateGroupConfigurationCollectionApiModel
    {
        [JsonProperty(PropertyName = "Groups", Order = 10)]
        public CertificateGroupConfigurationApiModel[] Groups { get; set; }
    }
}
