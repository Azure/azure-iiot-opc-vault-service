// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using System;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.v1.Exceptions
{
    public class BadRequestException : Exception
    {
        /// <summary>
        /// This exception is thrown by a controller when the input validation
        /// fails. The client should fix the request before retrying.
        /// </summary>
        public BadRequestException() : base()
        {
        }

        public BadRequestException(string message) : base(message)
        {
        }

        public BadRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
