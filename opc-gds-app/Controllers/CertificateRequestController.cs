using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.CosmosDB;
using Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Models;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.Common.Controllers
{
    [Authorize]
    public class CertificateRequestController : Controller
    {
        private readonly IDocumentDBCollection<CertificateRequest> db;
        public CertificateRequestController(IDocumentDBCollection<CertificateRequest> db)
        {
            this.db = db;
        }

        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            var requests = await db.GetAsync(x => true);
            return View(requests);
        }

#pragma warning disable 1998
        [ActionName("Create")]
        public async Task<ActionResult> CreateAsync()
        {
            return View();
        }
#pragma warning restore 1998

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("RequestId,ApplicationId,State")] CertificateRequest request)
        {
            if (ModelState.IsValid)
            {
                await db.CreateAsync(request);
                return RedirectToAction("Index");
            }

            return View(request);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("RequestId,ApplicationId,State")] CertificateRequest request)
        {
            if (ModelState.IsValid)
            {
                await db.UpdateAsync(request.RequestId, request);
                return RedirectToAction("Index");
            }

            return View(request);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(Guid id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }

            CertificateRequest request = await db.GetAsync(id);
            if (request == null)
            {
                return new NotFoundResult();
            }

            return View(request);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }

            CertificateRequest request = await db.GetAsync(id);
            if (request == null)
            {
                return new NotFoundResult();
            }

            return View(request);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("RequestId")] Guid id)
        {
            await db.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(Guid id)
        {
            CertificateRequest request = await db.GetAsync(id);
            return View(request);
        }
    }
}