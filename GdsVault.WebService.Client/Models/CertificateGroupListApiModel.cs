// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.Client.Models
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
