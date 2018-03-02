// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Azure.IoTSolutions.OpcGds.Services.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.IoTSolutions.OpcGds.WebService.v1.Models
{
    public sealed class DeviceTwinApiModel
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
        public string ETag { get; set; }

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
            { "$type", "DeviceTwin;" + Version.Number },
            { "$uri", "/" + Version.Path + "/devices/" + this.DeviceId + "/twin" },
            { "$version", this.version.ToString() },
            { "$created", this.created.ToString(DateFormat) },
            { "$modified", this.modified.ToString(DateFormat) }
        };

        public DeviceTwinApiModel(string deviceId, DeviceTwin deviceTwin)
        {
            if (deviceTwin != null)
            {
                this.ETag = deviceTwin.ETag;
                this.DeviceId = deviceId;
                this.DesiredProperties = deviceTwin.DesiredProperties;
                this.ReportedProperties = deviceTwin.ReportedProperties;
                this.Tags = deviceTwin.Tags;
                this.IsSimulated = deviceTwin.IsSimulated;
            }
        }

        public DeviceTwin ToServiceModel()
        {
            return new DeviceTwin
            (
                eTag: this.ETag,
                deviceId: this.DeviceId,
                desiredProperties: this.DesiredProperties,
                reportedProperties: this.ReportedProperties,
                tags: this.Tags,
                isSimulated: this.IsSimulated
            );
        }
    }
}
