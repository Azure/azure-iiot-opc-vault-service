// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api
{
    using Microsoft.Rest;

    /// <summary>
    /// OPC UA GdsVault Service
    /// </summary>
    public partial class OpcGdsVault : ServiceClient<OpcGdsVault>, IOpcGdsVault
    {
        /// <summary>
        /// Initializes a new instance of the OpcGdsVault class.
        /// </summary>
        /// <param name='baseUri'>
        /// Optional. The base URI of the service.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public OpcGdsVault(System.Uri baseUri)
        {
            if (baseUri == null)
            {
                throw new System.ArgumentNullException("baseUri");
            }
            BaseUri = baseUri;
        }

        /// <summary>
        /// Set the credential handler.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Subscription credentials which uniquely identify client subscription.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public void SetCredentials(ServiceClientCredentials credentials)
        {
            if (credentials == null)
            {
                throw new System.ArgumentNullException("credentials");
            }
            this.Credentials = credentials;
            this.Credentials.InitializeServiceClient(this);
        }
    }
}