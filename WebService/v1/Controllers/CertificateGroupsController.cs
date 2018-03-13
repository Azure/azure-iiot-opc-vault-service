// Copyright (c) Microsoft. All rights reserved.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IoTSolutions.GdsVault.Services;
using Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Filters;
using Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Models;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Controllers
{
    [Route(Version.Path + "/[controller]"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    public sealed class CertificateGroupsController : Controller
    {
        private readonly ICertificateGroups certificateGroups;

        public CertificateGroupsController(ICertificateGroups certificateGroups)
        {
            this.certificateGroups = certificateGroups;
        }

        /// <returns>List of certificate groups</returns>
        [HttpGet]
        public async Task<CertificateGroupListApiModel> GetAsync()
        {
            return new CertificateGroupListApiModel(await this.certificateGroups.GetCertificateGroupIds());
        }

        /// <summary>Get one device</summary>
        /// <param name="id">Device Id</param>
        [HttpGet("{id}")]
        public async Task<CertificateGroupConfigurationApiModel> GetAsync(string id)
        {
            return new CertificateGroupConfigurationApiModel(id, await this.certificateGroups.GetCertificateGroupConfiguration(id));
        }
#if mist
        /// <summary>Create one device</summary>
        /// <param name="device">Device information</param>
        /// <returns>Device information</returns>
        [HttpPost]
        public async Task<DeviceApiModel> PostAsync([FromBody] DeviceApiModel device)
        {
            return new DeviceApiModel(await this.certificateGroups.CreateAsync(device.ToServiceModel()));
        }
#endif
    }
}
