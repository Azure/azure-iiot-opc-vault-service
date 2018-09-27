﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Models
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
