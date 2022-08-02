using System.Threading;
using System.Threading.Tasks;

namespace Forestry.Eapc.External.Web.Services.Repositories.ProfessionalOperator
{
    /// <summary>
    /// Fake implementation of the <see cref="IProfessionalOperatorRepository"/> contract whilst we wait for the data schema to be delivered.
    /// </summary>
    public class FakeProfessionalOperatorRepository : IProfessionalOperatorRepository
    {
        /// <inheritdoc />
        public Task<ProfessionalOperator?> GetAsync(ExternalUser externalUser, CancellationToken cancellationToken = default)
        {
            ProfessionalOperator? result = externalUser.Email == null
                ? null: new ProfessionalOperator(externalUser.Email);

            return Task.FromResult(result);
        }
    }
}
