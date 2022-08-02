namespace Forestry.Eapc.External.Web.Infrastructure.User
{
    /// <summary>
    /// Defines the claim types that are specific to Forestry EAPC.
    /// </summary>
    public static class EapcClaimTypes
    {
        internal const string ClaimTypeNamespace = "https://www.gov.uk/government/organisations/forestry-commission";

        public const string ProfessionalOperatorNumber = ClaimTypeNamespace + "/professionalOperatorNumber";
        public const string CreditAccountReference = ClaimTypeNamespace + "/creditaccountreference";

        public const string HomeNation = ClaimTypeNamespace + "/homenation";
        public const string CompanyName = ClaimTypeNamespace + "/companyname";
        
        public const string Emails = "emails";
        
        public const string StreetAddressLine2 = ClaimTypeNamespace + "/streetaddress2";
        public const string StreetAddressLine3 = ClaimTypeNamespace + "/streetaddress3";
        public const string StreetAddressLine4 = ClaimTypeNamespace + "/streetaddress4";
        public const string ApprovedAccount = ClaimTypeNamespace + "/approvedaccount";
        public const string HasAgreedCreditTermsAndConditions = ClaimTypeNamespace + "/agreedcredittandc";

        public const string RegistrationConfirmationEmailRecipient = ClaimTypeNamespace + "/registrationemailrecipient";

        public const string DataVerseInstanceId = ClaimTypeNamespace + "/dataverseid";
    }
}
