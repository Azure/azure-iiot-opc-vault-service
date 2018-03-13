// Copyright (c) Microsoft. All rights reserved.

using System;
using System.IO;
using Microsoft.Azure.IoTSolutions.GdsVault.Services.Runtime;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.Runtime
{
    public interface IConfig
    {
        /// <summary>Web service listening port</summary>
        int Port { get; }

        /// <summary>Example of a path setting</summary>
        string SomeFolder { get; }

        /// <summary>Service layer configuration</summary>
        IServicesConfig ServicesConfig { get; }
    }

    /// <summary>Web service configuration</summary>
    public class Config : IConfig
    {
        private const string ApplicationKey = "GdsVault:";
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

        public Config(IConfigData configData)
        {
            this.Port = configData.GetInt(PortKey);
            this.SomeFolder = MapRelativePath(configData.GetString(SomeFolderKey));

            this.ServicesConfig = new ServicesConfig
            {
                HubConnString = configData.GetString(IoTHubConnStringKey),
                IoTHubManagerApiUrl = configData.GetString(IoTHubManagerApiUrlKey),
                IoTHubManagerTimeout = configData.GetInt(IoTHubManagerApiTimeoutKey)
            };
        }

        private static string MapRelativePath(string path)
        {
            if (path.StartsWith(".")) return AppContext.BaseDirectory + Path.DirectorySeparatorChar + path;
            return path;
        }
    }
}
