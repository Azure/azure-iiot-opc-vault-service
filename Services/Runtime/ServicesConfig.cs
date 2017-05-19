// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.Services.Runtime
{
    public interface IServicesConfig
    {
        string HubConnString { get; set; }
    }

    public class ServicesConfig : IServicesConfig
    {
        public string HubConnString { get; set; }
    }
}
