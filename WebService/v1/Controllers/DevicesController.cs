// Copyright (c) Microsoft. All rights reserved.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IoTSolutions.GdsVault.Services;
using Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Filters;
using Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Models;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Controllers
{
    [Route(Version.Path + "/[controller]"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    public sealed class DevicesController : Controller
    {
        private readonly IDevices devices;

        public DevicesController(IDevices devices)
        {
            this.devices = devices;
        }

        /// <returns>List of devices</returns>
        [HttpGet]
        public async Task<DeviceListApiModel> GetAsync()
        {
            return new DeviceListApiModel(await this.devices.GetListAsync());
        }

        /// <summary>Get one device</summary>
        /// <param name="id">Device Id</param>
        [HttpGet("{id}")]
        public async Task<DeviceApiModel> GetAsync(string id)
        {
            return new DeviceApiModel(await this.devices.GetAsync(id));
        }

        /// <summary>Create one device</summary>
        /// <param name="device">Device information</param>
        /// <returns>Device information</returns>
        [HttpPost]
        public async Task<DeviceApiModel> PostAsync([FromBody] DeviceApiModel device)
        {
            return new DeviceApiModel(await this.devices.CreateAsync(device.ToServiceModel()));
        }
    }
}
