using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forestry.Eapc.External.Web.Models.Application
{
    public class Address
    {
        [DisplayName("Contact Name")]
        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? ContactName { get; set; }
        
        [DisplayName("Address Line 1")]
        [StringLength(DataValueConstants.AddressLineMaxLength)]
        public string? Line1 { get; set; }
        
        [DisplayName("Address Line 2")]
        [StringLength(DataValueConstants.AddressLineMaxLength)]
        public string? Line2 { get; set; }
        
        [DisplayName("Address Line 3")]
        [StringLength(DataValueConstants.AddressLineMaxLength)]
        public string? Line3 { get; set; }
        
        [DisplayName("Address Line 4")]
        [StringLength(DataValueConstants.AddressLineMaxLength)]
        public string? Line4 { get; set; }
        
        [DisplayName("Address Line 5")]
        [StringLength(DataValueConstants.AddressLineMaxLength)]
        public string? Line5 { get; set; }
        
        [DisplayName("Postal Code")]
        [DataType(DataType.PostalCode)]
        [StringLength(DataValueConstants.PostalCodeMaxLength)]
        public string? PostalCode { get; set; }
    }
}