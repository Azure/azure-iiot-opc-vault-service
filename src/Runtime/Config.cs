// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.Azure.IIoT.Diagnostics;
using Microsoft.Azure.IIoT.Services.Runtime;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.Runtime
{
    /// <summary>Web service configuration</summary>
    public class Config : ServiceConfig
    {
        // web service config
        private const string ApplicationKey = "GdsVault:";
        private const string PortKey = ApplicationKey + "webservice_port";

        // services config
        private const string KeyVaultKey = "keyvault:";
        private const string KeyVaultApiUrlKey = KeyVaultKey + "serviceuri";
        private const string KeyVaultApiTimeoutKey = KeyVaultKey + "timeout";
        
        /// <summary>
        /// The configuration data.
        /// </summary>
        public IConfigData ConfigData { get; }

        /// <summary>
        /// Configuration constructor
        /// </summary>
        /// <param name="configuration"></param>
        public Config(IConfigurationRoot configuration) :
            base(Uptime.ProcessId, ServiceInfo.ID, configuration)
        {
            this.ConfigData = new ConfigData(configuration);
        }

        private static string MapRelativePath(string path)
        {
            if (path.StartsWith(".")) return AppContext.BaseDirectory + Path.DirectorySeparatorChar + path;
            return path;
        }

        /// <summary>Service layer configuration</summary>
        public IServicesConfig ServicesConfig =>
            new ServicesConfig
            {
                KeyVaultApiUrl = this.ConfigData.GetString(KeyVaultApiUrlKey),
                KeyVaultApiTimeout = this.ConfigData.GetInt(KeyVaultApiTimeoutKey)
            };

        /// <summary>
        /// Auth configuration
        /// </summary>
        private const string AUTH_KEY = "Auth:";
        private const string CORS_WHITELIST_KEY = AUTH_KEY + "cors_whitelist";
        private const string AUTH_TYPE_KEY = AUTH_KEY + "auth_type";
        private const string AUTH_REQUIRED_KEY = AUTH_KEY + "auth_required";
        /// <summary>Type of auth token</summary>
        public string AuthType =>
            ConfigData.GetString(AUTH_TYPE_KEY, "JWT");

        private const string JWT_KEY = AUTH_KEY + "JWT:";
        private const string JWT_ALGOS_KEY = JWT_KEY + "allowed_algorithms";
        private const string JWT_ISSUER_KEY = JWT_KEY + "issuer";
        private const string JWT_AUDIENCE_KEY = JWT_KEY + "audience";
        private const string JWT_CLOCK_SKEW_KEY = JWT_KEY + "clock_skew_seconds";
        /// <summary>Allowed JWT algos</summary>
        public IEnumerable<string> JwtAllowedAlgos =>
            ConfigData.GetString(JWT_ALGOS_KEY, "RS256,RS384,RS512").Split(',');
        /// <summary>JWT issuer</summary>
        public string JwtIssuer =>
            ConfigData.GetString(JWT_ISSUER_KEY);
        /// <summary>JWT audience</summary>
        public string JwtAudience =>
            ConfigData.GetString(JWT_AUDIENCE_KEY);
        /// <summary>JWT clock skew</summary>
        public TimeSpan JwtClockSkew =>
            TimeSpan.FromSeconds(ConfigData.GetInt(JWT_CLOCK_SKEW_KEY, 120));

    }
}

