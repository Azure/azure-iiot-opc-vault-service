// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.GdsVault.CosmosDB.Models;
using Opc.Ua;
using Opc.Ua.Gds;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoTSolutions.GdsVault.Services
{
    public interface IApplicationDatabase
    {
        Task<string> RegisterApplicationAsync(Application application);
        Task<string> UpdateApplicationAsync(string id, Application application);
        Task UnregisterApplicationAsync(string id);
        Task<Application> GetApplicationAsync(string id);
        Task<Application[]> FindApplicationAsync(string uri);
        Task<Application[]> QueryApplicationsAsync(
            uint startingRecordId,
            uint maxRecordsToReturn,
            string applicationName,
            string applicationUri,
            uint applicationType,
            string productUri,
            string[] serverCapabilities,
            out DateTime lastCounterResetTime,
            out uint nextRecordId
            );
    }
}
