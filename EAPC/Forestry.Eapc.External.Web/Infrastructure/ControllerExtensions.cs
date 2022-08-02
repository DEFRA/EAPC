using Microsoft.AspNetCore.Mvc;

namespace Forestry.Eapc.External.Web.Infrastructure
{
    public static class ControllerExtensions
    {
        public const string ConfirmationMessageKey = "ConfirmationMessage";
        private const string AllSectionValidationErrorsResolved = "AllSectionValidationErrorsResolved";

        public static void AddConfirmationMessage(this Controller controller, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            controller.TempData.Add(ConfirmationMessageKey, value);
        }

        public static bool AddTempDataFlagIfSectionErrorsNowResolved(this Controller controller, bool showErrors)
        {
            //If was showing errors but now model state is valid, then user has resolved:
            if (controller.ModelState.IsValid && showErrors)
            {
                controller.TempData[AllSectionValidationErrorsResolved] = true;
                return false;
            }

            return showErrors;// true;
        }

        public static void AddViewBagFlagIfSectionErrorsResolved(this Controller controller)
        {
            if (controller.TempData.ContainsKey(AllSectionValidationErrorsResolved))
            {
                controller.ViewBag.AllSectionValidationErrorsResolved = controller.TempData[AllSectionValidationErrorsResolved];
            }
        }
    }
}
