// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.Services.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.v1.Models
{
    public class DeviceApiModel
    {
        [JsonProperty(PropertyName = "Etag")]
        public string Etag { get; set; }

        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "C2DMessageCount")]
        public int C2DMessageCount { get; set; }

        [JsonProperty(PropertyName = "LastActivity")]
        public DateTime LastActivity { get; set; }

        [JsonProperty(PropertyName = "IsConnected")]
        public bool IsConnected { get; set; }

        [JsonProperty(PropertyName = "IsEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty(PropertyName = "LastStatusUpdated")]
        public DateTime LastStatusUpdated { get; set; }

        [JsonProperty(PropertyName = "$metadata")]
        public Dictionary<string, string> Metadata => new Dictionary<string, string>
        {
            { "$type", "Device;" + Version.Name },
            { "$uri", "/" + Version.Name + "/devices/" + this.Id },
            { "$twin_uri", "/" + Version.Name + "/devices/" + this.Id + "/twin" }
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DeviceTwinApiModel Twin { get; set; }

        public DeviceApiModel()
        {
        }

        public DeviceApiModel(DeviceServiceModel device)
        {
            this.Id = device.Id;
            this.Etag = device.Etag;
            this.C2DMessageCount = device.C2DMessageCount;
            this.LastActivity = device.LastActivity;
            this.IsConnected = device.Connected;
            this.IsEnabled = device.Enabled;
            this.LastStatusUpdated = device.LastStatusUpdated;
            this.Twin = new DeviceTwinApiModel(device.Id,device.Twin);
        }

        public DeviceServiceModel ToServiceModel()
        {
            return new DeviceServiceModel
            (
                etag: this.Etag,
                id: this.Id,
                c2DMessageCount: this.C2DMessageCount,
                lastActivity: this.LastActivity,
                connected: this.IsConnected,
                enabled: this.IsEnabled,
                lastStatusUpdated: this.LastStatusUpdated,
                twin: this.Twin?.ToServiceModel()
            );
        }
    }
}
