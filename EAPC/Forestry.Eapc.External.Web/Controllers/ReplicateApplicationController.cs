using System;
using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Infrastructure;
using Forestry.Eapc.External.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forestry.Eapc.External.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    [Authorize]
    [Route("{controller}/{identifier}/{action}")]
    public class ReplicateApplicationController : Controller
    {
        private readonly ReplicateExistingApplicationUseCase _useCase;

        public ReplicateApplicationController(ReplicateExistingApplicationUseCase useCase)
        {
            _useCase = useCase ?? throw new ArgumentNullException(nameof(useCase));
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string identifier,
            CancellationToken cancellationToken)
        {
            var application = await _useCase.GetByIdAsync(identifier, new ExternalUser(User), cancellationToken);
            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> Replicate(
            string identifier,
            CancellationToken cancellationToken)
        {
            var newApplication = await _useCase.ReplicateAsync(identifier, new ExternalUser(User), cancellationToken);

            if (newApplication.IsSuccess)
            {
                var confirmationMessage = $"A copy of the application has been created, the new application reference id is {newApplication.Value.ReferenceIdentifier}.";
                this.AddConfirmationMessage(confirmationMessage);
                return RedirectToAction("Applicant", "Application", new { identifier = newApplication.Value.Identifier });
            }
            
            this.AddConfirmationMessage("A copy of the application could not be created at this time, please try again.");
            return RedirectToAction("Index", "Applications");
        }
    }
}