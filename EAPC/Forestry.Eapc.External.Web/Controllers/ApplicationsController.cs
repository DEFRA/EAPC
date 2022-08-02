using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Infrastructure;
using Forestry.Eapc.External.Web.Services;
using Microsoft.AspNetCore.Authorization;

namespace Forestry.Eapc.External.Web.Controllers
{
    [Authorize]
    [RequireCompletedProfile]
    [Route("{controller}/{action=Index}")]
    public class ApplicationsController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(
            [FromServices] RetrieveApplicationsForUserUseCase useCase,
            CancellationToken cancellationToken)
        {
            var user = new ExternalUser(User);
            var applications = await useCase.RetrieveForUserAsync(user, cancellationToken);
            return View(applications);
        }

        [HttpGet]
        public async Task<IActionResult> Create(
            [FromServices] CreateNewApplicationUseCase useCase,
            CancellationToken cancellationToken = default)
        {
            var user = new ExternalUser(User);
            var result = await useCase.CreateAsync(user, cancellationToken);

            if (result.IsFailure)
            {
                this.AddConfirmationMessage(result.Error);
                return RedirectToAction("Index");
            }

            var applicationIdentifier = result.Value.Identifier!;
            
            return RedirectToAction(
                nameof(ApplicationController.Applicant), 
                "Application",
                new {identifier = applicationIdentifier});
        }

        [HttpGet]
        public IActionResult Completed()
        {
            var applicationReferenceId = TempData["ApplicationReferenceId"];
            ViewBag.ReferenceNumber = applicationReferenceId;
            return View();
        }
    }
}
