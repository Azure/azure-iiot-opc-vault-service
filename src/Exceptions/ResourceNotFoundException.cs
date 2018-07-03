// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using System;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.Exceptions
{
    /// <summary>
    /// This exception is thrown when a client is requesting a resource that
    /// doesn't exist yet.
    /// </summary>
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException() : base()
        {
        }

        public ResourceNotFoundException(string message) : base(message)
        {
        }

        public ResourceNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
