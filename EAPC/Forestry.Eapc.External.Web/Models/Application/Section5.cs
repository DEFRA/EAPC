using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forestry.Eapc.External.Web.Models.Application
{
    public class Section5
    {
        [DisplayName("Treatment")]
        public TreatmentType? Treatment { get; set; }

        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? TreatmentOtherText { get; set; }

        [DisplayName("Concentration")]
        [StringLength(DataValueConstants.ConcentrationMaxLength)]
        public string? Concentration { get; set; }

        [DisplayName("Chemical (active ingredient)")]
        public TreatmentChemical? Chemical { get; set; }

        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? ChemicalOtherText { get; set; }

        [DisplayName("Date Treatment Completed")]
        [DataType(DataType.Date)]
        public DateTime? DateOfTreatment { get; set; }

        [DisplayName("Duration")]
        [Range(0, 2000)]
        public int? Duration { get; set; }

        [DisplayName("Temperature")]
        [Range(0, 300)]
        public int? Temperature { get; set; }

        [DisplayName("Additional Information")]
        [StringLength(DataValueConstants.DefaultMultiLineTextMaxLength)]
        public string? AdditionalInformation { get; set; }
    }
}