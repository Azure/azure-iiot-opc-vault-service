// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Models
{
    public sealed class QueryApplicationsPageApiModel
    {
        [JsonProperty(PropertyName = "applicationName", Order = 20)]
        public string ApplicationName { get; set; }

        [JsonProperty(PropertyName = "applicationUri", Order = 30)]
        public string ApplicationUri { get; set; }

        [JsonProperty(PropertyName = "applicationType", Order = 40)]
        public uint ApplicationType { get; set; }

        [JsonProperty(PropertyName = "productUri", Order = 50)]
        public string ProductUri { get; set; }

        [JsonProperty(PropertyName = "serverCapabilities", Order = 60)]
        public IList<string> ServerCapabilities { get; set; }

        [JsonProperty(PropertyName = "nextPageLink", Order = 70)]
        public string NextPageLink { get; set; }

        [JsonProperty(PropertyName = "maxRecordsToReturn", Order = 80)]
        public int MaxRecordsToReturn { get; set; }

        public QueryApplicationsPageApiModel(
            string applicationName,
            string applicationUri,
            uint applicationType,
            string productUri,
            IList<string> serverCapabilities,
            string nextPageLink = null,
            int maxRecordsToReturn = -1
            )
        {
            this.ApplicationName = applicationName;
            this.ApplicationUri = applicationUri;
            this.ApplicationType = applicationType;
            this.ProductUri = productUri;
            this.ServerCapabilities = serverCapabilities;
            this.NextPageLink = nextPageLink;
            this.MaxRecordsToReturn = maxRecordsToReturn;
        }

    }
}
