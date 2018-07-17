using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api;
using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api.Models;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Common.Controllers
{
    [Authorize]
    public class CertificateRequestController : Controller
    {
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

#pragma warning disable 1998
        [ActionName("CreateNewKeyPair")]
        public async Task<ActionResult> CreateNewKeyPairAsync()
        {
            return View();
        }
#pragma warning restore 1998

        [HttpPost]
        [ActionName("CreateNewKeyPair")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateNewKeyPairAsync([Bind("RequestId,ApplicationId,State")] CreateNewKeyPairRequestApiModel request)
        {
            if (ModelState.IsValid)
            {
                //await db.CreateAsync(request);
                return RedirectToAction("Index");
            }

            return View(request);
        }

#pragma warning disable 1998
        [ActionName("CreateSigningRequest")]
        public async Task<ActionResult> CreateSigningRequestAsync()
        {
            return View();
        }
#pragma warning restore 1998

        [HttpPost]
        [ActionName("CreateSigningRequest")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSigningRequestAsync([Bind("RequestId,ApplicationId,State")] CreateSigningRequestApiModel request)
        {
            if (ModelState.IsValid)
            {
                //await db.CreateAsync(request);
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

    }
}