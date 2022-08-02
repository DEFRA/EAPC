using System;
using Microsoft.AspNetCore.Mvc;

namespace Forestry.Eapc.External.Web.Views.Shared.Components.GdsWarning
{
    /// <summary>
    /// https://design-system.service.gov.uk/components/warning-text/
    /// </summary>
    [ViewComponent(Name = "GdsWarning")]
    public class GdsWarningViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string warningMessageHtml, string warningMessageAssistiveHtml="Warning")
        {
            if (string.IsNullOrEmpty(warningMessageHtml)) throw new ArgumentNullException(nameof(warningMessageHtml));

            return View("Default", new GdsWarningViewComponentModel
            {
                WarningMessageHtml = warningMessageHtml, 
                WarningMessageAssistiveHtml = warningMessageAssistiveHtml
            });
        }
    }
}