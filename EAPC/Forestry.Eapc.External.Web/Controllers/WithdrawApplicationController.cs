using System;
using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Infrastructure;
using Forestry.Eapc.External.Web.Models.Application;
using Forestry.Eapc.External.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forestry.Eapc.External.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    [Authorize]
    [Route("{controller}/{identifier}/{action}")]
    public class WithdrawApplicationController : Controller
    {
        private readonly WithdrawApplicationUseCase _withdrawApplication;

        public WithdrawApplicationController(WithdrawApplicationUseCase withdrawApplication)
        {
            _withdrawApplication = withdrawApplication ?? throw new ArgumentNullException(nameof(withdrawApplication));
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string identifier,
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);
            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> Withraw(
            string identifier,
            CancellationToken cancellationToken)
        {
            var application = await FindApplicationAsync(identifier, cancellationToken);

            // should not be trying to withdraw an application in other states
            if (application.State is ApplicationState.Draft or ApplicationState.Submitted)
            {
                await _withdrawApplication.SetApplicationWithdrawnAsync(application, new ExternalUser(User), cancellationToken);
                var confirmationMessage = $"Application {application.ReferenceIdentifier} was successfully withdrawn.";
                this.AddConfirmationMessage(confirmationMessage);
            }

            return RedirectToAction("Index", "Applications");
        }

        private async Task<Application> FindApplicationAsync(string identifier, CancellationToken cancellationToken)
        {
            var user = new ExternalUser(User);
            var result = await _withdrawApplication.GetByIdAsync(identifier, user, cancellationToken);
            return result;
        }
    }
}