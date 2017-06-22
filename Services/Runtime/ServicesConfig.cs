// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.Services.Runtime
{
    public interface IServicesConfig
    {
        string HubConnString { get; set; }
        string IoTHubManagerHost { get; set; }
        int IoTHubManagerPort { get; set; }
        int IoTHubManagerTimeout { get; set; }
    }

    public class ServicesConfig : IServicesConfig
    {
        public string HubConnString { get; set; }
        public string IoTHubManagerHost { get; set; }
        public int IoTHubManagerPort { get; set; }
        public int IoTHubManagerTimeout { get; set; }
    }
}
