using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Azure.IoTSolutions.GdsVault.CosmosDB.Models;
using System;

namespace Microsoft.Azure.IoTSolutions.GdsVault.CosmosDB.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly IDocumentDBCollection<Application> db;
        public ApplicationController(IDocumentDBCollection<Application> db)
        {
            this.db = db;
        }

        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            var applications = await db.GetAsync(x => true);
            return View(applications);
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
        public async Task<ActionResult> CreateAsync([Bind("ApplicationId,ApplicationName,ApplicationUri")] Application application)
        {
            if (ModelState.IsValid)
            {
                await db.CreateAsync(application);
                return RedirectToAction("Index");
            }

            return View(application);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("ApplicationId,ApplicationName,ApplicationUri")] Application application)
        {
            if (ModelState.IsValid)
            {
                await db.UpdateAsync(application.ApplicationId, application);
                return RedirectToAction("Index");
            }

            return View(application);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(Guid id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }

            Application application = await db.GetAsync(id);
            if (application == null)
            {
                return new NotFoundResult();
            }

            return View(application);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }

            Application application = await db.GetAsync(id);
            if (application == null)
            {
                return new NotFoundResult();
            }

            return View(application);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("Id")] Guid id)
        {
            await db.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(Guid id)
        {
            Application application = await db.GetAsync(id);
            return View(application);
        }
    }
}