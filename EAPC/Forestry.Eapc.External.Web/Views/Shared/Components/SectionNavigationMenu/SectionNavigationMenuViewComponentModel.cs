using System.Collections.Generic;
using System.Linq;
using Forestry.Eapc.External.Web.Models.Application;
using Forestry.Eapc.External.Web.Services;

namespace Forestry.Eapc.External.Web.Views.Shared.Components.SectionNavigationMenu
{
    public class SectionNavigationMenuViewComponentModel
    {
        public List<SectionNavigationLinkModel> SectionNavigationLinksModel;

        public SectionNavigationMenuViewComponentModel(
            string currentSectionAction, 
            string[] sectionsFailingValidation, 
            ApplicationState applicationState)
        {
            SectionNavigationLinksModel = BuildCurrentModel(currentSectionAction, sectionsFailingValidation, applicationState);
        }
        
        private static List<SectionNavigationLinkModel> BuildCurrentModel(
            string currentSectionAction, 
            string[] sectionsFailingValidation, 
            ApplicationState applicationState)
        {
            var model = ApplicationFormSectionsMetaModel.Model;

            SetCurrentSection(model, currentSectionAction);
            ApplySectionErrors(model, sectionsFailingValidation);

            return model;
        }

        private static void ApplySectionErrors(List<SectionNavigationLinkModel> model, string[] sectionsFailingValidation )
        {
            if (!sectionsFailingValidation.Any()) return;

            foreach (var errorSection in sectionsFailingValidation)
            {
                model.Single(x => x.SectionAction == errorSection).HasErrors = true;
            }
        }

        private static void SetCurrentSection(List<SectionNavigationLinkModel> model, string currentSectionAction)
        {
            foreach (var sectionNavigationLinkModel in model)
            {
                sectionNavigationLinkModel.IsCurrent = sectionNavigationLinkModel.SectionAction == currentSectionAction;

                if (sectionNavigationLinkModel.IsCurrent) break;
            }
        }
    }
}
