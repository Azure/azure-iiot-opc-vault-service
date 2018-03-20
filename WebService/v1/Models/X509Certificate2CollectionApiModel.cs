﻿// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.v1.Models
{
    public sealed class X509Certificate2CollectionApiModel
    {
        [JsonProperty(PropertyName = "Chain", Order = 10)]
        public X509Certificate2ApiModel[] Chain { get; set; }

        public X509Certificate2CollectionApiModel(X509Certificate2Collection certificateCollection)
        {
            var chain = new List<X509Certificate2ApiModel>();
            foreach (var cert in certificateCollection)
            {
                var certApiModel = new X509Certificate2ApiModel(cert);
                chain.Add(certApiModel);
            }
            this.Chain = chain.ToArray();
        }

    }
}