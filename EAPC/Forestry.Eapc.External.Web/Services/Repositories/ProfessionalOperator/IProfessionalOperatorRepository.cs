using System.Threading;
using System.Threading.Tasks;

namespace Forestry.Eapc.External.Web.Services.Repositories.ProfessionalOperator
{
    public interface IProfessionalOperatorRepository
    {
        /// <summary>
        /// Gets a <see cref="ProfessionalOperator"/> instance from the underlying repository using the value within the
        /// <see cref="ExternalUser.ProfessionalOperatorNumber"/> property as the lookup key.
        /// </summary>
        /// <remarks>
        /// As the <see cref="ExternalUser.ProfessionalOperatorNumber"/> value is not guaranteed to be present, implementations shall
        /// return null if the property has no value. 
        /// </remarks>
        /// <param name="externalUser">The external user to verify.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="ProfessionalOperator"/> instance containing data this system needs read from the repository, or null
        /// if no match was found using the professional operator number read from the <paramref name="externalUser"/>.</returns>
        Task<ProfessionalOperator?> GetAsync(ExternalUser externalUser, CancellationToken cancellationToken = default);
    }
}
