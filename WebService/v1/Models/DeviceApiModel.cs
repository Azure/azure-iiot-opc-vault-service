// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.Services.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.v1.Models
{
    public sealed class DeviceApiModel
    {
        private const string DateFormat = "yyyy-MM-dd'T'HH:mm:sszzz";

        // Entity version number, maintained by the service, e.g. with
        // a dedicated property in the storage entity schema.
        // TODO: use the correct value, this is just a sample
        private readonly long version = new Random().Next();

        // When the entity was created (if supported by the storage)
        // TODO: use the correct value, this is just a sample
        private DateTimeOffset created = DateTimeOffset.MinValue;

        // Last time the entity was modified (if supported by the storage)
        // TODO: use the correct value, this is just a sample
        private DateTimeOffset modified = DateTimeOffset.UtcNow;

        [JsonProperty(PropertyName = "ETag")]
        public string Etag { get; set; }

        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "C2DMessageCount")]
        public int C2DMessageCount { get; set; }

        [JsonProperty(PropertyName = "LastActivity")]
        public string LastActivity { get; set; }

        [JsonProperty(PropertyName = "IsConnected")]
        public bool IsConnected { get; set; }

        [JsonProperty(PropertyName = "IsEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty(PropertyName = "LastStatusUpdated")]
        public string LastStatusUpdated { get; set; }

        [JsonProperty(PropertyName = "$metadata")]
        public Dictionary<string, string> Metadata => new Dictionary<string, string>
        {
            { "$type", "Device;" + Version.Number },
            { "$uri", "/" + Version.Path + "/devices/" + this.Id },
            { "$twin_uri", "/" + Version.Path + "/devices/" + this.Id + "/twin" },
            { "$version", this.version.ToString() },
            { "$created", this.created.ToString(DateFormat) },
            { "$modified", this.modified.ToString(DateFormat) }
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DeviceTwinApiModel Twin { get; set; }

        public DeviceApiModel()
        {
        }

        public DeviceApiModel(Device device)
        {
            this.Id = device.Id;
            this.Etag = device.ETag;
            this.C2DMessageCount = device.C2DMessageCount;
            this.LastActivity = device.LastActivity.ToString(DateFormat);
            this.IsConnected = device.Connected;
            this.IsEnabled = device.Enabled;
            this.LastStatusUpdated = device.LastStatusUpdated.ToString(DateFormat);
            this.Twin = new DeviceTwinApiModel(device.Id, device.Twin);
        }

        public Device ToServiceModel()
        {
            return new Device
            (
                eTag: this.Etag,
                id: this.Id,
                c2DMessageCount: this.C2DMessageCount,
                lastActivity: DateTimeOffset.Parse(this.LastActivity),
                connected: this.IsConnected,
                enabled: this.IsEnabled,
                lastStatusUpdated: DateTimeOffset.Parse(this.LastStatusUpdated),
                twin: this.Twin?.ToServiceModel()
            );
        }
    }
}
