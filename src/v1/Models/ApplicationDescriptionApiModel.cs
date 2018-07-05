// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.v1.Models
{
    public sealed class ApplicationDescriptionApiModel
    {
        [JsonProperty(PropertyName = "ApplicationUri", Order = 10)]
        public string ApplicationUri { get; set; }

        [JsonProperty(PropertyName = "ProductUri", Order = 20)]
        public string ProductUri { get; set; }

        [JsonProperty(PropertyName = "ApplicationName", Order = 30)]
        public ApplicationNameApiModel ApplicationName { get; set; }

        [JsonProperty(PropertyName = "ApplicationType", Order = 40)]
        public int ApplicationType { get; set; }

        [JsonProperty(PropertyName = "GatewayServerUri", Order = 50)]
        public string GatewayServerUri { get; set; }

        [JsonProperty(PropertyName = "DiscoveryProfileUri", Order = 60)]
        public string[] DiscoveryProfileUri { get; set; }

        [JsonProperty(PropertyName = "DiscoveryUrls", Order = 70)]
        public string[] DiscoveryUrls { get; set; }

        public ApplicationDescriptionApiModel(Application application)
        {
            this.ApplicationUri = application.ApplicationUri;
            if (application.ApplicationNames != null &&
                application.ApplicationNames.Length >= 1)
            {
                this.ApplicationName = new ApplicationNameApiModel(application.ApplicationNames[0]);
            }
            else
            {
                this.ApplicationName = new ApplicationNameApiModel(application.ApplicationName);
            }
            this.ApplicationType = application.ApplicationType;
            this.ProductUri = application.ProductUri;
        }

    }
}
