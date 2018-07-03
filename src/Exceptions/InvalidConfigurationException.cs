// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using System;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.Exceptions
{
    /// <summary>
    /// This exception is thrown when the service is configured incorrectly.
    /// In order to recover, the service owner should fix the configuration
    /// and re-deploy the service.
    /// </summary>
    public class InvalidConfigurationException : Exception
    {
        public InvalidConfigurationException() : base()
        {
        }

        public InvalidConfigurationException(string message) : base(message)
        {
        }

        public InvalidConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
