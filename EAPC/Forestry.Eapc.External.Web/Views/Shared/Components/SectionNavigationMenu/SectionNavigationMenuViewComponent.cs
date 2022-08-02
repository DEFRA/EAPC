using System;
using Forestry.Eapc.External.Web.Models.Application;
using Microsoft.AspNetCore.Mvc;

namespace Forestry.Eapc.External.Web.Views.Shared.Components.SectionNavigationMenu
{
    [ViewComponent(Name = "SectionNavigationMenu")]
    public class SectionNavigationMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ApplicationState applicationState)
        {
            var sectionsFailingValidation = GetErrorSections();
            var currentActionAsString = GetCurrentAction();

            var model = new SectionNavigationMenuViewComponentModel(currentActionAsString, sectionsFailingValidation, applicationState);

            return View("Default", model);
        }

        private string GetCurrentAction()
        {
            if (RouteData.Values.TryGetValue("action", out var currentActionValue))
            {
                if (currentActionValue is string currentActionAsString) return currentActionAsString;
            }

            return string.Empty;
        }

        private string[] GetErrorSections()
        {
            //the ViewData value is set in the Application Controller.
            var sectionsFailingValidationObject = ViewData[EapcConstants.ErroringSectionsViewDataKey];

            if (sectionsFailingValidationObject is string[] value) return value;

            return Array.Empty<string>();
        }
    }
}
