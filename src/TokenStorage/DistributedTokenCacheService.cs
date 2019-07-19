// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

namespace Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.TokenStorage {
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Serilog;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class DistributedTokenCacheService : TokenCacheService {

        /// <summary>
        /// Create service
        /// </summary>
        /// <param name="distributedCache"></param>
        /// <param name="logger"></param>
        /// <param name="dataProtectionProvider"></param>
        public DistributedTokenCacheService(IDistributedCache distributedCache,
            ILogger logger, IDataProtectionProvider dataProtectionProvider)
            : base(logger) {
            _distributedCache = distributedCache;
            _dataProtectionProvider = dataProtectionProvider;
        }

        /// <summary>
        /// Returns an instance of <see cref="TokenCache"/>.
        /// </summary>
        /// <param name="claimsPrincipal">Current user's <see cref="ClaimsPrincipal"/>.</param>
        /// <returns>An instance of <see cref="TokenCache"/>.</returns>
        public override Task<TokenCache> GetCacheAsync(ClaimsPrincipal claimsPrincipal) {
            if (_cache == null) {
                _cache = new DistributedTokenCache(claimsPrincipal, _distributedCache, _logger, _dataProtectionProvider);
            }
            return Task.FromResult(_cache);
        }

        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IDistributedCache _distributedCache;
    }
}
