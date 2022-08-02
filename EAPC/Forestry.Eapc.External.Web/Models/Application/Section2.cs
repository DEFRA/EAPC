using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forestry.Eapc.External.Web.Models.Application
{
    public class Section2
    {
        public Address GoodsInspectionAddress { get; set; } = new();

        [DisplayName("Inspection Information")]
        [StringLength(DataValueConstants.DefaultMultiLineTextMaxLength)]
        public string? AdditionalInformation { get; set; }

        public bool InspectionNotRequired { get; set; }
    }
}