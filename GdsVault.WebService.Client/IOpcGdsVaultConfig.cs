// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.Client {
    /// <summary>
    /// Configuration for service
    /// </summary>
    public interface IOpcGdsVaultConfig {

        /// <summary>
        /// GDS Vault service url
        /// </summary>
        string OpcGdsVaultServiceApiUrl { get; }
    }
}
