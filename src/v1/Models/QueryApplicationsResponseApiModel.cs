// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Models;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.v1.Models
{
    public sealed class QueryApplicationsResponseApiModel
    {
        [JsonProperty(PropertyName = "Applications", Order = 10)]
        public ApplicationDescriptionApiModel[] Applications { get; set; }

        [JsonProperty(PropertyName = "LastCounterResetTime", Order = 20)]
        public DateTime LastCounterResetTime { get; set; }

        [JsonProperty(PropertyName = "NextRecordId", Order = 30)]
        public uint NextRecordId { get; set; }

        public QueryApplicationsResponseApiModel(
            Application[] applications,
            DateTime lastCounterResetTime,
            uint nextRecordId
            )
        {
            var applicationsList = new List<ApplicationDescriptionApiModel>();
            foreach (var application in applications)
            {
                applicationsList.Add(new ApplicationDescriptionApiModel(application));
            }
            this.Applications = applicationsList.ToArray();
            this.LastCounterResetTime = lastCounterResetTime;
            this.NextRecordId = nextRecordId;
        }

        public QueryApplicationsResponseApiModel(QueryApplicationsResponseModel model)
        {
            var applicationsList = new List<ApplicationDescriptionApiModel>();
            foreach (var application in model.Applications)
            {
                applicationsList.Add(new ApplicationDescriptionApiModel(application));
            }
            this.Applications = applicationsList.ToArray();
            this.LastCounterResetTime = model.LastCounterResetTime;
            this.NextRecordId = model.NextRecordId;
        }

    }


}
