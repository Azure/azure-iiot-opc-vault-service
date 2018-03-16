// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.Client.Models
{
    public sealed class NewKeyPairRequestApiModel
    {
        [JsonProperty(PropertyName = "ApplicationURI", Order = 10)]
        public string ApplicationURI { get; set; }

        [JsonProperty(PropertyName = "SubjectName", Order = 20)]
        public string SubjectName { get; set; }

        [JsonProperty(PropertyName = "DomainNames", Order = 30)]
        public string [] DomainNames { get; set; }

        [JsonProperty(PropertyName = "PrivateKeyFormat", Order = 40)]
        public string PrivateKeyFormat { get; set; }

        [JsonProperty(PropertyName = "PrivateKeyPassword", Order = 50)]
        public string PrivateKeyPassword { get; set; }

    }
}
