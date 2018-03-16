﻿// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.v1.Models
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

        public CertificateGroupConfigurationApiModel(string id, Opc.Ua.Gds.Server.CertificateGroupConfiguration config)
        {
            this.Id = id;
            this.SubjectName = config.SubjectName;
            this.DefaultCertificateLifetime = config.DefaultCertificateLifetime;
            this.DefaultCertificateKeySize = config.DefaultCertificateKeySize;
            this.DefaultCertificateHashSize = config.DefaultCertificateHashSize;
        }
    }
}
