// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IIoT.OpcUa.Api.Registry;
using Microsoft.Azure.IIoT.OpcUa.Api.Registry.Models;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Auth;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Filters;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Models;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Controllers
{
    /// <summary>
    /// Application services.
    /// </summary>
    [ApiController]
    [Route(VersionInfo.PATH + "/reg"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    [Produces("application/json")]
    [Authorize(Policy = Policies.CanRead)]
    public sealed class RegistryController : Controller
    {
        private readonly IApplicationsDatabase _applicationDatabase;
        private readonly IRegistryServiceApi _registryServiceApi;
        private readonly ILogger _logger;

        /// <inheritdoc/>
        public RegistryController(
            IApplicationsDatabase applicationDatabase,
            IRegistryServiceApi registryServiceApi,
            ILogger logger)
        {
            _applicationDatabase = applicationDatabase;
            _registryServiceApi = registryServiceApi;
            _logger = logger;
        }

        /// <summary>
        /// List applications.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>The application records</returns>
        [HttpGet("find/{applicationUri}")]
        public async Task<QueryApplicationsResponseApiModel> ListApplicationsAsync()
        {
            var modelResult = new List<ApplicationRecordApiModel>();
            var query = new ApplicationRegistrationQueryApiModel()
            {
                //ApplicationUri = applicationUri
            };
            foreach (var record in await _registryServiceApi.QueryAllApplicationsAsync(query))
            {
                var resultRecord = new ApplicationRecordApiModel()
                {
                    ApplicationUri = record.ApplicationUri
                };
                modelResult.Add(new ApplicationRecordApiModel(resultRecord));
            }
            return new QueryApplicationsResponseApiModel(modelResult, null);
        }

        /// <summary>
        /// Get the status.
        /// </summary>
        [HttpGet]
        public Task<StatusResponseApiModel> GetStatusAsync()
        {
            return _registryServiceApi.GetServiceStatusAsync();
        }

    }
}
