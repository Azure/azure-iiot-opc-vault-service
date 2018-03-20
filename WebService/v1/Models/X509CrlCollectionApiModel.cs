﻿// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.v1.Models
{
    public sealed class X509CrlCollectionApiModel
    {
        [JsonProperty(PropertyName = "Chain", Order = 10)]
        public X509CrlApiModel[] Chain { get; set; }

        public X509CrlCollectionApiModel(IList<Opc.Ua.X509CRL> crls)
        {
            var chain = new List<X509CrlApiModel>();
            foreach (var crl in crls)
            {
                var crlApiModel = new X509CrlApiModel(crl);
                chain.Add(crlApiModel);
            }
            this.Chain = chain.ToArray();
        }

    }
}