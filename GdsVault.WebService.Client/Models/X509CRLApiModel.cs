// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.Client.Models
{
    public sealed class X509CrlApiModel
    {
        [JsonProperty(PropertyName = "Issuer", Order = 10)]
        public string Issuer { get; set; }

        [JsonProperty(PropertyName = "Crl", Order = 20)]
        public string Crl { get; set; }

    }
}
