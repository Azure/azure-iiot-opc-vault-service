﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.Runtime
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