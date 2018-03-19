// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.v1.Models
{
    public sealed class X509CrlApiModel
    {
        [JsonProperty(PropertyName = "Issuer", Order = 10)]
        public string Issuer { get; set; }

        [JsonProperty(PropertyName = "Crl", Order = 20)]
        public string Crl { get; set; }

        public X509CrlApiModel(Opc.Ua.X509CRL crl)
        {
            this.Crl = Convert.ToBase64String(crl.RawData);
            this.Issuer = crl.Issuer;
        }

        public Opc.Ua.X509CRL ToServiceModel()
        {
            return new Opc.Ua.X509CRL(Convert.FromBase64String(Crl));
        }

    }
}
