// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Azure.IIoT.OpcUa.Api.Vault;
using Microsoft.Azure.IIoT.OpcUa.Api.Vault.Models;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.App.Models;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.App.TokenStorage;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.App.Utils;
using Microsoft.Rest;
using Opc.Ua.Gds.Client;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Vault.App.Controllers
{
    [Authorize]
    public class CertomatController : Controller
    {
        const int ApplicationTypeClient = 1;
        private IOpcVault opcVault;
        private readonly OpcVaultApiOptions opcVaultOptions;
        private readonly AzureADOptions azureADOptions;
        private readonly ITokenCacheService tokenCacheService;

        public CertomatController(
            OpcVaultApiOptions opcVaultOptions,
            AzureADOptions azureADOptions,
            ITokenCacheService tokenCacheService)
        {
            this.opcVaultOptions = opcVaultOptions;
            this.azureADOptions = azureADOptions;
            this.tokenCacheService = tokenCacheService;
        }

        [ActionName("Register")]
        public async Task<IActionResult> RegisterAsync(string applicationId)
        {
            var apiModel = new ApplicationRecordApiModel();
            AuthorizeClient();
            if (applicationId != null)
            {
                try
                {
                    apiModel = await opcVault.GetApplicationAsync(applicationId);
                    ViewData["SuccessMessage"] =
                        "Application with id " + applicationId + " successfully loaded.";
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] =
                        "An application with id " + applicationId + " could not be found in the database.<br/>" +
                        "Message:" + ex.Message;
                }
            }
            UpdateApiModel(apiModel);
            return View(new ApplicationRecordRegisterApiModel(apiModel));
        }

        [HttpPost]
        [ActionName("Register")]
        [ValidateAntiForgeryToken]
        [ApplicationRecordRegisterApiModel]
        public async Task<ActionResult> RegisterAsync(
            ApplicationRecordRegisterApiModel apiModel,
            string find,
            string reg,
            string add,
            string del,
            string req)
        {
            string command = null;
            if (!String.IsNullOrEmpty(find)) { command = "find"; }
            if (!String.IsNullOrEmpty(add)) { command = "add"; }
            if (!String.IsNullOrEmpty(del)) { command = "delete"; }
            if (!String.IsNullOrEmpty(reg)) { command = "register"; }
            if (!String.IsNullOrEmpty(req)) { command = "request"; }

            UpdateApiModel(apiModel);

            if (ModelState.IsValid &&
                command == "request" &&
                apiModel.ApplicationId != null)
            {
                return RedirectToAction("Request", new { applicationId = apiModel.ApplicationId });
            }

            if (ModelState.IsValid &&
                command == "register")
            {
                AuthorizeClient();
                try
                {
                    if (apiModel.ApplicationType == ApplicationTypeClient)
                    {
                        apiModel.ServerCapabilities = null;
                        apiModel.DiscoveryUrls = null;
                    }
                    apiModel.ApplicationId = await opcVault.RegisterApplicationAsync(apiModel);
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] =
                        "The application registration failed.<br/>" +
                        "Message: " + ex.Message;
                    return View(apiModel);
                }
                return RedirectToAction("Request", new { applicationId = apiModel.ApplicationId });
            }

            if (command == "find")
            {
                AuthorizeClient();
                try
                {
                    var applications = await opcVault.FindApplicationAsync(apiModel.ApplicationUri);
                    if (applications == null || applications.Count == 0)
                    {
                        ViewData["ErrorMessage"] =
                            "Couldn't find the application with ApplicationUri " + apiModel.ApplicationUri;
                    }
                    else
                    {
                        return RedirectToAction("Register", new { applicationId = applications[0].ApplicationId });
                    }
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] =
                        "Failed to find the application with ApplicationUri" + apiModel.ApplicationUri + "<br/>" +
                        "Message:" + ex.Message;
                    return View(apiModel);
                }
            }

            if (command == "add")
            {
                apiModel.DiscoveryUrls.Add("");
            }

            return View(apiModel);
        }

        [ActionName("Request")]
        public async Task<IActionResult> RequestAsync(string applicationId)
        {
            AuthorizeClient();
            var application = await opcVault.GetApplicationAsync(applicationId);

            UpdateApiModel(application);
            return View(application);
        }

        [ActionName("StartNewKeyPair")]
        public async Task<ActionResult> StartNewKeyPairAsync(string id)
        {
            AuthorizeClient();
            var groups = await opcVault.GetCertificateGroupConfigurationCollectionAsync();
            if (groups == null)
            {
                return new NotFoundResult();
            }

            string defaultGroupId, defaultTypeId;
            if (groups.Groups.Count > 0)
            {
                defaultGroupId = groups.Groups[0].Name;
                defaultTypeId = groups.Groups[0].CertificateType;
            }
            else
            {
                return new NotFoundResult();
            }

            var application = await opcVault.GetApplicationAsync(id);
            if (application == null)
            {
                return new NotFoundResult();
            }

            ViewData["Application"] = application;
            ViewData["Groups"] = groups;

            var request = new StartNewKeyPairRequestApiModel()
            {
                ApplicationId = id,
                CertificateGroupId = defaultGroupId,
                CertificateTypeId = defaultTypeId
            };

            return View(request);
        }

        [HttpPost]
        [ActionName("StartNewKeyPair")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StartNewKeyPairAsync(
            StartNewKeyPairRequestApiModel request)
        {
            if (ModelState.IsValid)
            {
                AuthorizeClient();
                var id = await opcVault.StartNewKeyPairRequestAsync(request);
                string message = null;
                try
                {
                    await opcVault.ApproveCertificateRequestAsync(id, false);
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                return RedirectToAction("Details", new { id, message });
            }

            return View(request);
        }

        [ActionName("StartSigning")]
        public async Task<ActionResult> StartSigningAsync(string id)
        {
            AuthorizeClient();
            var groups = await opcVault.GetCertificateGroupConfigurationCollectionAsync();
            if (groups == null)
            {
                return new NotFoundResult();
            }

            string defaultGroupId, defaultTypeId;
            if (groups.Groups.Count > 0)
            {
                defaultGroupId = groups.Groups[0].Name;
                defaultTypeId = groups.Groups[0].CertificateType;
            }
            else
            {
                return new NotFoundResult();
            }

            var application = await opcVault.GetApplicationAsync(id);
            if (application == null)
            {
                return new NotFoundResult();
            }

            ViewData["Application"] = application;

            var request = new StartSigningRequestUploadApiModel()
            {
                ApiModel = new StartSigningRequestApiModel()
                {
                    ApplicationId = id,
                    CertificateGroupId = defaultGroupId,
                    CertificateTypeId = defaultTypeId
                }
            };

            return View(request);
        }

        [HttpPost]
        [ActionName("StartSigning")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StartSigningAsync(
            StartSigningRequestUploadApiModel request)
        {
            if (ModelState.IsValid && (request.CertificateRequestFile != null || request.ApiModel.CertificateRequest != null))
            {
                var requestApi = request.ApiModel;
                if (request.CertificateRequestFile != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await request.CertificateRequestFile.CopyToAsync(memoryStream);
                        requestApi.CertificateRequest = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
                AuthorizeClient();
                var id = await opcVault.StartSigningRequestAsync(requestApi);
                string message = null;
                try
                {
                    await opcVault.ApproveCertificateRequestAsync(id, false);
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                return RedirectToAction("Details", new { id, message });
            }

            return View(request);
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id, string message)
        {
            AuthorizeClient();
            var request = await opcVault.ReadCertificateRequestAsync(id);
            var model = new CertificateRequestRecordDetailsApiModel(request, message);
            ViewData["Message"] = message;
            return View(model);
        }

        [ActionName("Approve")]
        public async Task<ActionResult> ApproveAsync(string id)
        {
            AuthorizeClient();
            try
            {
                await opcVault.ApproveCertificateRequestAsync(id, false);
                return RedirectToAction("Details", new { id, message = "CertificateRequest approved!" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Details", new { id, message = ex.Message });
            }
        }

        [ActionName("Reject")]
        public async Task<ActionResult> RejectAsync(string id)
        {
            AuthorizeClient();
            try
            {
                await opcVault.ApproveCertificateRequestAsync(id, true);
                return RedirectToAction("Details", new { id, message = "CertificateRequest rejected!" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Details", new { id, message = ex.Message });
            }
        }

        [ActionName("Accept")]
        public async Task<ActionResult> AcceptAsync(string id)
        {
            AuthorizeClient();
            await opcVault.AcceptCertificateRequestAsync(id);
            return RedirectToAction("Details", new { id });
        }

        [ActionName("DownloadCertificate")]
        public async Task<ActionResult> DownloadCertificateAsync(string requestId, string applicationId)
        {
            AuthorizeClient();
            var result = await opcVault.FinishRequestAsync(requestId, applicationId);
            if (String.Compare(result.State, "Approved", StringComparison.OrdinalIgnoreCase) == 0 &&
                result.SignedCertificate != null)
            {
                var byteArray = Convert.FromBase64String(result.SignedCertificate);
                return new FileContentResult(byteArray, "application/pkix-cert")
                {
                    FileDownloadName = CertFileName(result.SignedCertificate) + ".der"
                };
            }
            return new NotFoundResult();
        }

        [ActionName("DownloadIssuer")]
        public async Task<ActionResult> DownloadIssuerAsync(string requestId)
        {
            AuthorizeClient();
            var request = await opcVault.ReadCertificateRequestAsync(requestId);
            if (request != null)
            {
                var issuer = await opcVault.GetCACertificateChainAsync(request.CertificateGroupId);
                var byteArray = Convert.FromBase64String(issuer.Chain[0].Certificate);
                return new FileContentResult(byteArray, ContentType.Cert)
                {
                    FileDownloadName = CertFileName(issuer.Chain[0].Certificate) + ".der"
                };
            }
            return new NotFoundResult();
        }

        [ActionName("DownloadIssuerCrl")]
        public async Task<ActionResult> DownloadIssuerCrlAsync(string requestId)
        {
            AuthorizeClient();
            var request = await opcVault.ReadCertificateRequestAsync(requestId);
            if (request != null)
            {
                var issuer = await opcVault.GetCACertificateChainAsync(request.CertificateGroupId);
                var crl = await opcVault.GetCACrlChainAsync(request.CertificateGroupId);
                var byteArray = Convert.FromBase64String(crl.Chain[0].Crl);
                return new FileContentResult(byteArray, ContentType.Crl)
                {
                    FileDownloadName = CertFileName(issuer.Chain[0].Certificate) + ".crl"
                };
            }
            return new NotFoundResult();
        }

        [ActionName("DownloadPrivateKey")]
        public async Task<ActionResult> DownloadPrivateKeyAsync(string requestId, string applicationId)
        {
            AuthorizeClient();
            var result = await opcVault.FinishRequestAsync(requestId, applicationId);
            if (String.Compare(result.State, "Approved", StringComparison.OrdinalIgnoreCase) == 0 &&
                result.PrivateKey != null)
            {
                if (String.Compare(result.PrivateKeyFormat, "PFX", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var byteArray = Convert.FromBase64String(result.PrivateKey);
                    return new FileContentResult(byteArray, ContentType.Pfx)
                    {
                        FileDownloadName = CertFileName(result.SignedCertificate) + ".pfx"
                    };
                }
                else if (String.Compare(result.PrivateKeyFormat, "PEM", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var byteArray = Convert.FromBase64String(result.PrivateKey);
                    return new FileContentResult(byteArray, ContentType.Pem)
                    {
                        FileDownloadName = CertFileName(result.SignedCertificate) + ".pem"
                    };
                }
            }
            return new NotFoundResult();
        }


        private string CertFileName(string signedCertificate)
        {
            try
            {
                var signedCertByteArray = Convert.FromBase64String(signedCertificate);
                X509Certificate2 cert = new X509Certificate2(signedCertByteArray);
                return cert.Subject + "[" + cert.Thumbprint + "]";
            }
            catch
            {
                return "Certificate";
            }
        }

        private void AuthorizeClient()
        {
            if (opcVault == null)
            {
                ServiceClientCredentials serviceClientCredentials =
                    new OpcVaultLoginCredentials(opcVaultOptions, azureADOptions, tokenCacheService, User);
                opcVault = new OpcVault(new Uri(opcVaultOptions.BaseAddress), serviceClientCredentials);
            }
        }

        private void UpdateApiModel(ApplicationRecordApiModel application)
        {
            if (application.ApplicationNames != null)
            {
                application.ApplicationNames = application.ApplicationNames.Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
            }
            else
            {
                application.ApplicationNames = new List<ApplicationNameApiModel>();
            }
            if (application.ApplicationNames.Count == 0)
            {
                application.ApplicationNames.Add(new ApplicationNameApiModel(null, application.ApplicationName));
            }
            else
            {
                application.ApplicationNames[0] = new ApplicationNameApiModel(null, application.ApplicationName);
            }
            if (application.DiscoveryUrls != null)
            {
                application.DiscoveryUrls = application.DiscoveryUrls.Where(x => !string.IsNullOrEmpty(x)).ToList();
            }
            else
            {
                application.DiscoveryUrls = new List<string>();
            }
            if (application.DiscoveryUrls.Count == 0)
            {
                application.DiscoveryUrls.Add("");
            }
        }
    }

    /// <summary>
    /// helper for model validation in registration form
    /// </summary>
    public class ApplicationRecordRegisterApiModelAttribute : ValidationAttribute, IClientModelValidator
    {
        ServerCapabilities _serverCaps = new ServerCapabilities();
        const int ApplicationTypeClient = 1;

        public ApplicationRecordRegisterApiModelAttribute()
        {
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            throw new NotImplementedException();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ApplicationRecordRegisterApiModel application = (ApplicationRecordRegisterApiModel)validationContext.ObjectInstance;
            var errorList = new List<string>();

            if (String.IsNullOrWhiteSpace(application.ApplicationUri)) { errorList.Add(nameof(application.ApplicationUri)); }
            if (String.IsNullOrWhiteSpace(application.ProductUri)) { errorList.Add(nameof(application.ProductUri)); }
            if (application.ApplicationType == null) { errorList.Add(nameof(application.ApplicationType)); }
            if (String.IsNullOrWhiteSpace(application.ApplicationName)) { errorList.Add(nameof(application.ApplicationName)); }
            if (application.ApplicationType != null && application.ApplicationType != ApplicationTypeClient)
            {
                if (application.DiscoveryUrls != null)
                {
                    for (int i = 0; i < application.DiscoveryUrls.Count; i++)
                    {
                        if (String.IsNullOrWhiteSpace(application.DiscoveryUrls[i])) { errorList.Add($"DiscoveryUrls[{i}]"); }
                    }
                }
                else
                {
                    errorList.Add($"DiscoveryUrls[0]");
                }
            }
            if (errorList.Count > 0) { return new ValidationResult("Required Field.", errorList); }

            /* entries will be ignored on register
            if (application.ApplicationType == ApplicationTypeClient)
            {
                if (!String.IsNullOrWhiteSpace(application.ServerCapabilities)) { errorList.Add(nameof(application.ServerCapabilities)); }
                for (int i = 0; i < application.DiscoveryUrls.Count; i++)
                {
                    if (!String.IsNullOrWhiteSpace(application.DiscoveryUrls[i])) { errorList.Add($"DiscoveryUrls[{i}]"); }
                }
                if (errorList.Count > 0) { return new ValidationResult("Invalid entry for client.", errorList); }
            }
            */

            if (!Uri.IsWellFormedUriString(application.ApplicationUri, UriKind.Absolute)) { errorList.Add("ApplicationUri"); }
            if (!Uri.IsWellFormedUriString(application.ProductUri, UriKind.Absolute)) { errorList.Add("ProductUri"); }
            if (application.ApplicationType != ApplicationTypeClient)
            {
                for (int i = 0; i < application.DiscoveryUrls.Count; i++)
                {
                    if (!Uri.IsWellFormedUriString(application.DiscoveryUrls[i], UriKind.Absolute)) { errorList.Add($"DiscoveryUrls[{i}]"); }
                }
            }
            if (errorList.Count > 0) { return new ValidationResult("Not a well formed Uri.", errorList); }

            if (application.ApplicationType != null &&
                application.ApplicationType != ApplicationTypeClient &&
                !String.IsNullOrEmpty(application.ServerCapabilities))
            {
                string[] serverCapModelArray = application.ServerCapabilities.Split(',');
                foreach (var cap in serverCapModelArray)
                {
                    ServerCapability serverCap = _serverCaps.Find(cap);
                    if (serverCap == null)
                    {
                        errorList.Add(nameof(application.ServerCapabilities));
                        return new ValidationResult(cap + " is not a valid ServerCapability.", errorList);
                    }
                }
            }

            return ValidationResult.Success;
        }
    }

    [ApplicationRecordRegisterApiModelAttribute]
    public class ApplicationRecordRegisterApiModel : ApplicationRecordApiModel
    {
        public ApplicationRecordRegisterApiModel() : base()
        { }

        public ApplicationRecordRegisterApiModel(ApplicationRecordApiModel apiModel) :
            base(apiModel.ApplicationId, apiModel.ID)
        {
            ApplicationUri = apiModel.ApplicationUri;
            ApplicationName = apiModel.ApplicationName;
            ApplicationType = apiModel.ApplicationType;
            ApplicationNames = apiModel.ApplicationNames;
            ProductUri = apiModel.ProductUri;
            DiscoveryUrls = apiModel.DiscoveryUrls;
            ServerCapabilities = apiModel.ServerCapabilities;
            GatewayServerUri = apiModel.GatewayServerUri;
            DiscoveryProfileUri = apiModel.DiscoveryProfileUri;
        }
    }

}


