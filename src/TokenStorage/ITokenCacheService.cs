// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

namespace Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.TokenStorage {
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface ITokenCacheService {
        /// <summary>
        /// Returns an instance of <see cref="TokenCache"/>.
        /// </summary>
        /// <param name="claimsPrincipal">Current user's <see cref="ClaimsPrincipal"/>.</param>
        /// <returns>An instance of <see cref="TokenCache"/>.</returns>
        Task<TokenCache> GetCacheAsync(ClaimsPrincipal claimsPrincipal);

        /// <summary>
        /// Clears the token cache.
        /// </summary>
        /// <param name="claimsPrincipal">Current user's <see cref="ClaimsPrincipal"/>.</param>
        Task ClearCacheAsync(ClaimsPrincipal claimsPrincipal);
    }
}
