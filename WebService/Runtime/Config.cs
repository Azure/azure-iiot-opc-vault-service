// Copyright (c) Microsoft. All rights reserved.

using System;
using System.IO;
using Microsoft.Azure.IoTSolutions.Common.Diagnostics;
using Microsoft.Azure.IoTSolutions.OpcGdsVault.Services.Runtime;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.WebService.Runtime
{
    public interface IConfig
    {
        /// <summary>Web service listening port</summary>
        int Port { get; }

        /// <summary>Example of a path setting</summary>
        string SomeFolder { get; }

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
        private const string ApplicationKey = "OpcGdsVault:";
        private const string PortKey = ApplicationKey + "webservice_port";
        private const string SomeFolderKey = ApplicationKey + "some_folder_path";

        private const string IoTHubKey = "iothub:";
        private const string IoTHubConnStringKey = IoTHubKey + "connstring";

        private const string IoTHubManagerKey = "iothubmanager:";
        private const string IoTHubManagerApiUrlKey = IoTHubManagerKey + "webservice_url";
        private const string IoTHubManagerApiTimeoutKey = IoTHubManagerKey + "webservice_timeout";

        /// <summary>Web service listening port</summary>
        public int Port { get; }

        /// <summary>Example of a path setting</summary>
        public string SomeFolder { get; }

        /// <summary>Service layer configuration</summary>
        public IServicesConfig ServicesConfig { get; }

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

            Logger = new Logger(Uptime.ProcessId, LogLevel.Debug);
            //ConfigData.GetLogLevel("Logging:LogLevel:Default", LogLevel.Debug);
            this.Port = ConfigData.GetInt(PortKey);
            this.SomeFolder = MapRelativePath(this.ConfigData.GetString(SomeFolderKey));
            
            this.ServicesConfig = new ServicesConfig
            {
                HubConnString = this.ConfigData.GetString(IoTHubConnStringKey),
                IoTHubManagerApiUrl = this.ConfigData.GetString(IoTHubManagerApiUrlKey),
                IoTHubManagerTimeout = this.ConfigData.GetInt(IoTHubManagerApiTimeoutKey)
            };
        }

        private static string MapRelativePath(string path)
        {
            if (path.StartsWith(".")) return AppContext.BaseDirectory + Path.DirectorySeparatorChar + path;
            return path;
        }
    }
}

