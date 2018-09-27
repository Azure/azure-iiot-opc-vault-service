// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

namespace Microsoft.Azure.IIoT.OpcUa.Services.Vault.Api
{
    using Microsoft.Rest;

    public class OpcVaultApiOptions
    {
        public string ResourceId { get; set; }
        public string BaseAddress { get; set; }
    }

    /// <summary>
    /// OPC UA Vault Service
    /// </summary>
    public partial class OpcVault : ServiceClient<OpcVault>, IOpcVault
    {
        /// <summary>
        /// Initializes a new instance of the OpcVault class.
        /// </summary>
        /// <param name='options'>
        /// OpcVaultApiOptions class.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public OpcVault(OpcVaultApiOptions options)
        {
            if (options == null)
            {
                throw new System.ArgumentNullException("OpcVaultApiOptions");
            }
            BaseUri = new System.Uri(options.BaseAddress);
        }

        /// <summary>
        /// Initializes a new instance of the OpcVault class.
        /// </summary>
        /// <param name='baseUri'>
        /// Optional. The base URI of the service.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public OpcVault(System.Uri baseUri)
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
