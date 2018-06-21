// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.Client.Models
{
    public sealed class SigningRequestApiModel
    {
        [JsonProperty(PropertyName = "ApplicationURI", Order = 10)]
        public string ApplicationURI { get; set; }

        [JsonProperty(PropertyName = "Csr", Order = 20)]
        public string Csr { get; set; }

    }
}
