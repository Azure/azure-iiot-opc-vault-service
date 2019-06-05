// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

namespace Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.Models {
    using Microsoft.Azure.IIoT.OpcUa.Api.Vault.v1.Models;
    using Newtonsoft.Json;

    public class CertificateRequestIndexApiModel : CertificateRequestRecordApiModel {
        public CertificateRequestIndexApiModel() {
        }

        public CertificateRequestIndexApiModel(CertificateRequestRecordApiModel apiModel) :
            base(apiModel.State, apiModel.SigningRequest) {
            RequestId = apiModel.RequestId;
            ApplicationId = apiModel.ApplicationId;
            CertificateGroupId = apiModel.CertificateGroupId;
            CertificateTypeId = apiModel.CertificateTypeId;
            SubjectName = apiModel.SubjectName;
            DomainNames = apiModel.DomainNames;
            PrivateKeyFormat = apiModel.PrivateKeyFormat;
            TrimLength = 40;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ApplicationUri")]
        public string ApplicationUri { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ApplicationName")]
        public string ApplicationName { get; set; }

        public int TrimLength { get; set; }
        public string ApplicationUriTrimmed => Trimmed(ApplicationUri);
        public string ApplicationNameTrimmed => Trimmed(ApplicationName);
        public string SubjectNameTrimmed => Trimmed(SubjectName);

        private string Trimmed(string value) {
            if (value?.Length > TrimLength) {
                return value.Substring(0, TrimLength - 3) + "...";
            }
            return value;
        }
    }
}
