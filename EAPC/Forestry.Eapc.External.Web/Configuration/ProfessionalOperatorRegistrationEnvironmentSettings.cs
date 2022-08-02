namespace Forestry.Eapc.External.Web.Configuration
{
    /// <summary>
    /// Configuration specific to the Professional Operator Registration environment within Powerapps.
    /// </summary>
    public class ProfessionalOperatorRegistrationEnvironmentSettings
    {
        /// <summary>
        /// Authentication settings to connect to Powerapps.
        /// </summary>
        public PowerappsAuthenticationSettings PowerappsAuthentication { get; set; }

        /// <summary>
        /// The name of the Operator Registrations table in Dataverse.
        /// </summary>
        /// <remarks>
        /// This value must be in lowercase, and singular, e.g. cr671_operatorregistration
        /// </remarks>
        public string OperatorRegistrationTable { get; set; }

        /// <summary>
        /// The name of the field that contains the professional operator number in the operator registrations table.
        /// </summary>
        /// <remarks>
        /// This value must be in lowercase, e.g. cr671_registrationnumber
        /// </remarks>
        public string ProfessionalOperatorNumberField { get; set; }

        /// <summary>
        /// The name of the field that contains the lookup from operator registration to operator.
        /// </summary>
        /// <remarks>
        /// This value must match the case as shown in Powerapps *when you view the column details* e.g. cr671_Operator
        /// </remarks>
        public string OperatorLookupField { get; set; }

        /// <summary>
        /// The name of the field that contains the operator's email address in the operator table.
        /// </summary>
        /// <remarks>
        /// This value must be in lowercase, e.g. cr671_emailaddress1
        /// </remarks>
        public string OperatorEmailField { get; set; }
    }
}