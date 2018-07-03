// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.AspNetCore.Builder;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.Auth {

    /// <summary>
    /// Cors setup
    /// </summary>
    public interface ICorsSetup
    {
        /// <summary>
        /// Configure cors on app
        /// </summary>
        /// <param name="app"></param>
        void UseMiddleware(IApplicationBuilder app);
    }
}
