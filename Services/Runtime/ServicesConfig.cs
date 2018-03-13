// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Azure.IoTSolutions.GdsVault.Services.Runtime
{
    public interface IServicesConfig
    {
        string HubConnString { get; set; }
        string IoTHubManagerApiUrl { get; set; }
        int IoTHubManagerTimeout { get; set; }
    }

    public class ServicesConfig : IServicesConfig
    {
        public string HubConnString { get; set; }
        public string IoTHubManagerApiUrl { get; set; }
        public int IoTHubManagerTimeout { get; set; }
    }
}
