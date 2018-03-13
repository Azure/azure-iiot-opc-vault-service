// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Models
{
    public sealed class NewKeyPairRequestApiModel
    {
        [JsonProperty(PropertyName = "ApplicationURI", Order = 10)]
        public string ApplicationURI { get; set; }

        [JsonProperty(PropertyName = "SubjectName", Order = 20)]
        public string SubjectName { get; set; }

        [JsonProperty(PropertyName = "DomainNames", Order = 30)]
        public string [] DomainNames { get; set; }

        public NewKeyPairRequestApiModel(string applicationURI, string subjectName, string[] domainNames)
        {
            this.ApplicationURI = applicationURI;
            this.SubjectName = subjectName;
            this.DomainNames = domainNames;
        }

    }
}
