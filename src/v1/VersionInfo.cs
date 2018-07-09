﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.v1
{
    /// <summary>
    /// Web service API version 1 information
    /// </summary>
    public static class VersionInfo
    {
        /// <summary>
        /// Number used for routing HTTP requests
        /// </summary>
        public const string NUMBER = "1";

        /// <summary>
        /// Full path used in the URL
        /// </summary>
        public const string PATH = "v" + NUMBER;

        /// <summary>
        /// Date when the API version has been published
        /// </summary>
        public const string DATE = "20180701";

    }
}
