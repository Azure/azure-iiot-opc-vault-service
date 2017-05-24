﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.Runtime;
using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.v1.Models
{
    public sealed class StatusApiModel
    {
        private const string DateFormat = "yyyy-MM-dd'T'HH:mm:sszzz";

        [JsonProperty(PropertyName = "Name", Order = 5)]
        public string Name => "ProjectNameHere";

        [JsonProperty(PropertyName = "Status", Order = 10)]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "CurrentTime", Order = 20)]
        public string CurrentTime => DateTimeOffset.UtcNow.ToString(DateFormat);

        [JsonProperty(PropertyName = "StartTime", Order = 30)]
        public string StartTime => Uptime.Start.ToString(DateFormat);

        [JsonProperty(PropertyName = "UpTime", Order = 40)]
        public long UpTime => Convert.ToInt64(Uptime.Duration.TotalSeconds);

        /// <summary>A property bag with details about the service</summary>
        [JsonProperty(PropertyName = "Properties", Order = 50)]
        public Dictionary<string, string> Properties => new Dictionary<string, string>
        {
            { "Foo", "Bar" },
            { "Simulation", "on" },
            { "Region", "US" },
            { "DebugMode", "off" }
        };

        /// <summary>A property bag with details about the internal dependencies</summary>
        [JsonProperty(PropertyName = "Dependencies", Order = 60)]
        public Dictionary<string, string> Dependencies => new Dictionary<string, string>
        {
            { "IoTHubManagerAPI", "OK:...msg..." },
            { "StorageAPI", "OK:timeout after 3 secs" },
            { "AuthAPI", "ERROR:certificate expired" },
            { "IoTHub", "OK:...msg..." },
            { "AAD", "ERROR:certificate expired" }
        };

        [JsonProperty(PropertyName = "$metadata", Order = 1000)]
        public Dictionary<string, string> Metadata => new Dictionary<string, string>
        {
            { "$type", "Status;" + Version.Number },
            { "$uri", "/" + Version.Path + "/status" }
        };

        public StatusApiModel(bool isOk, string msg)
        {
            this.Status = isOk ? "OK" : "ERROR";
            if (!string.IsNullOrEmpty(msg))
            {
                this.Status += ":" + msg;
            }
        }
    }
}
