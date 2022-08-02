using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services
{
    /// <summary>
    /// Defines the contract for application repository methods that are available to our API endpoints when not
    /// operating under the context of an authenticated user.
    /// </summary>
    public interface IApplicationUnattendedRepository
    {
        /// <summary>
        /// Retrieves the application instance by the given identifier.
        /// </summary>
        /// <param name="id">The application identifier.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A populated  <see cref="Application"/> model, or null if no application was found using the provided <paramref name="id"/>.</returns>
        Task<Application?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}
