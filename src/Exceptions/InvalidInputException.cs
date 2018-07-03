// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using System;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.Exceptions
{
    /// <summary>
    /// This exception is thrown when a client sends a request badly formatted
    /// or containing invalid values. The client should fix the request before
    /// retrying.
    /// </summary>
    public class InvalidInputException : Exception
    {
        public InvalidInputException() : base()
        {
        }

        public InvalidInputException(string message) : base(message)
        {
        }

        public InvalidInputException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
