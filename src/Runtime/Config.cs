// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.Azure.IIoT.Services.Runtime;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Runtime
{
    /// <summary>Web service configuration</summary>
    public class Config : ServiceConfig
    {
        // services config
        private const string KeyVaultKey = "KeyVault:";
        private const string KeyVaultApiUrlKey = KeyVaultKey + "ServiceUri";
        private const string KeyVaultResourceIDKey = KeyVaultKey + "ResourceID";
        private const string KeyVaultApiTimeoutKey = KeyVaultKey + "Timeout";
        private const string CosmosDBKey = "CosmosDB:";
        private const string CosmosDBEndpointKey = CosmosDBKey + "EndPoint";
        private const string CosmosDBTokenKey = CosmosDBKey + "Token";

        /// <summary>
        /// Configuration constructor
        /// </summary>
        /// <param name="configuration"></param>
        public Config(IConfigurationRoot configuration) :
            base(Uptime.ProcessId, ServiceInfo.ID, configuration)
        {
        }

        /// <summary>Service layer configuration</summary>
        public IServicesConfig ServicesConfig =>
            new ServicesConfig
            {
                KeyVaultApiUrl = this.GetString(KeyVaultApiUrlKey),
                KeyVaultResourceID = this.GetString(KeyVaultResourceIDKey),
                KeyVaultApiTimeout = this.GetInt(KeyVaultApiTimeoutKey),
                CosmosDBEndpoint = this.GetString(CosmosDBEndpointKey),
                CosmosDBToken = this.GetString(CosmosDBTokenKey)
            };
    }
}

