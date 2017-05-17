// Copyright (c) Microsoft. All rights reserved.

using System.Web.Http;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.v1.Models;

namespace Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.v1.Controllers
{
    [RoutePrefix(Version.Name)]
    public class StatusController : ApiController
    {
        public StatusApiModel Get()
        {
            return new StatusApiModel { Message = "OK" };
        }
    }
}
