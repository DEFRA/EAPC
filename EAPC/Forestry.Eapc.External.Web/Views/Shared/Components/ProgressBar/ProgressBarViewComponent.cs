using Microsoft.AspNetCore.Mvc;

namespace Forestry.Eapc.External.Web.Views.Shared.Components.ProgressBar
{
    [ViewComponent(Name = "ProgressBar")]
    public class ProgressBarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int step)
        {
            return View("Default", new ProgressBarViewComponentModel {Step = step});
        }
    }
}
