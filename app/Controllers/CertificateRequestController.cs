// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IIoT.OpcUa.Api.Vault;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.App.TokenStorage;
using Microsoft.Azure.IIoT.OpcUa.Services.Vault.App.Utils;
using Microsoft.Rest;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Vault.App.Controllers
{
    [Authorize]
    public class CertificateRequestController : Controller
    {
        private IOpcVault opcVault;
        private readonly OpcVaultApiOptions opcVaultOptions;
        private readonly AzureADOptions azureADOptions;
        private readonly ITokenCacheService tokenCacheService;

        public CertificateRequestController(
            OpcVaultApiOptions opcVaultOptions,
            AzureADOptions azureADOptions,
            ITokenCacheService tokenCacheService)
        {
            this.opcVaultOptions = opcVaultOptions;
            this.azureADOptions = azureADOptions;
            this.tokenCacheService = tokenCacheService;
        }

        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            AuthorizeClient();
            var requests = await opcVault.QueryRequestsAsync();
            return View(requests.Requests);
        }

#if DISABLEDPAGES        
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
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }

            return View(request);
        }
#endif
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

    }
}
