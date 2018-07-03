// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.v1.Models
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
