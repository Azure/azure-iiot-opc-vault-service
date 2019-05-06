// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

namespace Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.Utils {
    using Microsoft.AspNetCore.Authentication.AzureAD.UI;
    using Microsoft.Azure.IIoT.OpcUa.Api.Vault;
    using Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.TokenStorage;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Rest;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;

    public class OpcVaultLoginCredentials : ServiceClientCredentials {

        public OpcVaultLoginCredentials(
            OpcVaultApiOptions opcVaultOptions,
            AzureADOptions azureADOptions,
            ITokenCacheService tokenCacheService,
            ClaimsPrincipal claimsPrincipal) {
            _opcVaultOptions = opcVaultOptions;
            _azureADOptions = azureADOptions;
            _tokenCacheService = tokenCacheService;
            _claimsPrincipal = claimsPrincipal;
        }

        public override void InitializeServiceClient<T>(ServiceClient<T> client) {
            var tokenCache = _tokenCacheService.GetCacheAsync(_claimsPrincipal).Result;

            var authenticationContext =
                new AuthenticationContext(_azureADOptions.Instance + _azureADOptions.TenantId, tokenCache);

            var credential = new ClientCredential(
                _azureADOptions.ClientId,
                _azureADOptions.ClientSecret);

            var name = _claimsPrincipal.FindFirstValue(ClaimTypes.Upn) ??
                _claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            var userObjectId = (_claimsPrincipal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;
            var user = new UserIdentifier(userObjectId, UserIdentifierType.UniqueId);

            var result = authenticationContext.AcquireTokenSilentAsync(
                        _opcVaultOptions.ResourceId,
                        credential,
                        userId: user).GetAwaiter().GetResult();

            if (result == null) {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            _authenticationToken = result.AccessToken;
        }

        public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }

            if (_authenticationToken == null) {
                throw new InvalidOperationException("Token Provider Cannot Be Null");
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            await base.ProcessHttpRequestAsync(request, cancellationToken);
        }

        private readonly OpcVaultApiOptions _opcVaultOptions;
        private readonly AzureADOptions _azureADOptions;
        private readonly ITokenCacheService _tokenCacheService;
        private readonly ClaimsPrincipal _claimsPrincipal;
        private string _authenticationToken;
    }
}
