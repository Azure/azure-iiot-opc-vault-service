// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Models
{
    public sealed class CertificateGroupConfigurationApiModel
    {
        [JsonProperty(PropertyName = "Name", Order = 10)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "CertificateGroupConfiguration", Order = 20)]
        public Opc.Ua.Gds.Server.CertificateGroupConfiguration Config { get; set; }

        public CertificateGroupConfigurationApiModel(string id, Opc.Ua.Gds.Server.CertificateGroupConfiguration config)
        {
            this.Id = id;
            this.Config = config;
        }
    }
}
