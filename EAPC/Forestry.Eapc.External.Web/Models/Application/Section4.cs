using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forestry.Eapc.External.Web.Models.Application
{
    public class Section4
    {
        [DisplayName("Description of Products / Packages")]
        [StringLength(DataValueConstants.DefaultMultiLineTextMaxLength)]
        public string? DescriptionOfProducts { get; set; }

        [DisplayName("Common / Botanical Name")]
        public string[] BotanicalNames { get; set; } = Array.Empty<string>();

        [DisplayName("Where Grown")] 
        public string[] WhereGrowns { get; set; } = Array.Empty<string>();

        [DisplayName("Serial number(s) of phytosanitary certificates issued in country of origin, if any.")]
        [StringLength(DataValueConstants.DefaultMultiLineTextMaxLength)]
        public string? CertificateNumbersFromCountryOfOrigin { get; set; }

        [DisplayName("Quantity")]
        public List<Quantity> Quantity { get; set; } = new();

        [DisplayName("Means of conveyance")]
        public TransportType? MeansOfConveyance { get; set; }
        
        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? MeansOfConveyanceOtherText { get; set; }

        [DisplayName("Country of Destination")]
        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? CountryOfDestination { get; set; }

        [DisplayName("Commodity Type")]
        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? CommodityType { get; set; }
    }
}