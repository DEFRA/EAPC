using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Models.Profile;
using Forestry.Eapc.External.Web.Services;
using Forestry.Eapc.External.Web.Services.Repositories.ProfessionalOperator;
using Microsoft.AspNetCore.Authorization;

namespace Forestry.Eapc.External.Web.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class ProfileController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var model = ModelMapping.ToUserProfileModel(new ExternalUser(User));
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(
            UserProfileModel model, 
            [FromServices] SignUpToUseSystemUseCase useCase,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var outcome = await useCase.ApplyProfileAsync(
                new ExternalUser(User),
                model,
                cancellationToken);

            switch (outcome)
            {
                case ApplyProfileOutcome.OperationFailed:
                    ModelState.AddModelError(string.Empty, "There was a problem saving your profile data, please retry.");
                    return View(model);
                case ApplyProfileOutcome.ProfessionalOperatorNotFound:
                    ModelState.AddModelError(nameof(UserProfileModel.ProfessionalOperatorNumber), "The professional operator number appears to be invalid.");
                    return View(model);
                case ApplyProfileOutcome.UserCanAccessApplications:
                    return RedirectToAction("Completed");
                case ApplyProfileOutcome.AccountRequiresApproval:
                    return RedirectToAction("AwaitingApproval");
                default:
                    throw new ArgumentOutOfRangeException($"Invalid use case outcome value of {outcome} was received.");
            }
        }

        [HttpGet]
        public IActionResult Completed()
        {
            return View();
        }

        /// <summary>
        /// This endpoint is invoked when the logged in user is not the key contact for their professional operator (as determined by the data
        /// read from the <see cref="IProfessionalOperatorRepository"/>) and the key contact has not yet approved the user's account.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AwaitingApproval()
        {
            return View();
        }
    }
}
