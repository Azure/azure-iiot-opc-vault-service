// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.Client.Models
{
    public sealed class X509Certificate2ApiModel
    {
        [JsonProperty(PropertyName = "Subject", Order = 10)]
        public string Subject { get; set; }

        [JsonProperty(PropertyName = "Thumbprint", Order = 20)]
        public string Thumbprint { get; set; }

        [JsonProperty(PropertyName = "Certificate", Order = 20)]
        public string Certificate { get; set; }

    }
}
