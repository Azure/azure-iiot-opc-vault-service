// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.GdsVault.CosmosDB.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Models
{
    public sealed class ServerOnNetworkApiModel
    {
        [JsonProperty(PropertyName = "RecordId", Order = 10)]
        public uint RecordId { get; set; }

        [JsonProperty(PropertyName = "ServerName", Order = 20)]
        public string ServerName { get; set; }

        [JsonProperty(PropertyName = "DiscoveryUrl", Order = 30)]
        public string DiscoveryUrl { get; set; }

        [JsonProperty(PropertyName = "ServerCapabilities", Order = 40)]
        public string ServerCapabilities { get; set; }

        public ServerOnNetworkApiModel(Application application, string discoveryUrl)
        {
            this.RecordId = application.ID;
            this.ServerName = application.ApplicationName;
            this.DiscoveryUrl = discoveryUrl;
            this.ServerCapabilities = application.ServerCapabilities;
        }

    }
}
