using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Forestry.Eapc.External.Web.Services.Repositories.Users
{
    /// <summary>
    /// Defines the contract for the local repository of user information.
    /// </summary>
    public interface ILocalUserRepository
    {
        /// <summary>
        /// This method is invoked by the infrastructure when a user authenticates using Azure B2C and is redirected to our web application.
        /// Implementations may use this method to amend the <see cref="ClaimsPrincipal"/> representing the user before the data is written
        /// to the User property of the current Http Context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The primary purpose of this method is for the local user repository information to append any additional data for the user that
        /// has authenticated. This is done by adding one or more additional <see cref="ClaimsIdentity"/> instances to the provided
        /// <see cref="ClaimsPrincipal"/> populated with one or more <see cref="Claim"/> values.
        /// </para>
        /// <para>
        /// The provided user is guaranteed to have a <see cref="Claim"/> of type <see cref="ClaimTypes.NameIdentifier"/> which represents the
        /// user's unique identifier within Azure AD B2C. This value can be used as a primary key within a local user repository to maintain
        /// a 1:1 mapping if required.
        /// </para>
        /// </remarks>
        /// <param name="user">The authenticated user that has been returned from Azure AD B2C.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task HandleUserLoginAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Persists the data from the <paramref name="externalUser"/> to the local user repository.
        /// </summary>
        /// <param name="externalUser">The user for whom profile data should be updated.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task<ExternalUser> SaveProfileDataAsync(ExternalUser externalUser, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets a given user account in the local repository to approved.
        /// </summary>
        /// <remarks>
        /// Implementations are expected to account for receiving inputs corresponding to accounts that have already been approved. In such cases the implementation
        /// should return true and no update should occur to the underlying user profile data.
        /// </remarks>
        /// <param name="email">The email address of the account to approve.</param>
        /// <param name="professionalOperatorNumber">The professional operator number of the account to approve.</param>
        /// <param name="externalUser">The user carrying out the approval.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>true if the user account was found and approved, else false.</returns>
        Task<bool> ApproveUserAccountAsync(string email, string professionalOperatorNumber, ExternalUser externalUser, CancellationToken cancellationToken);
    }
}