// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Models
{

    public sealed class QueryApplicationsByIdApiModel
    {
        [JsonProperty(PropertyName = "startingRecordId", Order = 10)]
        public uint StartingRecordId { get; set; }

        [JsonProperty(PropertyName = "maxRecordsToReturn", Order = 20)]
        public uint MaxRecordsToReturn { get; set; }

        [JsonProperty(PropertyName = "applicationName", Order = 30)]
        public string ApplicationName { get; set; }

        [JsonProperty(PropertyName = "applicationUri", Order = 40)]
        public string ApplicationUri { get; set; }

        [JsonProperty(PropertyName = "applicationType", Order = 50)]
        public QueryApplicationType? ApplicationType { get; set; }

        [JsonProperty(PropertyName = "productUri", Order = 60)]
        public string ProductUri { get; set; }

        [JsonProperty(PropertyName = "serverCapabilities", Order = 70)]
        public IList<string> ServerCapabilities { get; set; }

        [JsonProperty(PropertyName = "applicationState", Order = 55)]
        public QueryApplicationState? ApplicationState { get; set; }

        public QueryApplicationsByIdApiModel(
            uint startingRecordId,
            uint maxRecordsToReturn,
            string applicationName,
            string applicationUri,
            QueryApplicationType? applicationType,
            string productUri,
            IList<string> serverCapabilities,
            QueryApplicationState? applicationState
            )
        {
            this.StartingRecordId = startingRecordId;
            this.MaxRecordsToReturn = maxRecordsToReturn;
            this.ApplicationName = applicationName;
            this.ApplicationUri = applicationUri;
            this.ApplicationType = applicationType;
            this.ApplicationState = applicationState;
            this.ProductUri = productUri;
            this.ServerCapabilities = serverCapabilities;
        }

    }
}
