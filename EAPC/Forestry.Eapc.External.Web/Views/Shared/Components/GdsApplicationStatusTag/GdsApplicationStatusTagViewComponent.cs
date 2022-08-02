using System;
using Forestry.Eapc.External.Web.Models.Application;
using Microsoft.AspNetCore.Mvc;

namespace Forestry.Eapc.External.Web.Views.Shared.Components.GdsApplicationStatusTag
{
    /// <summary>
    /// https://design-system.service.gov.uk/components/tag/
    /// </summary>
    [ViewComponent(Name = "GdsApplicationStatusTag")]
    public class GdsApplicationStatusTagViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ApplicationState applicationState)
        {
            return View("Default", new GdsApplicationStatusTagViewComponentModel{ApplicationState = applicationState});
        }
    }
}