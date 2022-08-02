namespace Forestry.Eapc.External.Web.Configuration
{
    public class AzureAdB2COptions
    {
        public string Instance { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string SignUpSignInPolicyId { get; set; }
        public string ResetPasswordPolicyId { get; set; }
        public string EditProfilePolicyId { get; set; }
        
        public string Authority => $"{Instance}/tfp/{Tenant}/{SignUpSignInPolicyId}/v2.0";
    }
}
