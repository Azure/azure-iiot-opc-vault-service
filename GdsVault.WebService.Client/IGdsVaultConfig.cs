// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.Client {
    /// <summary>
    /// Configuration for service
    /// </summary>
    public interface IGdsVaultConfig {

        /// <summary>
        /// GDS Vault service url
        /// </summary>
        string GdsVaultServiceApiUrl { get; }
    }
}
