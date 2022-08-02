namespace Forestry.Eapc.External.Web.Models.Application
{
    /// <summary>
    /// Enumeration of possible states an <see cref="Application"/> may be in during it's lifecycle.
    /// </summary>
    public enum ApplicationState
    {
        /// <summary>
        /// The application has not yet been submitted to Forest Services by an External User.
        /// </summary>
        Draft,

        /// <summary>
        /// The application has been submitted to Forest Services and can no longer be changed by an External User.
        /// </summary>
        Submitted,
        
        /// <summary>
        /// The application has been withdrawn (cancelled). Changes may not be made but a duplicate can be created which will then be in a <see cref="Draft"/> state.
        /// </summary>
        Withdrawn,
        
        /// <summary>
        /// A phytosanitary certificate has been issued by Forest Services for the application.
        /// </summary>
        Issued,

        /// <summary>
        /// An invoice has been issued by Forest Services to obtain payment for services related to the application.
        /// </summary>
        Charged,
        
        /// <summary>
        /// The issued invoice has been paid by the customer organisation.
        /// </summary>
        Paid,

        /// <summary>
        /// The application is in a state that does not match any of the other listed values.
        /// </summary>
        Unknown
    }
}
