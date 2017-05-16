// Copyright (c) Microsoft. All rights reserved.

using System.Web.Http;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.v1.Models;

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.v1.Controllers
{
    [RoutePrefix(Version.Name)]
    public class StatusController : ApiController
    {
        /// <summary>Return the service status</summary>
        /// <returns>Status object</returns>
        public StatusApiModel Get()
        {
            return new StatusApiModel { Message = "OK" };
        }
    }
}
