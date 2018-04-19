// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IoTSolutions.OpcGdsVault.Common.Http {
    using Microsoft.Azure.IoTSolutions.OpcGdsVault.Common.Exceptions;
    using System.Net;

    /// <summary>
    /// Retriable exception
    /// </summary>
    public class HttpTransientException : HttpResponseException, ITransientException {

        /// <inheritdoc />
        public HttpTransientException(HttpStatusCode statusCode) :
            base(statusCode) {
        }

        /// <inheritdoc />
        public HttpTransientException(HttpStatusCode statusCode, string message) :
            base(statusCode, message) {
        }
    }
}
