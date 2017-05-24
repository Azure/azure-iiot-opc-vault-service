// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.Services.Models
{
    public sealed class DeviceServiceModel
    {
        public string ETag { get; set; }
        public string Id { get; set; }
        public int C2DMessageCount { get; set; }
        public DateTimeOffset LastActivity { get; set; }
        public bool Connected { get; set; }
        public bool Enabled { get; set; }
        public DateTimeOffset LastStatusUpdated { get; set; }
        public DeviceTwinServiceModel Twin { get; set; }

        public DeviceServiceModel(
            string eTag,
            string id,
            int c2DMessageCount,
            DateTimeOffset lastActivity,
            bool connected,
            bool enabled,
            DateTimeOffset lastStatusUpdated,
            DeviceTwinServiceModel twin)
        {
            this.ETag = eTag;
            this.Id = id;
            this.C2DMessageCount = c2DMessageCount;
            this.LastActivity = lastActivity.UtcDateTime;
            this.Connected = connected;
            this.Enabled = enabled;
            this.LastStatusUpdated = lastStatusUpdated.UtcDateTime;
            this.Twin = twin;
        }

        public DeviceServiceModel(Device azureDevice, DeviceTwinServiceModel twin) :
            this(
                eTag: azureDevice.ETag,
                id: azureDevice.Id,
                c2DMessageCount: azureDevice.CloudToDeviceMessageCount,
                lastActivity: new DateTimeOffset(azureDevice.LastActivityTime, TimeSpan.Zero),
                connected: azureDevice.ConnectionState.Equals(DeviceConnectionState.Connected),
                enabled: azureDevice.Status.Equals(DeviceStatus.Enabled),
                lastStatusUpdated: new DateTimeOffset(azureDevice.StatusUpdatedTime, TimeSpan.Zero),
                twin: twin)
        {
        }

        public DeviceServiceModel(Device azureDevice, Twin azureTwin) :
            this(azureDevice, new DeviceTwinServiceModel(azureTwin))
        {
        }

        public Device ToAzureModel()
        {
            return new Device(this.Id)
            {
                ETag = this.ETag
            };
        }
    }
}
