using System;
using System.Globalization;

namespace Forestry.Eapc.External.Web.Configuration
{
    /// <summary>
    /// Description of the configuration of an AzureAD public client application (desktop/mobile application). This should
    /// match the application registration done in the Azure portal
    /// </summary>
    public class PowerappsAuthenticationSettings
    {
        /// <summary>
        /// instance of Azure AD
        /// </summary>
        public string Instance { get; set; } = "https://login.microsoftonline.com/{0}";

        /// <summary>
        /// Endpoint
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// The Tenant is:
        /// - either the tenant ID of the Azure AD tenant in which this application is registered (a guid)
        /// or a domain name associated with the tenant
        /// - or 'organizations' (for a multi-tenant application)
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// Guid used by the application to uniquely identify itself to Azure AD
        /// This is the application Id, As found in the app registration overview.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// URL of the authority
        /// </summary>
        public string Authority
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture, Instance, Tenant);
            }
        }

        /// <summary>
        /// Client secret (application password)
        /// </summary>
        /// <remarks>Daemon applications can authenticate with AAD through two mechanisms: ClientSecret
        /// (which is a kind of application password: this property)
        /// or a certificate previously shared with AzureAD during the application registration 
        /// (and identified by the CertificateName property belows)
        /// </remarks> 
        public string ClientSecret { get; set; }
    }
}