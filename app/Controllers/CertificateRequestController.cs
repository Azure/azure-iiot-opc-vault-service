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

        [ActionName("CreateNewKeyPair")]
        public Task<ActionResult> CreateNewKeyPairAsync()
        {
            return Task.FromResult<ActionResult>(View());
        }

        [HttpPost]
        [ActionName("CreateNewKeyPair")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateNewKeyPairAsync([Bind("RequestId,ApplicationId,State")] CreateNewKeyPairRequestApiModel request)
        {
            if (ModelState.IsValid)
            {
                //await db.CreateAsync(request);
                await Task.Delay(100);
                return RedirectToAction("Index");
            }

            return View(request);
        }

        [ActionName("CreateSigningRequest")]
        public Task<ActionResult> CreateSigningRequestAsync()
        {
            return Task.FromResult<ActionResult>(View());
        }

        [HttpPost]
        [ActionName("CreateSigningRequest")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSigningRequestAsync([Bind("RequestId,ApplicationId,State")] CreateSigningRequestApiModel request)
        {
            if (ModelState.IsValid)
            {
                //await db.CreateAsync(request);
                await Task.Delay(100);
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