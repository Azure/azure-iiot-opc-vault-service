// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Azure.IoTSolutions.GdsVault.Services.Runtime
{
    public interface IServicesConfig
    {
        string KeyVaultApiUrl { get; set; }
        int KeyVaultApiTimeout { get; set; }
    }

    public class ServicesConfig : IServicesConfig
    {
        public string KeyVaultApiUrl { get; set; }
        public int KeyVaultApiTimeout { get; set; }
    }
}
