// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.IIoT.Auth;
using Microsoft.Azure.IIoT.Auth.Clients;
using Microsoft.Azure.IIoT.Auth.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Auth
{

    /// <summary> 
    /// The token provider for the service to service authentication.
    /// </summary>
    public class IIoTTokenProvider : ITokenProvider
    {
        ClientCredential _clientCredential;
        IHttpContextAccessor _ctx;
        string _authority;
        private const string _kAuthority = "https://login.microsoftonline.com/";
        private const string _kGrantType = "urn:ietf:params:oauth:grant-type:jwt-bearer";

        public IIoTTokenProvider(
            IHttpContextAccessor ctx,
            IClientConfig clientConfig
            )
        {
            _ctx = ctx;
            _authority = String.IsNullOrEmpty(clientConfig.InstanceUrl) ? _kAuthority : clientConfig.InstanceUrl;
            if (!_authority.EndsWith("/"))
            {
                _authority += "/";
            }
            _authority += clientConfig.TenantId;
            if (!string.IsNullOrEmpty(clientConfig.AppId) &&
                !string.IsNullOrEmpty(clientConfig.AppSecret))
            {
                _clientCredential = new ClientCredential(clientConfig.AppId, clientConfig.AppSecret);
            }
        }

        /// <summary>
        /// Obtain token from user
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="scopes"></param>
        /// <returns></returns>
        public async Task<TokenResultModel> GetTokenForAsync(string resource,
            IEnumerable<string> scopes)
        {
            if (_clientCredential == null)
            {
                return null;
            }

            var user = _ctx.HttpContext.User;
            // User id should be known, we need it to sign in on behalf of...
            if (user == null)
            {
                throw new AuthenticationException("Missing claims principal.");
            }

            string name = user.FindFirstValue(ClaimTypes.Upn);
            if (String.IsNullOrEmpty(name))
            {
                name = user.FindFirstValue(ClaimTypes.Email);
            }

            const string kAccessTokenKey = "access_token";
            var token = await _ctx.HttpContext.GetTokenAsync(kAccessTokenKey);
            if (string.IsNullOrEmpty(token))
            {
                // TODO: The above always fails currently. Find out what we do wrongly.
                // This is the 1.1 workaround...
                token = user?.FindFirstValue(kAccessTokenKey);
                if (string.IsNullOrEmpty(token))
                {
                    throw new AuthenticationException(
                        $"No auth on behalf of {name} without token...");
                }
            }

            var ctx = new AuthenticationContext(_authority, TokenCache.DefaultShared);

            try
            {
                var result = await ctx.AcquireTokenAsync(resource,
                    _clientCredential,
                    new UserAssertion(token, _kGrantType, name));
                return result.ToTokenResult();
            }
            catch (AdalException ex)
            {
                throw new AuthenticationException(
                    $"Failed to authenticate on behalf of {name}", ex);
            }
        }

        /// <summary>
        /// Simplified token generation without user impersonation.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="scopes"></param>
        /// <returns></returns>
        public async Task<TokenResultModel> GetServiceTokenForAsync(string resource, IEnumerable<string> scopes = null)
        {
            var context = new AuthenticationContext(_authority, TokenCache.DefaultShared);
            AuthenticationResult result = await context.AcquireTokenAsync(resource, _clientCredential);
            var jwt = new JwtSecurityToken(result.AccessToken);
            return new TokenResultModel
            {
                RawToken = result.AccessToken,
                SignatureAlgorithm = jwt.SignatureAlgorithm,
                Authority = result.Authority,
                TokenType = result.AccessTokenType,
                ExpiresOn = result.ExpiresOn,
                TenantId = result.TenantId,
                IdToken = result.IdToken
            };
        }

        public Task InvalidateAsync(string resource)
        {
            return Task.CompletedTask;
        }
    }
}
