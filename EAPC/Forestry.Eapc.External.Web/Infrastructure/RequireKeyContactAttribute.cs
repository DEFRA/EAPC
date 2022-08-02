using Forestry.Eapc.External.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Forestry.Eapc.External.Web.Infrastructure
{
    public class RequireKeyContactAttribute : ActionFilterAttribute
    {
        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            
            var externalUser = new ExternalUser(context.HttpContext.User);

            if (!externalUser.IsProfessionalOperatorKeyContact)
            {
                context.Result = new RedirectToActionResult("Index", "Home", new object());
            }
        }
    }
}
