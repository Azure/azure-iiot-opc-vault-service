﻿// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.IoTSolutions.OpcGds.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.OpcGds.Services.Models;
using Microsoft.Azure.IoTSolutions.OpcGds.Services.Runtime;

namespace Microsoft.Azure.IoTSolutions.OpcGds.Services
{
    public interface IDeviceTwins
    {
        Task<IEnumerable<DeviceTwin>> GetListAsync();

        Task<DeviceTwin> GetAsync(string deviceId);
    }

    public sealed class DeviceTwins : IDeviceTwins
    {
        // Max is 1000
        private const int PageSize = 1000;

        private readonly RegistryManager registry;
        private readonly ILogger log;

        public DeviceTwins(
            IServicesConfig config,
            ILogger logger)
        {
            this.registry = RegistryManager.CreateFromConnectionString(config.HubConnString);
            this.log = logger;

            this.log.Debug("Creating new instance of `DeviceTwins` service", () => { });
        }

        public async Task<IEnumerable<DeviceTwin>> GetListAsync()
        {
            var result = new List<DeviceTwin>();
            var query = this.registry.CreateQuery("SELECT * FROM devices", PageSize);
            while (query.HasMoreResults)
            {
                var page = await query.GetNextAsTwinAsync();
                result.AddRange(page.Select(x => new DeviceTwin(x)));
            }

            return result;
        }

        public async Task<DeviceTwin> GetAsync(string id)
        {
            var twin = await this.registry.GetTwinAsync(id);
            return twin == null ? null : new DeviceTwin(twin);
        }
    }
}
