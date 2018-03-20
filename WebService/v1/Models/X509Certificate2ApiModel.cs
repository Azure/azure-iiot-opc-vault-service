﻿// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.v1.Models
{
    public sealed class X509Certificate2ApiModel
    {
        [JsonProperty(PropertyName = "Subject", Order = 10)]
        public string Subject { get; set; }

        [JsonProperty(PropertyName = "Thumbprint", Order = 20)]
        public string Thumbprint { get; set; }

        [JsonProperty(PropertyName = "Certificate", Order = 20)]
        public string Certificate { get; set; }

        public X509Certificate2ApiModel(X509Certificate2 certificate)
        {
            this.Certificate = Convert.ToBase64String(certificate.RawData);
            this.Thumbprint = certificate.Thumbprint;
            this.Subject = certificate.Subject;
        }

        public X509Certificate2 ToServiceModel()
        {
            return new X509Certificate2(Convert.FromBase64String(Certificate));
        }

    }
}