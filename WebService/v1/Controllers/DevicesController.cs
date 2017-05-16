// Copyright (c) Microsoft. All rights reserved.

using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.Services;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.Runtime;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.v1.Models;

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.v1.Controllers
{
    [RoutePrefix(Version.Name)]
    public class DevicesController : ApiController
    {
        private static readonly IConfig config = new Config();
        private readonly IDevices devices = new Devices(config.ServicesConfig);

        /// <summary>GetAsync a list of devices</summary>
        /// <returns>List of devices</returns>
        public async Task<DeviceListApiModel> GetAsync()
        {
            return new DeviceListApiModel(await this.devices.GetListAsync());
        }

        /// <summary>GetAsync one device</summary>
        /// <param name="id">Device Id</param>
        /// <returns>Device information</returns>
        public async Task<DeviceApiModel> GetAsync(string id)
        {
            return new DeviceApiModel(await this.devices.GetAsync(id));
        }

        /// <summary>Create one device</summary>
        /// <param name="device">Device information</param>
        /// <returns>Device information</returns>
        public async Task<DeviceApiModel> PostAsync(DeviceApiModel device)
        {
            return new DeviceApiModel(await this.devices.CreateAsync(device.ToServiceModel()));
        }
    }
}
