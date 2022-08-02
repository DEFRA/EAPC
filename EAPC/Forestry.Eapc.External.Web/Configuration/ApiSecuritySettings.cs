namespace Forestry.Eapc.External.Web.Configuration
{
    public class ApiSecuritySettings
    {
        public string AuthenticationHeaderKey { get; set; } = "X-Authorization-Token";

        public string? AuthenticationHeaderValue { get; set; }
    }
}
