namespace Forestry.Eapc.External.Web.Views.Shared.Components.NextPreviousNavigation
{
    public class NextPreviousNavigationViewComponentModel
    {
        public bool ShowPrevious { get; set; }
        public bool ShowSave { get; set; }
        public bool ShowNext { get; set; }
        public bool ShowGoToSummary { get; set; }
        public string NextButtonDisplayText{ get; set; } = "Save and Continue";
    }
}
