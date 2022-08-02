using System.Collections.Generic;
using System.Linq;
using Forestry.Eapc.External.Web.Controllers;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services
{
    public static class ApplicationFormSectionsMetaModel
    {
        public static List<SectionNavigationLinkModel> Model => BuildModel();

        public static SectionNavigationLinkModel GetSectionItem(string section) => GetByAction(section);

        private static List<SectionNavigationLinkModel> BuildModel()
        {
            return new List<SectionNavigationLinkModel>
            {
                new(1, nameof(Application.Applicant), "Your Information"),
                new(2, nameof(Application.Section1), "Exporter Details"),
                new(3, nameof(Application.Section2), "Goods Inspection Site"),
                new(4, nameof(Application.Section3), "Consignment Destination"),
                new(5, nameof(Application.Section4), "Details of Consignment"),
                new(6, nameof(Application.Section5), "Special Conditions"),
                new(7, nameof(Application.Section6), "Additional Declarations"),
                new(8, nameof(Application.Section7), "Confirmation of Order"),
                new(9, nameof(ApplicationController.SupportingDocumentsSection), "Supporting Information"),
                new(10, nameof(ApplicationController.CertificatePreview), "Certificate Preview", displaySectionState:false),
                new(11, nameof(ApplicationController.Summary), "Check your Answers", displaySectionState:false),
                new(12, nameof(ApplicationController.Confirmation), "Confirmation", displaySectionState:false)
            };
        }

        private static SectionNavigationLinkModel GetByAction(string section)
        {
            return Model.Single(x => x.SectionAction == section);
        }
    }
}