// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Models
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

    }
}
