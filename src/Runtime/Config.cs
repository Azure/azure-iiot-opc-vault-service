// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.Azure.IIoT.OpcUa.Services.Gds.Auth;
using Microsoft.Azure.IIoT.Diagnostics;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.Runtime
{
    public interface IConfig : IClientAuthConfig
    {
        /// <summary>Web service listening port</summary>
        int Port { get; }

        /// <summary>Example of a path setting</summary>
        ///string SomeFolder { get; }

        /// <summary>Service layer configuration</summary>
        IServicesConfig ServicesConfig { get; }

        /// <summary>
        /// A configured logger
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Configuration
        /// </summary>
        IConfigurationRoot Configuration { get; }

        /// <summary>
        /// The configuration data.
        /// </summary>
        IConfigData ConfigData { get; }

    }

    /// <summary>Web service configuration</summary>
    public class Config : IConfig
    {
        // web service config
        private const string ApplicationKey = "GdsVault:";
        private const string PortKey = ApplicationKey + "webservice_port";

        // services config
        private const string KeyVaultKey = "keyvault:";
        private const string KeyVaultApiUrlKey = KeyVaultKey + "serviceuri";
        private const string KeyVaultApiTimeoutKey = KeyVaultKey + "timeout";
        

        // TODO: OPCTWIN
        //private const string IoTHubManagerKey = "iothubmanager:";
        //private const string IoTHubManagerApiUrlKey = IoTHubManagerKey + "webservice_url";
        //private const string IoTHubManagerApiTimeoutKey = IoTHubManagerKey + "webservice_timeout";

        /// <summary>
        /// A configured logger.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// The Configuration root.
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        /// <summary>
        /// The configuration data.
        /// </summary>
        public IConfigData ConfigData { get; }

        public Config(IConfigurationRoot configRoot)
        {
            this.Configuration = configRoot;
            this.ConfigData = new ConfigData(configRoot);
            this.Logger = new TraceLogger(Uptime.ProcessId);
            // override port
            if (Port != 0)
            {
                configRoot["urls"] = "http://*:" + Port.ToString();
            }
        }

        private static string MapRelativePath(string path)
        {
            if (path.StartsWith(".")) return AppContext.BaseDirectory + Path.DirectorySeparatorChar + path;
            return path;
        }

        /// <summary>Web service listening port</summary>
        public int Port => ConfigData.GetInt(PortKey);

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
        /// <summary>Cors whitelist</summary>
        public string CorsWhitelist =>
            ConfigData.GetString(CORS_WHITELIST_KEY, string.Empty);
        /// <summary>Whether enabled</summary>
        public bool CorsEnabled =>
            !string.IsNullOrEmpty(CorsWhitelist.Trim());
        /// <summary>Auth needed?</summary>
        public bool AuthRequired =>
            ConfigData.GetBool(AUTH_REQUIRED_KEY, false);
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

