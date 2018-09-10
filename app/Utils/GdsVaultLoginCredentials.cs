﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.App.TokenStorage;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.App.Utils
{
    public class GdsVaultLoginCredentials : ServiceClientCredentials
    {
        private GdsVaultOptions gdsVaultOptions;
        private AzureADOptions azureADOptions;
        private ITokenCacheService tokenCacheService;
        private ClaimsPrincipal claimsPrincipal;

        public GdsVaultLoginCredentials(
            GdsVaultOptions gdsVaultOptions,
            AzureADOptions azureADOptions,
            ITokenCacheService tokenCacheService,
            ClaimsPrincipal claimsPrincipal)
        {
            this.gdsVaultOptions = gdsVaultOptions;
            this.azureADOptions = azureADOptions;
            this.tokenCacheService = tokenCacheService;
            this.claimsPrincipal = claimsPrincipal;
        }
        private string AuthenticationToken { get; set; }
        public override void InitializeServiceClient<T>(ServiceClient<T> client)
        {
            var tokenCache = tokenCacheService.GetCacheAsync(claimsPrincipal).Result;

            var authenticationContext =
                new AuthenticationContext(azureADOptions.Instance + azureADOptions.TenantId);

            var credential = new ClientCredential(
                clientId: azureADOptions.ClientId,
                clientSecret: azureADOptions.ClientSecret);

            var result = authenticationContext.AcquireTokenAsync(
                resource: gdsVaultOptions.ResourceId,
                clientCredential: credential).Result;

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            AuthenticationToken = result.AccessToken;
        }

        public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
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
