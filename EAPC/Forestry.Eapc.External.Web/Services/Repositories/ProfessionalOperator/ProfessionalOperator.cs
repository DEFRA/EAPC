using System;
namespace Forestry.Eapc.External.Web.Services.Repositories.ProfessionalOperator
{
    /// <summary>
    /// A DTO of the data returned from a <see cref="IProfessionalOperatorRepository"/> implementation that contains
    /// data items this system needs pertinent to a professional operator.
    /// </summary>
    public class ProfessionalOperator
    {
        /// <summary>
        /// Gets the email address of the key contact linked to the professional operator.
        /// </summary>
        public string KeyContactEmail { get; }

        /// <summary>
        /// Creates a new instance of a <see cref="ProfessionalOperator"/>.
        /// </summary>
        /// <param name="keyContactEmail">The email address of the key contact linked to the professional operator.</param>
        public ProfessionalOperator(string keyContactEmail)
        {
            KeyContactEmail = keyContactEmail ?? throw new ArgumentNullException(nameof(keyContactEmail));
        }

        /// <summary>
        /// Returns whether the provided <paramref name="user"/> is a key contact for the current <see cref="ProfessionalOperator"/>
        /// by comparing the email address of the <see cref="ExternalUser"/> to the <see cref="KeyContactEmail"/> value.
        /// </summary>
        /// <param name="user">The external user to check.</param>
        /// <returns>true if the <paramref name="user"/> is the key contact, else false.</returns>
        public bool IsKeyContact(ExternalUser user)
        {
            return KeyContactEmail.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
