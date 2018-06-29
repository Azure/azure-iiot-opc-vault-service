// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IoTSolutions.GdsVault.Services;
using Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Filters;
using Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Models;

namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Controllers
{
    [Route(ServiceInfo.PATH + "/app"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    [Produces("application/json")]

    public sealed class ApplicationController : Controller
    {
        //private readonly IApplicationService _applicationService;

        public ApplicationController(/*IApplicationService applicationService*/)
        {
            //this._applicationService = applicationService;
        }

        /// <summary>Query applications</summary>
        [HttpGet("query")]
        public async Task<QueryApplicationsResponseApiModel> QueryApplicationsAsync([FromBody] QueryApplicationsApiModel query)
        {
            return null;
        }

        /// <summary>Get application</summary>
        [HttpGet("{id}")]
        public async Task<ApplicationRecordApiModel> GetApplicationAsync(string id)
        {
            return null;
        }

        /// <summary>Find applications</summary>
        [HttpGet("find/{uri}")]
        public async Task<ApplicationRecordApiModel[]> FindApplicationAsync(string uri)
        {
            return null;
        }

        /// <summary>Query servers.</summary>
        [HttpGet("servers")]
        public async Task<QueryServersResponseApiModel> QueryServersAsync([FromBody] QueryServersApiModel query)
        {
            return null;
        }

        /// <summary>
        /// Register new application.
        /// </summary>
        [HttpPut]
        public async Task<string> RegisterAsync([FromBody] ApplicationRecordApiModel application)
        {
            return null;
        }


        /// <summary>
        /// Update application.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<string> UpdateAsync(string id, [FromBody] ApplicationRecordApiModel application)
        {
            return null;
        }

        /// <summary>
        /// Unregister application
        /// </summary>
        [HttpDelete("{id}")]
        public async Task UnregisterAsync(string id)
        {

        }

    }
}
