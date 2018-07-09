// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.v1.Models
{
    public sealed class QueryServersResponseApiModel
    {
        [JsonProperty(PropertyName = "Servers", Order = 10)]
        public ServerOnNetworkApiModel [] Servers { get; set; }

        [JsonProperty(PropertyName = "LastCounterResetTime", Order = 20)]
        public DateTime LastCounterResetTime { get; set; }

        public QueryServersResponseApiModel(
            ServerOnNetworkApiModel[] servers,
            DateTime lastCounterResetTime
            )
        {
            this.Servers = servers;
            this.LastCounterResetTime = lastCounterResetTime;
        }

        public QueryServersResponseApiModel(
            Application[] applications,
            DateTime lastCounterResetTime
            )
        {
            var servers = new List<ServerOnNetworkApiModel>();
            if (applications != null)
            {
                foreach (var application in applications)
                {
                    foreach (var discoverUrl in application.DiscoveryUrls)
                    {
                        servers.Add(new ServerOnNetworkApiModel(application, discoverUrl));
                    }
                }
            }
            this.Servers = servers.ToArray();
            this.LastCounterResetTime = lastCounterResetTime;
        }

    }
}
