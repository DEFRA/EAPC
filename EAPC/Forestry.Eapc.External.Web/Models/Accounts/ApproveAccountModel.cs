using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forestry.Eapc.External.Web.Models.Accounts
{
    public class ApproveAccountModel
    {
        /// <summary>
        /// Gets and Sets the email address of the account to be approved.
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        [MaxLength(DataValueConstants.EmailMaxLength)]
        [DisplayName("Email Address of the user account to approve")]
        public string Email { get; set; }

        /// <summary>
        /// Gets and Sets the professional operator number.
        /// </summary>
        [Required]
        [RegularExpression(DataValueConstants.ProfessionalOperatorNumberRegex, ErrorMessage = "Your six-digit professional operator number must be provided.")]
        [DisplayName("Professional Operator Number")]
        public string ProfessionalOperatorNumber { get; set; }
    }
}
