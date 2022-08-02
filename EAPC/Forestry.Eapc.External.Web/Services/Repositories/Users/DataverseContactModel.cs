using System.Text.Json.Serialization;
using Forestry.Eapc.External.Web.Models.Repository;

namespace Forestry.Eapc.External.Web.Services.Repositories.Users
{
    public class DataverseContactModel
    {
        [JsonPropertyName("contactid")]
        public string DataVerseId { get; set; }

        [JsonPropertyName("firstname")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastname")]
        public string LastName { get; set; }

        [JsonPropertyName("emailaddress1")]
        public string EmailAddress { get; set; }

        [JsonPropertyName("address1_telephone1")]
        public string Telephone{ get; set; }

        [JsonPropertyName("address1_line1")]
        public string AddressLine1 { get; set; }
        
        [JsonPropertyName("address1_line2")]
        public string AddressLine2 { get; set; }
        
        [JsonPropertyName("address1_line3")]
        public string AddressLine3 { get; set; }

        [JsonPropertyName("address1_stateorprovince")]
        public string AddressLine4 { get; set; }
        
        [JsonPropertyName("address1_postalcode")]
        public string PostalCode { get; set; }
        
        [JsonPropertyName("cr671_creditaccountreference")]
        public string? CreditAccountReference { get; set; }

        [JsonPropertyName("cr671_professionaloperatornumber")]
        public string ProfessionalOperatorNumber { get; set; }
        
        [JsonPropertyName("cr671_identityproviderid")]
        public string IdentityProviderId { get; set; }
        
        [JsonPropertyName("cr671_homenation")]
        public HomeNation? HomeNation { get; set; }
        
        [JsonPropertyName("cr671_credittermsandconditionssignup")]
        public bool IsCreditTermsAndConditionsSignup { get; set; }
        
        [JsonPropertyName("cr671_exportercompanyname")]
        public string CompanyName { get; set; }

        [JsonPropertyName("cr671_approvedbyprofessionaloperator")]
        public bool IsApprovedByProfessionalOperator { get; set; }

        [JsonPropertyName("cr671_registrationconfirmationtargetemail")]
        public string? RegistrationConfirmationEmailTarget { get; set; }
    }
}
