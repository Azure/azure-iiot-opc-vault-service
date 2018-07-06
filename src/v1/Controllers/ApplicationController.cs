// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IIoT.OpcUa.Services.Gds.v1.Filters;
using Microsoft.Azure.IIoT.OpcUa.Services.Gds.v1.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.v1.Controllers
{
    [Route(VersionInfo.PATH + "/app"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    [Produces("application/json")]
    public sealed class ApplicationController : Controller
    {
        private readonly IApplicationsDatabase _applicationDatabase;

        public ApplicationController(IApplicationsDatabase applicationDatabase)
        {
            this._applicationDatabase = applicationDatabase;
        }

        /// <summary>
        /// Register new application.
        /// </summary>
        [HttpPost]
        [SwaggerOperation(operationId: "RegisterApplication")]
        public async Task<string> RegisterApplicationAsync([FromBody] ApplicationRecordApiModel application)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }
            return await _applicationDatabase.RegisterApplicationAsync(application.ToServiceModel());
        }

        /// <summary>
        /// Update application.
        /// </summary>
        [HttpPut("{id}")]
        [SwaggerOperation(operationId: "UpdateApplication")]
        public async Task<string> UpdateApplicationAsync(string id, [FromBody] ApplicationRecordApiModel application)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }
            return await _applicationDatabase.UpdateApplicationAsync(id, application.ToServiceModel());
        }

        /// <summary>
        /// Unregister application
        /// </summary>
        [HttpDelete("{id}")]
        [SwaggerOperation(operationId: "UnregisterApplication")]
        public async Task UnregisterApplicationAsync(string id)
        {
            await _applicationDatabase.UnregisterApplicationAsync(id);
        }

        /// <summary>Get application</summary>
        [HttpGet("{id}")]
        [SwaggerOperation(operationId: "GetApplication")]
        public async Task<ApplicationRecordApiModel> GetApplicationAsync(string id)
        {
            return new ApplicationRecordApiModel(await _applicationDatabase.GetApplicationAsync(id));
        }

        /// <summary>Find applications</summary>
        [HttpGet("find/{uri}")]
        [SwaggerOperation(operationId: "FindApplication")]
        public async Task<ApplicationRecordApiModel[]> FindApplicationAsync(string uri)
        {
            var modelResult = new List<ApplicationRecordApiModel>();
            foreach (var record in await _applicationDatabase.FindApplicationAsync(uri))
            {
                modelResult.Add(new ApplicationRecordApiModel(record));
            }
            return modelResult.ToArray();
        }

        /// <summary>Query applications</summary>
        [HttpPost("query")]
        [SwaggerOperation(operationId: "QueryApplications")]
        public async Task<QueryApplicationsResponseApiModel> QueryApplicationsAsync([FromBody] QueryApplicationsApiModel query)
        {
            DateTime lastCounterResetTime;
            uint nextRecordId;
            var result = await _applicationDatabase.QueryApplicationsAsync(
                query.StartingRecordId,
                query.MaxRecordsToReturn,
                query.ApplicationName,
                query.ApplicationUri,
                query.ApplicationType,
                query.ProductUri,
                query.ServerCapabilities,
                out lastCounterResetTime,
                out nextRecordId
                );
            return new QueryApplicationsResponseApiModel(result, lastCounterResetTime, nextRecordId);
        }

        [HttpPost("servers")]
        [SwaggerOperation(operationId: "QueryServers")]
        public async Task<QueryServersResponseApiModel> QueryServersAsync([FromBody] QueryServersApiModel query)
        {
            DateTime lastCounterResetTime;
            uint nextRecordId;
            var result = await _applicationDatabase.QueryApplicationsAsync(
                query.StartingRecordId,
                query.MaxRecordsToReturn,
                query.ApplicationName,
                query.ApplicationUri,
                0,
                query.ProductUri,
                query.ServerCapabilities,
                out lastCounterResetTime,
                out nextRecordId
                );
            return new QueryServersResponseApiModel(result, lastCounterResetTime);
        }
    }
}
