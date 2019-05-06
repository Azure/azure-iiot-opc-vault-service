// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

namespace Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.TokenStorage {
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Serilog;
    using System.Security.Claims;
    using System.Threading.Tasks;

    /// <summary>
    /// Returns and manages the instance of token cache to be used when making use of ADAL.
    /// </summary>
    public abstract class TokenCacheService : ITokenCacheService {

        /// <summary>
        /// Initializes a new instance of TokenCacheService
        /// </summary>
        protected TokenCacheService(ILogger logger) => _logger = logger;

        /// <summary>
        /// Returns an instance of <see cref="TokenCache"/>.
        /// </summary>
        /// <param name="claimsPrincipal">Current user's <see cref="ClaimsPrincipal"/>.</param>
        /// <returns>An instance of <see cref="TokenCache"/>.</returns>
        public abstract Task<TokenCache> GetCacheAsync(ClaimsPrincipal claimsPrincipal);

        /// <summary>
        /// Clears the token cache.
        /// </summary>
        /// <param name="claimsPrincipal">Current user's <see cref="ClaimsPrincipal"/>.</param>
        public virtual async Task ClearCacheAsync(ClaimsPrincipal claimsPrincipal) {
            var cache = await GetCacheAsync(claimsPrincipal);
            cache.Clear();
        }
        protected readonly ILogger _logger;
        protected TokenCache _cache;
    }
}

