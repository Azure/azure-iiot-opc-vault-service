// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Models
{
    public sealed class SigningRequestApiModel
    {
        [JsonProperty(PropertyName = "ApplicationURI", Order = 10)]
        public string ApplicationURI { get; set; }

        [JsonProperty(PropertyName = "Csr", Order = 20)]
        public string Csr { get; set; }

        public SigningRequestApiModel(string applicationURI, byte [] csr)
        {
            this.Csr = Convert.ToBase64String(csr);
            this.ApplicationURI = applicationURI;
        }

        public byte [] ToServiceModel()
        {
            return Convert.FromBase64String(Csr);
        }

    }
}
