﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.v1.Auth {
    using System.Collections.Generic;

    /// <summary>
    /// Defines gdsvault api policies.
    /// </summary>
    public static class Policies {

        /// <summary>
        /// Allowed to query applications and cert requests
        /// </summary>
        public const string CanRead =
            nameof(CanRead);

        /// <summary>
        /// Allowed to manage applications and cert requests
        /// </summary>
        public const string CanManage =
            nameof(CanManage);

        /// <summary>
        /// Return all policies
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> All() {
            yield return CanRead;
            yield return CanManage;
        }
    }
}
