// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.ProjectNameHere.Services.Runtime;

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.Runtime
{
    public interface IConfig
    {
        /// <summary>Web service listening port</summary>
        int Port { get; }

        /// <summary>Service layer configuration</summary>
        IServicesConfig ServicesConfig { get; }
    }

    /// <summary>Web service configuration</summary>
    public class Config : IConfig
    {
        private const string Application = "project-name-here.";

        /// <summary>Web service listening port</summary>
        public int Port { get; }

        /// <summary>Service layer configuration</summary>
        public IServicesConfig ServicesConfig { get; }

        public Config(IConfigData configData)
        {
            this.Port = configData.GetInt(Application + "webservice.port");

            this.ServicesConfig = new ServicesConfig
            {
                HubConnString = configData.GetString(Application + "iothub.connstring"),
                IoTHubManagerHost = configData.GetString("iothubmanager.webservice.host"),
                IoTHubManagerPort = configData.GetInt("iothubmanager.webservice.port")
            };
        }
    }
}
