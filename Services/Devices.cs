﻿// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.Services.Runtime;

// TODO: handle exceptions
// TODO: logging
// TODO: documentation

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.Services
{
    public interface IDevices
    {
        Task<IEnumerable<Models.Device>> GetListAsync();
        Task<Models.Device> GetAsync(string id);
        Task<Models.Device> CreateAsync(Models.Device toServiceModel);
    }

    public sealed class Devices : IDevices
    {
        private const int MaxGetList = 1000;
        private readonly RegistryManager registry;
        private readonly IDeviceTwins deviceTwins;

        public Devices(IServicesConfig config, IDeviceTwins deviceTwins)
        {
            this.registry = RegistryManager.CreateFromConnectionString(config.HubConnString);
            this.deviceTwins = deviceTwins;
        }

        public async Task<IEnumerable<Models.Device>> GetListAsync()
        {
            var devices = await this.registry.GetDevicesAsync(MaxGetList);

            return devices.Select(azureDevice => new Models.Device(azureDevice, (Twin)null)).ToList();
        }

        public async Task<Models.Device> GetAsync(string id)
        {
            var remoteDevice = await this.registry.GetDeviceAsync(id);

            return remoteDevice == null ? null : new Models.Device(remoteDevice, await this.deviceTwins.GetAsync(id));
        }

        public async Task<Models.Device> CreateAsync(Models.Device device)
        {
            var azureDevice = await this.registry.AddDeviceAsync(device.ToAzureModel());

            // TODO: do we need to fetch the twin and return it?
            if (device.Twin == null) return new Models.Device(azureDevice, (Twin)null);

            // TODO: do we need to fetch the twin ETag first?
            var azureTwin = await this.registry.UpdateTwinAsync(device.Id, device.Twin.ToAzureModel(), device.Twin.ETag);
            return new Models.Device(azureDevice, azureTwin);
        }
    }
}
