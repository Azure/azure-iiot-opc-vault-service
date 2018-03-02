// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;

namespace Microsoft.Azure.IoTSolutions.OpcGds.Services.Models
{
    public sealed class Device
    {
        public string ETag { get; set; }
        public string Id { get; set; }
        public int C2DMessageCount { get; set; }
        public DateTimeOffset LastActivity { get; set; }
        public bool Connected { get; set; }
        public bool Enabled { get; set; }
        public DateTimeOffset LastStatusUpdated { get; set; }
        public DeviceTwin Twin { get; set; }

        public Device(
            string eTag,
            string id,
            int c2DMessageCount,
            DateTimeOffset lastActivity,
            bool connected,
            bool enabled,
            DateTimeOffset lastStatusUpdated,
            DeviceTwin twin)
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

        public Device(Azure.Devices.Device azureDevice, DeviceTwin twin) :
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

        public Device(Azure.Devices.Device azureDevice, Twin azureTwin) :
            this(azureDevice, new DeviceTwin(azureTwin))
        {
        }

        public Azure.Devices.Device ToAzureModel()
        {
            return new Azure.Devices.Device(this.Id)
            {
                ETag = this.ETag
            };
        }
    }
}
