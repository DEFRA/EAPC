using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Infrastructure;
using Forestry.Eapc.External.Web.Models.Accounts;
using Forestry.Eapc.External.Web.Services;
using Microsoft.AspNetCore.Authorization;

namespace Forestry.Eapc.External.Web.Controllers
{
    [Authorize]
    [RequireCompletedProfile]
    //[RequireKeyContact]
    [AutoValidateAntiforgeryToken]
    public class AccountsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ApproveAccountModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(
            ApproveAccountModel model, 
            [FromServices] ApproveAccountUseCase useCase,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await useCase.ExecuteAsync(model, new ExternalUser(User), cancellationToken);

            switch (result)
            {
                case ApproveAccountOutcome.ProfessionalOperatorNumberMismatch:
                    ModelState.AddModelError(nameof(ApproveAccountModel.ProfessionalOperatorNumber),"The entered professional operator number is invalid");
                    return View(model);
                case ApproveAccountOutcome.LocalAccountNotFound:
                    ModelState.AddModelError(string.Empty, "No account was found for approval matching the values provided.");
                    return View(model);
                case ApproveAccountOutcome.Success:
                    return RedirectToAction(nameof(Success));
                default:
                    throw new ArgumentOutOfRangeException("Unepected ApproveAccountOutcome value " + result);
            }
        }

        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }
    }
}
