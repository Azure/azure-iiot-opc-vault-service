// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.Client.Models
{
    public sealed class StatusApiModel
    {
        private const string DateFormat = "yyyy-MM-dd'T'HH:mm:sszzz";

        [JsonProperty(PropertyName = "Name", Order = 10)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Status", Order = 20)]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "CurrentTime", Order = 30)]
        public string CurrentTime { get; set; }

        [JsonProperty(PropertyName = "StartTime", Order = 40)]
        public string StartTime { get; set; }

        [JsonProperty(PropertyName = "UpTime", Order = 50)]
        public long UpTime { get; set; }

        /// <summary>
        /// Value generated at bootstrap by each instance of the service and
        /// used to correlate logs coming from the same instance. The value
        /// changes every time the service starts.
        /// </summary>
        [JsonProperty(PropertyName = "UID", Order = 60)]
        public string UID { get; set; }

        /// <summary>A property bag with details about the service</summary>
        [JsonProperty(PropertyName = "Properties", Order = 70)]
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>A property bag with details about the internal dependencies</summary>
        [JsonProperty(PropertyName = "Dependencies", Order = 80)]
        public Dictionary<string, string> Dependencies { get; set; }

        [JsonProperty(PropertyName = "$metadata", Order = 1000)]
        public Dictionary<string, string> Metadata { get; set; }

    }
}
