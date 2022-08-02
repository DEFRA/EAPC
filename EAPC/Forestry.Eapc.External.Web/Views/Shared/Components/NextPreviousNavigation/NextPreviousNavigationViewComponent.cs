using Microsoft.AspNetCore.Mvc;

namespace Forestry.Eapc.External.Web.Views.Shared.Components.NextPreviousNavigation
{
    [ViewComponent(Name = "NextPreviousNavigation")]
    public class NextPreviousNavigationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(bool showPrevious, bool showSave, bool showNext, bool showGoToSummary, string nextButtonDisplayText= "Save and continue")
        {
            return View("Default",
                new NextPreviousNavigationViewComponentModel
                {
                    ShowPrevious = showPrevious, 
                    ShowSave = showSave, 
                    ShowNext = showNext, 
                    ShowGoToSummary = showGoToSummary,
                    NextButtonDisplayText = nextButtonDisplayText
                });
        }
    }
}
