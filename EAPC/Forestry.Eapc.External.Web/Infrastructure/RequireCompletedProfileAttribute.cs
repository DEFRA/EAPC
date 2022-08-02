using System.Linq;
using Forestry.Eapc.External.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Forestry.Eapc.External.Web.Infrastructure
{
    /// <summary>
    /// Mandates that the user account and profile is in a specific state before they are allowed to complete
    /// actions pertinent to a phytosanitary certificate application.
    /// </summary>
    public class RequireCompletedProfileAttribute : ActionFilterAttribute
    {
        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var externalUser = new ExternalUser(context.HttpContext.User);
            
            if (RequiresMoreProfileData(externalUser))
            {
                // profile information incomplete to allow us to continue, so we redirect the user to ask them politely to fill in the required ata
                context.Result = new RedirectToActionResult("Index", "Profile", new object());
                return;
            }

            if (AccountAwaitingApprovalFromProfessionalOperatorKeyContact(externalUser))
            {
                // profile information incomplete to allow us to continue, so we redirect the user to ask them politely to fill in the required ata
                context.Result = new RedirectToActionResult("AwaitingApproval", "Profile", new object());
            }
        }

        private static bool RequiresMoreProfileData(ExternalUser user)
        {
            if (AnyNullOrWhitespace(user.ProfessionalOperatorNumber, user.GivenName, user.Surname, user.CompanyName, user.Telephone))
            {
                return true;
            }

            return string.IsNullOrWhiteSpace(user.CreditAccountReference) && !user.SignedUpToCreditTermsAndConditions;
        }

        private static bool AccountAwaitingApprovalFromProfessionalOperatorKeyContact(ExternalUser user)
        {
            return !user.IsApprovedAccount;
        }

        private static bool AnyNullOrWhitespace(params string?[] values)
        {
            return values.Any(string.IsNullOrWhiteSpace);
        }
    }
}
