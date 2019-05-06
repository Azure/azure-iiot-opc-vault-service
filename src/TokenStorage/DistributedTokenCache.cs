// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

namespace Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.TokenStorage {
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Serilog;
    using System;
    using System.Security.Claims;

    public class DistributedTokenCache : TokenCache {

        /// <summary>
        /// Initializes a new instance of <see cref="DistributedTokenCache"/>
        /// </summary>
        /// <param name="claimsPrincipal">A <see cref="ClaimsPrincipal"/> for the signed in user</param>
        /// <param name="distributedCache">An implementation of <see cref="IDistributedCache"/> in which to store the access tokens.</param>
        /// <param name="logger">A <see cref="ILogger"/> instance.</param>
        /// <param name="dataProtectionProvider">An <see cref="IDataProtectionProvider"/> for creating a data protector.</param>
        public DistributedTokenCache(
            ClaimsPrincipal claimsPrincipal,
            IDistributedCache distributedCache,
            ILogger logger,
            IDataProtectionProvider dataProtectionProvider) {
            _claimsPrincipal = claimsPrincipal;
            _cacheKey = BuildCacheKey(_claimsPrincipal);
            _distributedCache = distributedCache;
            _logger = logger;
            _protector = dataProtectionProvider.CreateProtector(typeof(DistributedTokenCache).FullName);
            AfterAccess = AfterAccessNotification;
            LoadFromCache();
        }

        /// <summary>
        /// Builds the cache key to use for this item in the distributed cache.
        /// </summary>
        /// <param name="claimsPrincipal">A <see cref="ClaimsPrincipal"/> for the signed in user</param>
        /// <returns>Cache key for this item.</returns>
        private static string BuildCacheKey(ClaimsPrincipal claimsPrincipal) => string.Format(
                "UserId:{0}",
                claimsPrincipal.Identity.Name);

        /// <summary>
        /// Attempts to load tokens from distributed cache.
        /// </summary>
        private void LoadFromCache() {
            var cacheData = _distributedCache.Get(_cacheKey);
            if (cacheData != null) {
                Deserialize(_protector.Unprotect(cacheData));
                _logger.TokensRetrievedFromStore(_cacheKey);
            }
        }

        /// <summary>
        /// Handles the AfterAccessNotification event, which is triggered right after ADAL accesses the cache.
        /// </summary>
        /// <param name="args">An instance of <see cref="TokenCacheNotificationArgs"/> containing information for this event.</param>
        public void AfterAccessNotification(TokenCacheNotificationArgs args) {
            if (HasStateChanged) {
                try {
                    if (Count > 0) {
                        _distributedCache.Set(_cacheKey, _protector.Protect(Serialize()));
                        _logger.TokensWrittenToStore(args.ClientId, args.UniqueId, args.Resource);
                    }
                    else {
                        // There are no tokens for this user/client, so remove them from the cache.
                        // This was previously handled in an overridden Clear() method, but the built-in Clear() calls this
                        // after the dictionary is cleared.
                        _distributedCache.Remove(_cacheKey);
                        _logger.TokenCacheCleared(_claimsPrincipal.Identity.Name ?? "<none>");
                    }
                    HasStateChanged = false;
                }
                catch (Exception exp) {
                    _logger.WriteToCacheFailed(exp);
                    throw;
                }
            }
        }
        private readonly ClaimsPrincipal _claimsPrincipal;
        private readonly ILogger _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly IDataProtector _protector;
        private readonly string _cacheKey;
    }
}
