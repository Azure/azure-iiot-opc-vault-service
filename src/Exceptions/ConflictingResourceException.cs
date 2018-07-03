// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.Exceptions
{
    /// <summary>
    /// This exception is thrown when a client attempts to create a resource
    /// which would conflict with an existing one, for instance using the same
    /// identifier. The client should change the identifier or assume the
    /// resource has already been created.
    /// </summary>
    public class ConflictingResourceException : Exception
    {
        public ConflictingResourceException() : base()
        {
        }

        public ConflictingResourceException(string message) : base(message)
        {
        }

        public ConflictingResourceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
