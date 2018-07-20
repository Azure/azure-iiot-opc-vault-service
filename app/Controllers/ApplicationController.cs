// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api.Models;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly IOpcGdsVault gdsVault;
        public ApplicationController(IOpcGdsVault gdsVault)
        {
            this.gdsVault = gdsVault;
        }


        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            var applicationQuery = new QueryApplicationsApiModel();
            var applications = await gdsVault.QueryApplicationsAsync(applicationQuery);
            return View(applications.Applications);
        }


#pragma warning disable 1998
        [ActionName("Register")]
        public async Task<ActionResult> RegisterAsync()
        {
            return View();
        }
#pragma warning restore 1998

        [HttpPost]
        [ActionName("Register")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterAsync(
            [Bind("ApplicationUri,ApplicationName,ApplicationType,ProductUri,ServerCapabilities")]
            ApplicationRecordApiModel application)
        {
            if (ModelState.IsValid)
            {
                await gdsVault.RegisterApplicationAsync(application);
                return RedirectToAction("Index");
            }

            return View(application);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(
            [Bind("ApplicationId,ApplicationName,ApplicationType,ProductUri,ServerCapabilities")]
            ApplicationRecordApiModel newApplication)
        {
            if (ModelState.IsValid)
            {
                var application = await gdsVault.GetApplicationAsync(newApplication.ApplicationId);
                if (application == null)
                {
                    return new NotFoundResult();
                }

                application.ApplicationName = newApplication.ApplicationName;
                application.ApplicationType = newApplication.ApplicationType;
                application.ProductUri = newApplication.ProductUri;
                application.ServerCapabilities = newApplication.ServerCapabilities;

                await gdsVault.UpdateApplicationAsync(application.ApplicationId, application);
                return RedirectToAction("Index");
            }

            return View(newApplication);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return new BadRequestResult();
            }

            var application = await gdsVault.GetApplicationAsync(id);
            if (application == null)
            {
                return new NotFoundResult();
            }

            return View(application);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return new BadRequestResult();
            }

            var application = await gdsVault.GetApplicationAsync(id);
            if (application == null)
            {
                return new NotFoundResult();
            }

            return View(application);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("Id")] string id)
        {

            await gdsVault.UnregisterApplicationAsync(id);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            var application = await gdsVault.GetApplicationAsync(id);
            if (application == null)
            {
                return new NotFoundResult();
            }
            return View(application);
        }

    }
}