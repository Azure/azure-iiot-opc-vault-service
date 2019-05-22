// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//

namespace Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.Controllers {
    using Microsoft.AspNetCore.Authentication.AzureAD.UI;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.IIoT.OpcUa.Api.Vault;
    using Microsoft.Azure.IIoT.OpcUa.Api.Vault.Models;
    using Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.Models;
    using Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.TokenStorage;
    using Microsoft.Azure.IIoT.WebApps.OpcUa.Vault.Utils;
    using Microsoft.Rest;
    using Serilog;
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    [Authorize]
    public class DownloadController : Controller {
        protected IVaultServiceApi _opcVault;
        private readonly OpcVaultApiOptions _opcVaultOptions;
        private readonly AzureADOptions _azureADOptions;
        private readonly ITokenCacheService _tokenCacheService;
        private readonly ILogger _log;

        public DownloadController(
            OpcVaultApiOptions opcVaultOptions,
            AzureADOptions azureADOptions,
            ITokenCacheService tokenCacheService,
            ILogger log) {
            _opcVaultOptions = opcVaultOptions;
            _azureADOptions = azureADOptions;
            _tokenCacheService = tokenCacheService;
            _log = log;
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id, string errorMessage, string successMessage) {
            AuthorizeClient();
            var request = await _opcVault.GetCertificateRequestAsync(id);
            ViewData["ErrorMessage"] = errorMessage;
            ViewData["SuccessMessage"] = successMessage;

            var application = await _opcVault.GetApplicationAsync(request.ApplicationId);
            if (application == null) {
                return new NotFoundResult();
            }

            ViewData["Application"] = application;

            return View(request);
        }

        [ActionName("Approve")]
        public async Task<ActionResult> ApproveAsync(string id) {
            AuthorizeClient();
            try {
                await _opcVault.ApproveCertificateRequestAsync(id, false);
                return RedirectToAction("Details", new { id, successMessage = "CertificateRequest approved!" });
            }
            catch (Exception ex) {
                _log.Error(ex, "Failed to approve certificate request.");
                var errorMessage =
                "Failed to approve certificate request." +
                "Message: " + ex.Message;
                return RedirectToAction("Details", new { id, errorMessage });
            }
        }

        [ActionName("Reject")]
        public async Task<ActionResult> RejectAsync(string id) {
            AuthorizeClient();
            try {
                await _opcVault.ApproveCertificateRequestAsync(id, true);
                return RedirectToAction("Details", new { id, successMessage = "CertificateRequest rejected!" });
            }
            catch (Exception ex) {
                _log.Error(ex, "Failed to reject certificate request.");
                var errorMessage =
                "Failed to reject certificate request." +
                "Message: " + ex.Message;
                return RedirectToAction("Details", new { id, errorMessage });
            }
        }

        [ActionName("Accept")]
        public async Task<ActionResult> AcceptAsync(string id) {
            AuthorizeClient();
            try {
                await _opcVault.AcceptCertificateRequestAsync(id);
                return RedirectToAction("Details", new { id, successMessage = "CertificateRequest accepted!" });
            }
            catch (Exception ex) {
                _log.Error(ex, "Failed to accept certificate request.");
                var errorMessage =
                "Failed to accept certificate request." +
                "Message: " + ex.Message;
                return RedirectToAction("Details", new { id, errorMessage });
            }
        }

        [ActionName("DownloadCertificate")]
        public async Task<ActionResult> DownloadCertificateAsync(string requestId, string applicationId) {
            AuthorizeClient();
            try {
                var result = await _opcVault.FetchCertificateRequestResultAsync(requestId, applicationId);
                if ((result.State == CertificateRequestState.Approved ||
                    result.State == CertificateRequestState.Accepted) &&
                    result.SignedCertificate != null) {
                    var byteArray = Convert.FromBase64String(result.SignedCertificate);
                    return new FileContentResult(byteArray, ContentEncodings.MimeTypeCert) {
                        FileDownloadName = Utils.CertFileName(result.SignedCertificate) + ".der"
                    };
                }
                return RedirectToAction("Details", new {
                    id = requestId,
                    errorMessage = "Certificate request " + requestId + " is in invalid state or has no certificate."
                });
            }
            catch (Exception ex) {
                var errorMessage =
                "Failed to download certificate." +
                "Message: " + ex.Message;
                return RedirectToAction("Details", new { id = requestId, errorMessage });
            }
        }

        [ActionName("DownloadCertificateBase64")]
        public async Task<ActionResult> DownloadCertificateBase64Async(string requestId, string applicationId) {
            AuthorizeClient();
            try {
                var result = await _opcVault.FetchCertificateRequestResultAsync(requestId, applicationId);
                if ((result.State == CertificateRequestState.Approved ||
                    result.State == CertificateRequestState.Accepted) &&
                    result.SignedCertificate != null) {
                    return RedirectToAction("DownloadCertBase64", new { cert = result.SignedCertificate });
                }
                return RedirectToAction("Details", new {
                    id = requestId,
                    errorMessage = "Certificate request not in the proper state or certificate is missing."
                });
            }
            catch (Exception ex) {
                var errorMessage =
                "Failed to download certificate." +
                "Message: " + ex.Message;
                return RedirectToAction("Details", new { id = requestId, errorMessage });
            }
        }

        [ActionName("DownloadIssuer")]
        public async Task<ActionResult> DownloadIssuerAsync(string requestId) {
            AuthorizeClient();
            try {
                var request = await _opcVault.GetCertificateRequestAsync(requestId);
                if (request != null) {
                    var issuer = await _opcVault.GetCertificateGroupIssuerCAChainAsync(request.CertificateGroupId);
                    var byteArray = Convert.FromBase64String(issuer.Chain[0].Certificate);
                    return new FileContentResult(byteArray, ContentEncodings.MimeTypeCert) {
                        FileDownloadName = Utils.CertFileName(issuer.Chain[0].Certificate) + ".der"
                    };
                }
                return RedirectToAction("Details", new {
                    id = requestId,
                    errorMessage = "Certificate request " + requestId + " not found."
                });
            }
            catch (Exception ex) {
                var errorMessage =
                "Failed to download Issuer CA certificate." +
                "Message: " + ex.Message;
                return RedirectToAction("Details", new { id = requestId, errorMessage });
            }
        }

        [ActionName("DownloadIssuerCrl")]
        public async Task<ActionResult> DownloadIssuerCrlAsync(string requestId) {
            AuthorizeClient();
            try {
                var request = await _opcVault.GetCertificateRequestAsync(requestId);
                if (request != null) {
                    var issuer = await _opcVault.GetCertificateGroupIssuerCAChainAsync(request.CertificateGroupId);
                    var crl = await _opcVault.GetCertificateGroupIssuerCACrlChainAsync(request.CertificateGroupId);
                    var byteArray = Convert.FromBase64String(crl.Chain[0].Crl);
                    return new FileContentResult(byteArray, ContentEncodings.MimeTypeCrl) {
                        FileDownloadName = Utils.CertFileName(issuer.Chain[0].Certificate) + ".crl"
                    };
                }
                return RedirectToAction("Details", new {
                    id = requestId,
                    errorMessage = "Certificate request " + requestId + " not found."
                });
            }
            catch (Exception ex) {
                var errorMessage =
                "Failed to download Issuer CRL." +
                "Message: " + ex.Message;
                return RedirectToAction("Details", new { id = requestId, errorMessage });
            }
        }

        [ActionName("DownloadIssuerBase64")]
        public async Task<ActionResult> DownloadIssuerBase64Async(string groupId, string requestId) {
            AuthorizeClient();
            try {
                if (groupId == null) {
                    var request = await _opcVault.GetCertificateRequestAsync(requestId);
                    if (request != null) {
                        groupId = request.CertificateGroupId;
                    }
                    else {
                        return RedirectToAction("Details", new {
                            id = requestId,
                            errorMessage = "Certificate request " + requestId + " not found."
                        });
                    }
                }
                if (groupId != null) {
                    var issuer = await _opcVault.GetCertificateGroupIssuerCAChainAsync(groupId);
                    return RedirectToAction("DownloadCertBase64", new { cert = issuer.Chain[0].Certificate });
                }
                return RedirectToAction("Details", new {
                    id = requestId,
                    errorMessage = "Certificate request " + requestId + " has no group id."
                });
            }
            catch (Exception ex) {
                var errorMessage =
                "Failed to load Issuer CA certificate." +
                "Message: " + ex.Message;
                return RedirectToAction("Details", new { id = requestId, errorMessage });
            }
        }

        [ActionName("DownloadIssuerCrlBase64")]
        public async Task<ActionResult> DownloadIssuerCrlBase64Async(string groupId, string requestId) {
            AuthorizeClient();
            try {
                if (groupId == null) {
                    var request = await _opcVault.GetCertificateRequestAsync(requestId);
                    if (request != null) {
                        groupId = request.CertificateGroupId;
                    }
                }

                if (groupId != null) {
                    var crl = await _opcVault.GetCertificateGroupIssuerCACrlChainAsync(groupId);
                    return RedirectToAction("DownloadCrlBase64", new { crl = crl.Chain[0].Crl });
                }
                return RedirectToAction("Details", new {
                    id = requestId,
                    errorMessage = "Certificate request " + requestId + " has no group id."
                });
            }
            catch (Exception ex) {
                var errorMessage =
                "Failed to load Issuer CRL." +
                "Message: " + ex.Message;
                return RedirectToAction("Details", new { id = requestId, errorMessage });
            }
        }

        [ActionName("DownloadPrivateKey")]
        public async Task<ActionResult> DownloadPrivateKeyAsync(string requestId, string applicationId) {
            AuthorizeClient();
            try {
                var result = await _opcVault.FetchCertificateRequestResultAsync(requestId, applicationId);
                if (result.State == CertificateRequestState.Approved &&
                    result.PrivateKey != null) {
                    if (string.Compare(result.PrivateKeyFormat, "PFX", StringComparison.OrdinalIgnoreCase) == 0) {
                        var byteArray = Convert.FromBase64String(result.PrivateKey);
                        return new FileContentResult(byteArray, ContentEncodings.MimeTypePfxCert) {
                            FileDownloadName = Utils.CertFileName(result.SignedCertificate) + ".pfx"
                        };
                    }
                    else if (string.Compare(result.PrivateKeyFormat, "PEM", StringComparison.OrdinalIgnoreCase) == 0) {
                        var byteArray = Convert.FromBase64String(result.PrivateKey);
                        return new FileContentResult(byteArray, ContentEncodings.MimeTypePemCert) {
                            FileDownloadName = Utils.CertFileName(result.SignedCertificate) + ".pem"
                        };
                    }
                }
                return RedirectToAction("Details", new {
                    id = requestId,
                    errorMessage = "Failed to download the private key."
                });
            }
            catch (Exception ex) {
                var errorMessage =
                "Failed to download the private key." +
                "Message: " + ex.Message;
                return RedirectToAction("Details", new { id = requestId, errorMessage });
            }
        }

        [ActionName("DownloadKeyBase64")]
        public async Task<ActionResult> DownloadPrivateKeyBase64Async(string requestId, string applicationId) {
            AuthorizeClient();
            try {
                var result = await _opcVault.FetchCertificateRequestResultAsync(requestId, applicationId);
                if (result.State == CertificateRequestState.Approved &&
                    result.PrivateKey != null) {
                    var model = new KeyDetailsApiModel();
                    if (string.Compare(result.PrivateKeyFormat, "PFX", StringComparison.OrdinalIgnoreCase) == 0) {
                        model.EncodedBase64 = result.PrivateKey;
                        return View(model);
                    }
                    else if (string.Compare(result.PrivateKeyFormat, "PEM", StringComparison.OrdinalIgnoreCase) == 0) {
                        // if (false)
                        // {
                        //     //to display PEM as text
                        //     var byteArray = Convert.FromBase64String(result.PrivateKey);
                        //     model.EncodedBase64 = System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                        // }
                        // else
                        {
                            model.EncodedBase64 = result.PrivateKey;
                        }
                        return View(model);
                    }
                }
                return RedirectToAction("Details", new {
                    id = requestId,
                    errorMessage = "Failed to download the private key."
                });
            }
            catch (Exception ex) {
                var errorMessage =
                "Failed to download the private key." +
                "Message: " + ex.Message;
                return RedirectToAction("Details", new { id = requestId, errorMessage });
            }
        }

        [ActionName("DownloadCertBase64")]
        public ActionResult DownloadCertBase64(string cert) {
            var byteArray = Convert.FromBase64String(cert);
            var certificate = new X509Certificate2(byteArray);
            var model = new CertificateDetailsApiModel {
                Subject = certificate.Subject,
                Issuer = certificate.Issuer,
                Thumbprint = certificate.Thumbprint,
                SerialNumber = certificate.SerialNumber,
                NotBefore = certificate.NotBefore,
                NotAfter = certificate.NotAfter,
                EncodedBase64 = cert
            };
            return View(model);
        }

        [ActionName("DownloadCrlBase64")]
        public ActionResult DownloadCrlBase64(string crl) {
            var byteArray = Convert.FromBase64String(crl);
            var crlObject = new Opc.Ua.X509CRL(byteArray);
            var model = new CrlDetailsApiModel {
                UpdateTime = crlObject.UpdateTime,
                NextUpdateTime = crlObject.NextUpdateTime,
                Issuer = crlObject.Issuer,
                EncodedBase64 = crl
            };
            return View(model);
        }

        protected void AuthorizeClient() {
            if (_opcVault == null) {
                ServiceClientCredentials serviceClientCredentials =
                    new OpcVaultLoginCredentials(_opcVaultOptions, _azureADOptions, _tokenCacheService, User);
                _opcVault = new OpcVault(new Uri(_opcVaultOptions.BaseAddress), serviceClientCredentials);
            }
        }

    }
}


