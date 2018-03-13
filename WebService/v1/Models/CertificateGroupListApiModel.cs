// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Azure.IoTSolutions.GdsVault.WebService.Runtime;
using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Models
{
    public sealed class CertificateGroupListApiModel
    {
        [JsonProperty(PropertyName = "Groups", Order = 20)]
        public string [] Groups { get; set; }

        public CertificateGroupListApiModel(string[] groups)
        {
            this.Groups = groups;
        }
    }
}
