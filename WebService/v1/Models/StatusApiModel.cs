// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.Runtime;
using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.v1.Models
{
    public class StatusApiModel
    {
        [JsonProperty(PropertyName = "Message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "CurrentTime")]
        public DateTime CurrentTime => DateTime.UtcNow;

        [JsonProperty(PropertyName = "StartTime")]
        public DateTime StartTime => Uptime.Start;

        [JsonProperty(PropertyName = "UpTime")]
        public TimeSpan UpTime => Uptime.Duration;

        [JsonProperty(PropertyName = "$metadata")]
        public Dictionary<string, string> Metadata => new Dictionary<string, string>
        {
            { "$type", "Status;" + Version.Name },
            { "$uri", "/" + Version.Name + "/status" }
        };
    }
}
