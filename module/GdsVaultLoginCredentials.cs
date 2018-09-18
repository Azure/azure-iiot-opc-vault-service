﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Opc.Ua.Gds.Server.GdsVault
{
    // Summary:
    //     Options for configuring authentication using Azure Active Directory.
    public class GdsEdgeAzureADOptions
    {
        //
        // Summary:
        //     Gets or sets the client Id.
        public string ClientId { get; set; }
        //
        // Summary:
        //     Gets or sets the client secret.
        public string ClientSecret { get; set; }
        //
        // Summary:
        //     Gets or sets the tenant Id.
        public string TenantId { get; set; }
        //
        // Summary:
        //     Gets or sets the Azure Active Directory instance.
        public string Authority { get; set; }
    }

    public class GdsVaultLoginCredentials : ServiceClientCredentials
    {
        private GdsVaultApiOptions gdsVaultOptions;
        private GdsEdgeAzureADOptions azureADOptions;
        private const string kAuthority = "https://login.microsoftonline.com/";
        private string AuthenticationToken { get; set; }
        private DateTimeOffset ExpiresOn { get; set; }

        public GdsVaultLoginCredentials(
            GdsVaultApiOptions gdsVaultOptions,
            GdsEdgeAzureADOptions azureADOptions)
        {
            this.gdsVaultOptions = gdsVaultOptions;
            this.azureADOptions = azureADOptions;
        }

        public override void InitializeServiceClient<T>(ServiceClient<T> client)
        {
            InternalInitializeServiceClient();
        }
        private void InternalInitializeServiceClient()
        {
            var authenticationContext =
                new AuthenticationContext(
                    (String.IsNullOrEmpty(azureADOptions.Authority) ? kAuthority : azureADOptions.Authority) + azureADOptions.TenantId);

            ClientCredential clientCredential = new ClientCredential(
                clientId: azureADOptions.ClientId,
                clientSecret: azureADOptions.ClientSecret);

            var result = authenticationContext.AcquireTokenAsync(
                        resource: gdsVaultOptions.ResourceId,
                        clientCredential: clientCredential).GetAwaiter().GetResult();

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            ExpiresOn = result.ExpiresOn;
            AuthenticationToken = result.AccessToken;
        }

        public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            DateTime now = DateTime.UtcNow;
            if (now.Add(TimeSpan.FromMinutes(2)) >= ExpiresOn)
            {
                InternalInitializeServiceClient();
            }

            if (AuthenticationToken == null)
            {
                throw new InvalidOperationException("Token Provider Cannot Be Null");
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticationToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //request.Version = new Version(apiVersion);
            await base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }

}