// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.Client.Models
{
    public sealed class CertificateKeyPairApiModel
    {
        [JsonProperty(PropertyName = "Subject", Order = 10)]
        public string Subject { get; set; }

        [JsonProperty(PropertyName = "Thumbprint", Order = 20)]
        public string Thumbprint { get; set; }

        [JsonProperty(PropertyName = "Certificate", Order = 30)]
        public string Certificate { get; set; }

        [JsonProperty(PropertyName = "PrivateKeyFormat", Order = 40)]
        public string PrivateKeyFormat { get; set; }

        [JsonProperty(PropertyName = "PrivateKey", Order = 50)]
        public string PrivateKey { get; set; }

    }
}
