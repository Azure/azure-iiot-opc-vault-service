// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.Services.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.v1.Models
{
    public class DeviceTwinApiModel
    {
        [JsonProperty(PropertyName = "Etag")]
        public string Etag { get; set; }

        [JsonProperty(PropertyName = "DeviceId")]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "ReportedProperties")]
        public Dictionary<string, JToken> ReportedProperties { get; set; }

        [JsonProperty(PropertyName = "DesiredProperties")]
        public Dictionary<string, JToken> DesiredProperties { get; set; }

        [JsonProperty(PropertyName = "Tags")]
        public Dictionary<string, JToken> Tags { get; set; }

        [JsonProperty(PropertyName = "IsSimulated")]
        public bool IsSimulated { get; set; }

        [JsonProperty(PropertyName = "$metadata")]
        public Dictionary<string, string> Metadata => new Dictionary<string, string>
        {
            { "$type", "DeviceTwin;" + Version.Name },
            { "$uri", "/" + Version.Name + "/devices/" + this.DeviceId + "/twin" }
        };

        public DeviceTwinApiModel(string deviceId, DeviceTwinServiceModel deviceTwin)
        {
            if (deviceTwin != null)
            {
                this.Etag = deviceTwin.Etag;
                this.DeviceId = deviceId;
                this.DesiredProperties = deviceTwin.DesiredProperties;
                this.ReportedProperties = deviceTwin.ReportedProperties;
                this.Tags = deviceTwin.Tags;
                this.IsSimulated = deviceTwin.IsSimulated;
            }
        }

        public DeviceTwinServiceModel ToServiceModel()
        {
            return new DeviceTwinServiceModel
            (
                etag: this.Etag,
                deviceId: this.DeviceId,
                desiredProperties: this.DesiredProperties,
                reportedProperties: this.ReportedProperties,
                tags: this.Tags,
                isSimulated: this.IsSimulated
            );
        }
    }
}
