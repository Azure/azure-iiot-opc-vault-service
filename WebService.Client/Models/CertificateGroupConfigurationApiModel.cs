// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.Client.Models
{
    public sealed class CertificateGroupConfigurationApiModel
    {
        [JsonProperty(PropertyName = "Name", Order = 10)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "SubjectName", Order = 20)]
        public string SubjectName { get; set; }

        [JsonProperty(PropertyName = "DefaultCertificateLifetime", Order = 30)]
        public ushort DefaultCertificateLifetime { get; set; }

        [JsonProperty(PropertyName = "DefaultCertificateKeySize", Order = 40)]
        public ushort DefaultCertificateKeySize { get; set; }

        [JsonProperty(PropertyName = "DefaultCertificateHashSize", Order = 50)]
        public ushort DefaultCertificateHashSize { get; set; }
    }
}
