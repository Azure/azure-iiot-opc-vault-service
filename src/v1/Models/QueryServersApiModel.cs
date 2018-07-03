﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.v1.Models
{
    public sealed class QueryServersApiModel
    {
        [JsonProperty(PropertyName = "StartingRecordId", Order = 10)]
        public uint StartingRecordId { get; set; }

        [JsonProperty(PropertyName = "MaxRecordsToReturn", Order = 20)]
        public uint MaxRecordsToReturn { get; set; }

        [JsonProperty(PropertyName = "ApplicationName", Order = 30)]
        public string ApplicationName { get; set; }

        [JsonProperty(PropertyName = "ApplicationUri", Order = 40)]
        public string ApplicationUri { get; set; }

        [JsonProperty(PropertyName = "ProductUri", Order = 60)]
        public string ProductUri { get; set; }

        [JsonProperty(PropertyName = "ServerCapabilities", Order = 70)]
        public string [] ServerCapabilities { get; set; }

        public QueryServersApiModel(
            uint startingRecordId,
            uint maxRecordsToReturn,
            string applicationName,
            string applicationUri,
            string productUri,
            IList<string> serverCapabilities
            )
        {
            this.StartingRecordId = startingRecordId;
            this.MaxRecordsToReturn = maxRecordsToReturn;
            this.ApplicationName = applicationName;
            this.ApplicationUri = applicationUri;
            this.ProductUri = productUri;
            this.ServerCapabilities = serverCapabilities?.ToArray();
        }

    }
}