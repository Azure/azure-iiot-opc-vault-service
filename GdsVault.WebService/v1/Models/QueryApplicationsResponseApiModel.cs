// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Models
{
    public sealed class QueryApplicationsResponseApiModel
    {
        [JsonProperty(PropertyName = "Applications", Order = 10)]
        public ApplicationDescriptionApiModel[] Applications { get; set; }

        [JsonProperty(PropertyName = "LastCounterResetTime", Order = 20)]
        public DateTime LastCounterResetTime { get; set; }

        [JsonProperty(PropertyName = "NextRecordId", Order = 20)]
        public uint NextRecordId { get; set; }

        public QueryApplicationsResponseApiModel(
            ApplicationDescriptionApiModel[] applications,
            DateTime lastCounterResetTime,
            uint nextRecordId
            )
        {
            this.Applications = applications;
            this.LastCounterResetTime = lastCounterResetTime;
            this.NextRecordId = nextRecordId;
        }
    }
}
