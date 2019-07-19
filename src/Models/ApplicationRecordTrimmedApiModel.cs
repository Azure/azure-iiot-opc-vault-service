// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

using Microsoft.Azure.IIoT.OpcUa.Api.Vault.v1.Models;

namespace Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.Models {
    public class ApplicationRecordTrimmedApiModel : ApplicationRecordApiModel {
        public ApplicationRecordTrimmedApiModel() { }

        public ApplicationRecordTrimmedApiModel(ApplicationRecordApiModel apiModel) :
            base(apiModel.State, apiModel.ApplicationType, apiModel.ApplicationId, apiModel.Id) {
            ApplicationUri = apiModel.ApplicationUri;
            ApplicationName = apiModel.ApplicationName;
            ApplicationNames = apiModel.ApplicationNames;
            ProductUri = apiModel.ProductUri;
            DiscoveryUrls = apiModel.DiscoveryUrls;
            ServerCapabilities = apiModel.ServerCapabilities;
            GatewayServerUri = apiModel.GatewayServerUri;
            DiscoveryProfileUri = apiModel.DiscoveryProfileUri;
            TrimLength = 40;
        }

        public int TrimLength { get; set; }
        public string ApplicationUriTrimmed => Trimmed(ApplicationUri);
        public string ApplicationNameTrimmed => Trimmed(ApplicationName);

        private string Trimmed(string value) {
            if (value?.Length > TrimLength) {
                return value.Substring(0, TrimLength - 3) + "...";
            }

            return value;
        }
    }

}
