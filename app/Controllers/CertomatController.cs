// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IIoT.OpcUa.Api.Vault;
using Microsoft.Azure.IIoT.OpcUa.Api.Vault.Models;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.App.Models;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.App.TokenStorage;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.App.Utils;
using Microsoft.Rest;

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
        public async Task<IActionResult> RegisterAsync(string id)
        {
            var apiModel = new ApplicationRecordApiModel();
            AuthorizeClient();
            if (id != null)
            {
                try
                {
                    apiModel = await opcVault.GetApplicationAsync(id);
                    ViewData["SuccessMessage"] =
                        "Application with id " + id + " successfully loaded.";
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] =
                        "An application with id " + id + " could not be found in the database.\r\n" +
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
                return RedirectToAction("Request", new { id = apiModel.ApplicationId });
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
                        "The application registration failed.\r\n" +
                        "Message: " + ex.Message;
                    return View(apiModel);
                }
                return RedirectToAction("Request", new { id = apiModel.ApplicationId });
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
                        return RedirectToAction("Register", new { id = applications[0].ApplicationId });
                    }
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] =
                        "Failed to find the application with ApplicationUri" + apiModel.ApplicationUri + "\r\n" +
                        "Message:" + ex.Message;
                    return View(apiModel);
                }
            }

            if (!String.IsNullOrWhiteSpace(apiModel.DiscoveryUrls.Last()) &&
                command == "add")
            {
                apiModel.DiscoveryUrls.Add("");
            }

            return View(apiModel);
        }

        [ActionName("Request")]
        public async Task<IActionResult> RequestAsync(string id)
        {
            AuthorizeClient();
            try
            {
                var application = await opcVault.GetApplicationAsync(id);
                UpdateApiModel(application);
                return View(application);
            }
            catch (Exception ex)
            {
                var application = new ApplicationRecordApiModel();
                ViewData["ErrorMessage"] =
                    "Failed to find the application with ApplicationId " + id + "\r\n" +
                    "Message:" + ex.Message;
                return View(application);
            }
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

            // preset Domain names with discovery Urls
            var domainNames = new List<string>();
            if (application.DiscoveryUrls != null)
            {
                foreach (var discoveryUrl in application.DiscoveryUrls)
                {
                    Uri url = Opc.Ua.Utils.ParseUri(discoveryUrl);
                    if (url == null)
                    {
                        continue;
                    }

                    string domainName = url.DnsSafeHost;
                    if (url.HostNameType != UriHostNameType.Dns)
                    {
                        domainName = Opc.Ua.Utils.NormalizedIPAddress(domainName);
                    }

                    if (!Opc.Ua.Utils.FindStringIgnoreCase(domainNames, domainName))
                    {
                        domainNames.Add(domainName);
                    }
                }
            }

            ViewData["Application"] = application;
            ViewData["Groups"] = groups;

            var request = new StartNewKeyPairRequestFormApiModel()
            {
                ApplicationId = id,
                CertificateGroupId = defaultGroupId,
                CertificateTypeId = defaultTypeId,
                DomainNames = domainNames
            };
            UpdateApiModel(request);
            return View(request);
        }

        [HttpPost]
        [ActionName("StartNewKeyPair")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StartNewKeyPairAsync(
            StartNewKeyPairRequestFormApiModel request,
            string add,
            string del)
        {
            AuthorizeClient();
            UpdateApiModel(request);
            if (ModelState.IsValid &&
                String.IsNullOrEmpty(add) &&
                String.IsNullOrEmpty(del))
            {
                string id;
                try
                {
                    id = await opcVault.StartNewKeyPairRequestAsync(request);
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] =
                        "Failed to create Certificate Request.\r\n" +
                        "Message:" + ex.Message;
                    goto LoadAppAndView;
                }
                string message = null;
                try
                {
                    await opcVault.ApproveCertificateRequestAsync(id, false);
                }
                catch (Exception ex)
                {
                    message =
                    "Failed to approve Certificate Request.\r\n" +
                    "Please contact Administrator for approval." +
                    ex.Message;
                }
                return RedirectToAction("Details", new { id, message });
            }

            if (!String.IsNullOrWhiteSpace(request.DomainNames.Last()) &&
                !String.IsNullOrEmpty(add))
            {
                request.DomainNames.Add("");
            }

            LoadAppAndView:
            // reload app info
            var application = await opcVault.GetApplicationAsync(request.ApplicationId);
            if (application == null)
            {
                return new NotFoundResult();
            }

            ViewData["Application"] = application;

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

            var request = new StartSigningRequestUploadApiModel()
            {
                ApiModel = new StartSigningRequestApiModel()
                {
                    ApplicationId = id,
                    CertificateGroupId = defaultGroupId,
                    CertificateTypeId = defaultTypeId
                },
                ApplicationUri = application.ApplicationUri,
                ApplicationName = application.ApplicationName
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
            ViewData["Message"] = message;

            var application = await opcVault.GetApplicationAsync(request.ApplicationId);
            if (application == null)
            {
                return new NotFoundResult();
            }

            ViewData["Application"] = application;

            return View(request);
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
                return new FileContentResult(byteArray, ContentType.Cert)
                {
                    FileDownloadName = Utils.Utils.CertFileName(result.SignedCertificate) + ".der"
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
                    FileDownloadName = Utils.Utils.CertFileName(issuer.Chain[0].Certificate) + ".der"
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
                    FileDownloadName = Utils.Utils.CertFileName(issuer.Chain[0].Certificate) + ".crl"
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
                        FileDownloadName = Utils.Utils.CertFileName(result.SignedCertificate) + ".pfx"
                    };
                }
                else if (String.Compare(result.PrivateKeyFormat, "PEM", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var byteArray = Convert.FromBase64String(result.PrivateKey);
                    return new FileContentResult(byteArray, ContentType.Pem)
                    {
                        FileDownloadName = Utils.Utils.CertFileName(result.SignedCertificate) + ".pem"
                    };
                }
            }
            return new NotFoundResult();
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

        private void UpdateApiModel(StartNewKeyPairRequestFormApiModel request)
        {
            if (request.DomainNames != null)
            {
                request.DomainNames = request.DomainNames.Where(x => !string.IsNullOrEmpty(x)).ToList();
            }
            else
            {
                request.DomainNames = new List<string>();
            }
            if (request.DomainNames.Count == 0)
            {
                request.DomainNames.Add("");
            }
        }
    }
}


