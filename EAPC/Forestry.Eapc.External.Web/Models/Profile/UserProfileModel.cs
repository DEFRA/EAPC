using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Forestry.Eapc.External.Web.Infrastructure;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Models.Profile
{
    public class UserProfileModel : IValidatableObject
    {
        /// <summary>
        /// Gets and Sets the user's first name.
        /// </summary>
        [Required]
        [MaxLength(DataValueConstants.NamePartMaxLength)]
        [DisplayName("Given Name")]
        public string? FirstName { get; set; }
        
        /// <summary>
        /// Gets and Sets the user's last name.
        /// </summary>
        [Required]
        [MaxLength(DataValueConstants.NamePartMaxLength)]
        [DisplayName("Family Name")]
        public string? LastName { get; set; }
        
        /// <summary>
        /// Gets and Sets the professional operator number.
        /// </summary>
        [Required]
        [RegularExpression(DataValueConstants.ProfessionalOperatorNumberRegex, ErrorMessage = "Your six-digit professional operator number must be provided.")]
        [DisplayName("Professional Operator Number")]
        public string? ProfessionalOperatorNumber { get; set; }

        /// <summary>
        /// Gets and Sets a contact telephone number.
        /// </summary>
        [EapcTelephone(ErrorMessage = "The provided telephone number is invalid.")]
        [Required]
        [DisplayName("Contact Telephone")]
        public string? TelephoneNumber { get; set; }

        /// <summary>
        /// Gets and Sets the company name the user works for.
        /// </summary>
        [Required]
        [MaxLength(DataValueConstants.CompanyNameMaxLength)]
        [DisplayName("Company Name")]
        public string? CompanyName { get; set; }

        /// <summary>
        /// Gets and Sets line 1 of the contact address.
        /// </summary>
        [DisplayName("Address Line 1")]
        [MaxLength(DataValueConstants.AddressLineMaxLength)]
        public string? AddressLine1 { get; set; }
        
        /// <summary>
        /// Gets and Sets line 2 of the contact address.
        /// </summary>
        [DisplayName("Address Line 2")]
        [MaxLength(DataValueConstants.AddressLineMaxLength)]
        public string? AddressLine2 { get; set; }
        
        /// <summary>
        /// Gets and Sets line 3 of the contact address.
        /// </summary>
        [DisplayName("Address Line 3")]
        [MaxLength(DataValueConstants.AddressLineMaxLength)]
        public string? AddressLine3 { get; set; }
        
        /// <summary>
        /// Gets and Sets line 4 of the contact address.
        /// </summary>
        [DisplayName("Address Line 4")]
        [MaxLength(DataValueConstants.AddressLineMaxLength)]
        public string? AddressLine4 { get; set; }
        
        /// <summary>
        /// Gets and Sets line postal code of the contact address.
        /// </summary>
        [DisplayName("Postal Code")]
        [MaxLength(DataValueConstants.PostalCodeMaxLength)]
        public string? PostalCode { get; set; }

        /// <summary>
        /// Gets and Sets the user's credit reference number.
        /// </summary>
        [DisplayName("Credit Customer Number")]
        public string? CreditReferenceNumber { get; set; }

        /// <summary>
        /// Gets and Sets whether the user is signing up to the credit terms and conditions.
        /// </summary>
        [DisplayName("Accept Credit Terms and Conditions?")]
        public bool AcceptsCreditTermsAndConditions { get; set; }

        /// <summary>
        /// Gets and Sets the user's default home nation.
        /// </summary>
        [DisplayName("Region")]
        public HomeNation? HomeNation { get; set; }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(CreditReferenceNumber) && !AcceptsCreditTermsAndConditions)
            {
                yield return new ValidationResult(
                    "A credit reference number must be provided or you must agree to the credit terms and conditions.",
                    new [] {nameof(CreditReferenceNumber)});
            }
        }
    }
}
