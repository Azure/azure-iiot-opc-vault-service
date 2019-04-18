// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Services.Vault.Runtime
{
    /// <summary>
    /// Registry configuration
    /// </summary>
    public class RegistryConfig
    {
        /// <summary>
        /// Opc registry service url
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Resource id of registry service
        /// </summary>
        public string ServiceResourceId { get; set; }

    }
}
