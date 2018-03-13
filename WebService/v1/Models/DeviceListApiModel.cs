// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Microsoft.Azure.IoTSolutions.GdsVault.Services.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Models
{
    public sealed class DeviceListApiModel
    {
        [JsonProperty(PropertyName = "Items")]
        public List<DeviceApiModel> Items { get; set; }

        [JsonProperty(PropertyName = "$metadata")]
        public Dictionary<string, string> Metadata => new Dictionary<string, string>
        {
            { "$type", "DeviceList;" + Version.Number },
            { "$uri", "/" + Version.Path + "/devices" }
        };

        public DeviceListApiModel(IEnumerable<Device> devices)
        {
            this.Items = new List<DeviceApiModel>();
            foreach (var d in devices) this.Items.Add(new DeviceApiModel(d));
        }
    }
}
