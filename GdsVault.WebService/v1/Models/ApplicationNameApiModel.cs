﻿// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.GdsVault.CosmosDB.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Models
{
    public sealed class ApplicationNameApiModel
    {
        [JsonProperty(PropertyName = "Locale", Order = 10)]
        public string Locale { get; set; }

        [JsonProperty(PropertyName = "Text", Order = 20)]
        public string Text { get; set; }

        public ApplicationNameApiModel(ApplicationName applicationName)
        {
            this.Locale = applicationName.Locale;
            this.Text = applicationName.Text;
        }

        public ApplicationNameApiModel(string applicationName)
        {
            this.Locale = null;
            this.Text = applicationName;
        }

        public ApplicationName ToServiceModel()
        {
            var applicationName = new ApplicationName();
            applicationName.Locale = this.Locale;
            applicationName.Text = this.Text;
            return applicationName;
        }

    }
}
