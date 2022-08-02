using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forestry.Eapc.External.Web.Models.Application
{
    public class Section3
    {
        [DisplayName("Name of Consignee")]
        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? NameOfConsignee { get; set; }

        public Address AddressOfConsignee { get; set; } = new();

        [DisplayName("Port of Import")]
        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? PortOfImport { get; set; }

        [DisplayName("Port of Export (in the UK)")]
        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? PortOfExport { get; set; }

        [DisplayName("Date of Export")]
        [DataType(DataType.Date)]
        public DateTime? DateOfExport { get; set; }

        [DisplayName("Country of Destination")]
        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? CountryOfDestination { get; set; }
    }
}