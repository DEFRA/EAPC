using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forestry.Eapc.External.Web.Models.Application
{
    public class Section7
    {
        [DisplayName("Request PDF Copy")]
        public bool PdfCopyRequested { get; set; }
        
        public Address CertificateDeliveryAddress { get; set; } = new();

        [DisplayName("Your Order Reference")]
        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? CustomerPurchaseOrderNumber { get; set; }

        [DisplayName("Credit Customer Number")]
        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? CustomerCreditNumber { get; set; }
    }
}