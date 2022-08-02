using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forestry.Eapc.External.Web.Models.Application
{
    public class Applicant
    {
        [DisplayName("Name of Applicant")]
        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? PersonName { get; set; }
        
        [DisplayName("Applicant Company Name")]
        [StringLength(DataValueConstants.CompanyNameMaxLength)]
        public string? CompanyName { get; set; }

        [DisplayName("Region")]
        public HomeNation? Region { get; set; }

        [DisplayName("Professional Operator Number")]
        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? ProfessionalOperatorNumber { get; set; }

        [DisplayName("Contact Email")]
        [EmailAddress]
        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? Email { get; set; }
        
        [DisplayName("Contact Telephone")]
        [Phone]
        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? Telephone { get; set; }

        public ExportStatus? ExportStatus { get; set; } = Models.Application.ExportStatus.New;
    }
}