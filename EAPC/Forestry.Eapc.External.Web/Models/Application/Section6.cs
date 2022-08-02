using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forestry.Eapc.External.Web.Models.Application
{
    public class Section6
    {
        [DisplayName("Additional Declarations")]
        [StringLength(DataValueConstants.AdditionalDeclarationsMaxLength)]
        public string? AdditionalDeclarations { get; set; }

        public bool AdditionalDeclarationsNotRequired { get; set; }
    }
}