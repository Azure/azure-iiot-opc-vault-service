﻿// Copyright (c) Microsoft. All rights reserved.


namespace Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Controllers
{
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IoTSolutions.GdsVault.Common.Diagnostics;
using Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Filters;
using Microsoft.Azure.IoTSolutions.GdsVault.WebService.v1.Models;

    [Route(ServiceInfo.PATH + "/[controller]"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    [Produces("application/json")]

    public sealed class StatusController : Controller
    {
        private readonly ILogger log;

        public StatusController(ILogger logger)
        {
            this.log = logger;
        }

        [HttpGet]
        public StatusApiModel Get()
        {
            // TODO: calculate the actual service status
            var isOk = true;

            this.log.Info("Service status request", () => new { Healthy = isOk });
            return new StatusApiModel(isOk, "Alive and well");
        }
    }
}