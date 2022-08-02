namespace Forestry.Eapc.External.Web.Configuration
{
    /// <summary>
    /// Configuration specific to the EAPC environment within Powerapps.
    /// </summary>
    public class EapcEnvironmentSettings
    {
        /// <summary>
        /// Authentication settings to connect to Powerapps.
        /// </summary>
        public PowerappsAuthenticationSettings PowerappsAuthentication { get; set; }
    }
}