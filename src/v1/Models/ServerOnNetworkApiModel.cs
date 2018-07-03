// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.Azure.IIoT.OpcUa.Services.Gds.CosmosDB.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.v1.Models
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
