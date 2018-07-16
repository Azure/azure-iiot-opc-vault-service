﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models;
using System;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Models
{
    public sealed class QueryApplicationsResponseModel
    {
        public Application[] Applications { get; set; }

        public DateTime LastCounterResetTime { get; set; }

        public int NextRecordId { get; set; }

        public QueryApplicationsResponseModel(
            Application[] applications,
            DateTime lastCounterResetTime,
            uint nextRecordId
            )
        {
            this.Applications = applications;
            this.LastCounterResetTime = lastCounterResetTime;
            this.NextRecordId = (int)nextRecordId;
        }
    }
}

