// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1
{
    /// <summary>
    /// Web service API version 1 information
    /// </summary>
    public static class ServiceInfo
    {
        /// <summary>
        /// Name of service
        /// </summary>
        public const string NAME = "GdsVault";

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
        public const string DATE = "20180501";

        /// <summary>
        /// Description of service
        /// </summary>
        public const string DESCRIPTION = "Opc GDS Vault Micro Service";
    }
}
