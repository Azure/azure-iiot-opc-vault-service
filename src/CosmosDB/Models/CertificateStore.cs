// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.CosmosDB.Models
{
    [Serializable]
    public class CertificateStore
    {
        [JsonProperty(PropertyName = "id")]
        public Guid TrustListId { get; private set; }
        public string Path { get; set; }
        public string AuthorityId { get; set; }
    }

}
