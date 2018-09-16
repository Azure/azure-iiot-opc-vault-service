// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api.Models;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.App.TokenStorage;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.App.Utils;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.App.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private IOpcGdsVault gdsVault;
        private readonly GdsVaultOptions gdsVaultOptions;
        private readonly AzureADOptions azureADOptions;
        private readonly ITokenCacheService tokenCacheService;

        public ApplicationController(
            GdsVaultOptions gdsVaultOptions,
            AzureADOptions azureADOptions,
            ITokenCacheService tokenCacheService)
        {
            this.gdsVaultOptions = gdsVaultOptions;
            this.azureADOptions = azureADOptions;
            this.tokenCacheService = tokenCacheService;
        }


        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            AuthorizeGdsVaultClient();
            var applicationQuery = new QueryApplicationsApiModel();
            var applications = await gdsVault.QueryApplicationsAsync(applicationQuery);
            return View(applications.Applications);
        }

        [ActionName("Register")]
        public Task<ActionResult> RegisterAsync()
        {
            var application = new ApplicationRecordApiModel();
            application.ApplicationNames = new List<ApplicationNameApiModel>();
            application.DiscoveryUrls = new List<string>();
            application.DiscoveryUrls.Add("");
            var appRegisterModel = new ApplicationRecordRegisterApiModel()
            {
                ApiModel = application
            };
            return Task.FromResult<ActionResult>(View(appRegisterModel));
        }

        [HttpPost]
        [ActionName("Register")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterAsync(
            ApplicationRecordRegisterApiModel appRegisterModel)
        {
            var application = appRegisterModel.ApiModel;
            if (ModelState.IsValid)
            {
                if (application.ApplicationNames == null)
                {
                    application.ApplicationNames = new List<ApplicationNameApiModel>();
                }
                if (application.ApplicationNames.Count == 0)
                {
                    application.ApplicationNames.Add(new ApplicationNameApiModel(null, application.ApplicationName));
                }
                AuthorizeGdsVaultClient();
                await gdsVault.RegisterApplicationAsync(application);
                return RedirectToAction("Index");
            }

            return View(appRegisterModel);
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
                AuthorizeGdsVaultClient();
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
            AuthorizeGdsVaultClient();
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
            AuthorizeGdsVaultClient();
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
            AuthorizeGdsVaultClient();
            await gdsVault.UnregisterApplicationAsync(id);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            AuthorizeGdsVaultClient();
            var application = await gdsVault.GetApplicationAsync(id);
            if (application == null)
            {
                return new NotFoundResult();
            }
            return View(application);
        }

        private void AuthorizeGdsVaultClient()
        {
            if (gdsVault == null)
            {
                ServiceClientCredentials serviceClientCredentials =
                    new GdsVaultLoginCredentials(gdsVaultOptions, azureADOptions, tokenCacheService, User);
                gdsVault = new OpcGdsVault(new Uri(gdsVaultOptions.BaseAddress), serviceClientCredentials);
            }
        }

    }
}