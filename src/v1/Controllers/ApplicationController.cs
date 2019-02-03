// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Auth;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Filters;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Models;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Vault.v1.Controllers
{
    /// <inheritdoc/>
    [ApiController]
    [Route(VersionInfo.PATH + "/app"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    [Produces("application/json")]
    [Authorize(Policy = Policies.CanRead)]
    public sealed class ApplicationController : Controller
    {
        private readonly IApplicationsDatabase _applicationDatabase;

        /// <inheritdoc/>
        public ApplicationController(IApplicationsDatabase applicationDatabase)
        {
            _applicationDatabase = applicationDatabase;
        }

        /// <summary>
        /// Register new application.
        /// </summary>
        /// <remarks>
        /// After registration an application is in the 'New' state and needs
        /// approval by a manager to be avavilable for certificate operation.
        /// Requires Writer role.
        /// </remarks>
        /// <param name="application">The new application</param>
        /// <returns>The registered application record</returns>
        [HttpPost("register")]
        [Authorize(Policy = Policies.CanWrite)]
        public async Task<ApplicationRecordApiModel> RegisterApplicationAsync([FromBody] ApplicationRecordApiModel application)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }
            var applicationServiceModel = application.ToServiceModel();
            applicationServiceModel.AuthorityId = User.Identity.Name;
            return new ApplicationRecordApiModel(await _applicationDatabase.RegisterApplicationAsync(applicationServiceModel));
        }

        /// <summary>
        /// Get application.
        /// </summary>
        /// <param name="applicationId">The application id</param>
        /// <returns>The application record</returns>
        [HttpGet("{applicationId}")]
        public async Task<ApplicationRecordApiModel> GetApplicationAsync(string applicationId)
        {
            return new ApplicationRecordApiModel(await _applicationDatabase.GetApplicationAsync(applicationId));
        }

        /// <summary>
        /// Update application.
        /// </summary>
        /// <remarks>
        /// Update the application with given application id, however state information is unchanged.
        /// Requires Writer role.
        /// </remarks>
        /// <param name="application">The updated application</param>
        /// <returns>The updated application record</returns>
        [HttpPatch("{applicationId}")]
        [Authorize(Policy = Policies.CanWrite)]
        public async Task<ApplicationRecordApiModel> UpdateApplicationAsync([FromBody] ApplicationRecordApiModel application)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }
            var applicationServiceModel = application.ToServiceModel();
            applicationServiceModel.AuthorityId = User.Identity.Name;
            return new ApplicationRecordApiModel(await _applicationDatabase.UpdateApplicationAsync(application.ApplicationId, applicationServiceModel));
        }

        /// <summary>
        /// Approve or reject a new application.
        /// <remarks>
        /// A manager can approve a new application or force an application from any state.
        /// After approval the application is in the 'Approved' or 'Rejected' state.
        /// Requires Manager role.
        /// </remarks>
        /// </summary>
        /// <param name="applicationId">The application id</param>
        /// <param name="approved">approve or reject the new application</param>
        /// <param name="force">optional, force application in new state</param>
        /// <returns>The updated application record</returns>
        [HttpPost("{applicationId}/{approved}/approve")]
        [Authorize(Policy = Policies.CanManage)]
        public async Task<ApplicationRecordApiModel> ApproveApplicationAsync(string applicationId, bool approved, bool? force)
        {
            return new ApplicationRecordApiModel(await _applicationDatabase.ApproveApplicationAsync(applicationId, approved, force ?? false));
        }

        /// <summary>
        /// Unregister application.
        /// </summary>
        /// Unregisters the application record and all associated information.
        /// The application record remains in the database in 'Unregistered' state.
        /// Certificate Requests associated with the application id are set to the 'Deleted' state,
        /// and will be revoked with the next CRL update.
        /// Requires Writer role.
        /// <param name="applicationId">The application id</param>
        [HttpDelete("{applicationId}/unregister")]
        [Authorize(Policy = Policies.CanWrite)]
        public async Task UnregisterApplicationAsync(string applicationId)
        {
            await _applicationDatabase.UnregisterApplicationAsync(applicationId);
        }

        /// <summary>
        /// Delete application.
        /// </summary>
        /// Deletes the application record.
        /// Certificate Requests associated with the application id are set in the deleted state,
        /// and will be revoked with the next CRL update.
        /// Requires Manager role.
        /// <param name="applicationId">The application id</param>
        /// <param name="force">optional, skip sanity checks and force to delete application</param>
        [HttpDelete("{applicationId}")]
        [Authorize(Policy = Policies.CanManage)]
        public async Task DeleteApplicationAsync(string applicationId, bool? force)
        {
            await _applicationDatabase.DeleteApplicationAsync(applicationId, force ?? false);
        }

        /// <summary>
        /// List applications with matching appllicatin Uri.
        /// </summary>
        /// <param name="applicationUri">The application Uri</param>
        /// <returns>The application records</returns>
        [HttpGet("find/{uri}")]
        public async Task<IList<ApplicationRecordApiModel>> ListApplicationsAsync(string applicationUri)
        {
            var modelResult = new List<ApplicationRecordApiModel>();
            foreach (var record in await _applicationDatabase.ListApplicationAsync(applicationUri))
            {
                modelResult.Add(new ApplicationRecordApiModel(record));
            }
            return modelResult;
        }

        /// <summary>
        /// Query applications.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="anyState"></param>
        /// <returns></returns>
        [HttpPost("query")]
        public async Task<QueryApplicationsResponseApiModel> QueryApplicationsAsync(
            [FromBody] QueryApplicationsApiModel query,
            bool? anyState)
        {
            if (query == null)
            {
                // query all
                query = new QueryApplicationsApiModel(0, 0, null, null, 0, null, null);
            }
            var result = await _applicationDatabase.QueryApplicationsAsync(
                query.StartingRecordId,
                query.MaxRecordsToReturn,
                query.ApplicationName,
                query.ApplicationUri,
                (uint)query.ApplicationType,
                query.ProductUri,
                query.ServerCapabilities,
                anyState
                );
            return new QueryApplicationsResponseApiModel(result);
        }

        /// <summary>Query applications</summary>
        [HttpPost("query/page")]
        public async Task<QueryApplicationsPageResponseApiModel> QueryApplicationsPageAsync(
            [FromBody] QueryApplicationsPageApiModel query,
            bool? anyState)
        {
            if (query == null)
            {
                // query all
                query = new QueryApplicationsPageApiModel(null, null, 0, null, null);
            }
            var result = await _applicationDatabase.QueryApplicationsPageAsync(
                query.ApplicationName,
                query.ApplicationUri,
                (uint)query.ApplicationType,
                query.ProductUri,
                query.ServerCapabilities,
                query.NextPageLink,
                query.MaxRecordsToReturn,
                anyState);
            return new QueryApplicationsPageResponseApiModel(result);
        }

    }
}
