using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api.Models;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Controllers
{
    [Authorize]
    public class CertificateRequestController : Controller
    {
        // see RFC 2585
        const string ContentTypeCert = "application/pkix-cert";
        const string ContentTypeCrl = "application/pkix-crl";
        // see CertificateContentType.Pfx
        const string ContentTypePfx = "application/x-pkcs12";
        // see CertificateContentType.Pem
        const string ContentTypePem = "application/x-pem-file";

        private readonly IOpcGdsVault gdsVault;
        public CertificateRequestController(IOpcGdsVault gdsVault)
        {
            this.gdsVault = gdsVault;
        }

        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            var requests = await gdsVault.QueryRequestsAsync();
            return View(requests.Requests);
        }

        [ActionName("CreateNewKeyPair")]
        public async Task<ActionResult> CreateNewKeyPairAsync(string id)
        {
            var groups = await gdsVault.GetCertificateGroupConfigurationCollectionAsync();
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

            var application = await gdsVault.GetApplicationAsync(id);
            if (application == null)
            {
                return new NotFoundResult();
            }

            ViewData["Application"] = application;
            ViewData["Groups"] = groups;

            var request = new CreateNewKeyPairRequestApiModel()
            {
                ApplicationId = id,
                CertificateGroupId = defaultGroupId,
                CertificateTypeId = defaultTypeId
            };

            return View(request);
        }

        [HttpPost]
        [ActionName("CreateNewKeyPair")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateNewKeyPairAsync(
            CreateNewKeyPairRequestApiModel request)
        {
            if (ModelState.IsValid)
            {
                var id = await gdsVault.CreateNewKeyPairRequestAsync(request);
                return RedirectToAction("Index");
            }

            return View(request);
        }

        [ActionName("CreateSigningRequest")]
        public async Task<ActionResult> CreateSigningRequestAsync(string id)
        {
            var groups = await gdsVault.GetCertificateGroupConfigurationCollectionAsync();
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

            var application = await gdsVault.GetApplicationAsync(id);
            if (application == null)
            {
                return new NotFoundResult();
            }

            ViewData["Application"] = application;

            var request = new CreateSigningRequestApiModel()
            {
                ApplicationId = id,
                CertificateGroupId = defaultGroupId,
                CertificateTypeId = defaultTypeId
            };

            return View(request);
        }

        [HttpPost]
        [ActionName("CreateSigningRequest")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSigningRequestAsync(
            CreateSigningRequestApiModel request)
        {
            if (ModelState.IsValid)
            {
                var id = await gdsVault.CreateSigningRequestAsync(request);
                return RedirectToAction("Index");
            }

            return View(request);
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            var request = await gdsVault.ReadCertificateRequestAsync(id);
            return View(request);
        }

        [ActionName("Approve")]
        public async Task<ActionResult> ApproveAsync(string id)
        {
            await gdsVault.ApproveCertificateRequestAsync(id, false);
            return RedirectToAction("Details", new { id });
        }

        [ActionName("Reject")]
        public async Task<ActionResult> RejectAsync(string id)
        {
            await gdsVault.ApproveCertificateRequestAsync(id, true);
            return RedirectToAction("Details", new { id });
        }

        [ActionName("Accept")]
        public async Task<ActionResult> AcceptAsync(string id)
        {
            await gdsVault.AcceptCertificateRequestAsync(id);
            return RedirectToAction("Details", new { id });
        }

        [ActionName("DownloadCertificate")]
        public async Task<ActionResult> DownloadCertificateAsync(string requestId, string applicationId)
        {
            var result = await gdsVault.CompleteCertificateRequestAsync(requestId, applicationId);
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

        [ActionName("DownloadPrivateKey")]
        public async Task<ActionResult> DownloadPrivateKeyAsync(string requestId, string applicationId)
        {
            var result = await gdsVault.CompleteCertificateRequestAsync(requestId, applicationId);
            if (String.Compare(result.State, "Approved", StringComparison.OrdinalIgnoreCase) == 0 &&
                result.PrivateKey != null)
            {
                if (String.Compare(result.PrivateKeyFormat, "PFX", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var byteArray = Convert.FromBase64String(result.PrivateKey);
                    return new FileContentResult(byteArray, ContentTypePfx)
                    {
                        FileDownloadName = CertFileName(result.SignedCertificate) + ".pfx"
                    };
                }
                else if (String.Compare(result.PrivateKeyFormat, "PEM", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var byteArray = Convert.FromBase64String(result.PrivateKey);
                    return new FileContentResult(byteArray, ContentTypePem)
                    {
                        FileDownloadName = CertFileName(result.SignedCertificate) + ".pem"
                    };
                }
            }
            return new NotFoundResult();
        }

        [HttpPost]
        public ActionResult UploadFile(object o)
        {
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

    }
}
