// Copyright (c) Microsoft. All rights reserved.

using System;
using System.IO;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.Services.Runtime;

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.Runtime
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
        private const string Application = "projectnamehere:";

        /// <summary>Web service listening port</summary>
        public int Port { get; }

        /// <summary>Example of a path setting</summary>
        public string SomeFolder { get; }

        /// <summary>Service layer configuration</summary>
        public IServicesConfig ServicesConfig { get; }

        public Config(IConfigData configData)
        {
            this.Port = configData.GetInt(Application + "webservice_port");
            this.SomeFolder = MapRelativePath(configData.GetString(Application + "some_folder_path"));

            this.ServicesConfig = new ServicesConfig
            {
                HubConnString = configData.GetString("iothub:connstring"),
                IoTHubManagerHost = configData.GetString("iothubmanager:webservice_host"),
                IoTHubManagerPort = configData.GetInt("iothubmanager:webservice_port"),
                IoTHubManagerTimeout = configData.GetInt("iothubmanager:webservice_timeout")
            };
        }

        private static string MapRelativePath(string path)
        {
            if (path.StartsWith(".")) return AppContext.BaseDirectory + Path.DirectorySeparatorChar + path;
            return path;
        }
    }
}
