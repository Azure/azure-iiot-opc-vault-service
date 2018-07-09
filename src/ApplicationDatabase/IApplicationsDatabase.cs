﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault
{
    public interface IApplicationsDatabase
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
        Task<Application[]> QueryServersAsync(
            uint startingRecordId,
            uint maxRecordsToReturn,
            string applicationName,
            string applicationUri,
            string productUri,
            string[] serverCapabilities,
            out DateTime lastCounterResetTime
            );

    }
}
