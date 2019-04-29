// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Models
{
    public sealed class RegistryApplicationStatusResponseApiModel
    {
        [JsonProperty(PropertyName = "applications", Order = 10)]
        public IList<RegistryApplicationStatusApiModel> Applications { get; set; }

        [JsonProperty(PropertyName = "nextPageLink", Order = 20)]
        public string NextPageLink { get; set; }

        public RegistryApplicationStatusResponseApiModel(IList<RegistryApplicationStatusApiModel> applications, string nextPageLink = null)
        {
            Applications = applications;
            NextPageLink = nextPageLink;
        }
    }
}
