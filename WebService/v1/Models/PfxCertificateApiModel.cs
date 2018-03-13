// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Models
{
    public sealed class PfxCertificateApiModel
    {
        [JsonProperty(PropertyName = "Subject", Order = 10)]
        public string Subject { get; set; }

        [JsonProperty(PropertyName = "Pfx", Order = 20)]
        public string Pfx { get; set; }

        public PfxCertificateApiModel(X509Certificate2 certificate)
        {
            this.Pfx = Convert.ToBase64String(certificate.Export(X509ContentType.Pfx));
            this.Subject = certificate.Subject;
        }

        public X509Certificate2 ToServiceModel()
        {
            return CertificateFactory.CreateCertificateFromPKCS12(Convert.FromBase64String(Pfx), string.Empty);
        }

    }
}
