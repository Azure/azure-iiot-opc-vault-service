// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//


namespace Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.Models {
    using Newtonsoft.Json;

    public class CertificateDetailsCollectionApiModel {
        /// <summary>
        /// Initializes a new instance of the CertificateDetailsCollectionApiModel
        /// class.
        /// </summary>
        public CertificateDetailsCollectionApiModel(string id) => Name = id;

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Certificates")]
        public CertificateDetailsApiModel[] Certificates { get; set; }

    }
}
