using System;
using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Models.Accounts;
using Forestry.Eapc.External.Web.Services.Repositories.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Forestry.Eapc.External.Web.Services
{
    public class ApproveAccountUseCase
    {
        private readonly ILocalUserRepository _localUserRepository;
        private readonly ILogger<ApproveAccountModel> _logger;

        public ApproveAccountUseCase(
            ILocalUserRepository localUserRepository, 
            ILogger<ApproveAccountModel> logger)
        {
            _localUserRepository = localUserRepository ?? throw new ArgumentNullException(nameof(localUserRepository));
            _logger = logger ?? new NullLogger<ApproveAccountModel>();
        }

        public async Task<ApproveAccountOutcome> ExecuteAsync(ApproveAccountModel model, ExternalUser externalUser, CancellationToken cancellationToken)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (externalUser == null) throw new ArgumentNullException(nameof(externalUser));

            if (externalUser.ProfessionalOperatorNumber == null)
            {
                // this should not happen but is here as a safeguard just in case
                throw new ArgumentException(
                    "Unable to process request as the external user has no professional operator number to use as a cross-check");
            }
            
            if (!externalUser.ProfessionalOperatorNumber.Equals(model.ProfessionalOperatorNumber))
            {
                _logger.LogWarning(
                    "An approve account request was blocked from user {ExternalUser} to approve {Email} at professional operator {ProfessionalOperatorNumber} because the user's professional operator {ExternalUserProfessionalOperatorNumber} does not match",
                    externalUser.Email,
                    model.Email,
                    model.ProfessionalOperatorNumber,
                    externalUser.ProfessionalOperatorNumber);

                return ApproveAccountOutcome.ProfessionalOperatorNumberMismatch;
            }

            _logger.LogDebug(
                "Sending request to repository to approve account {Email} at professional operator {ExternalUserProfessionalOperatorNumber}",
                model.Email,
                model.ProfessionalOperatorNumber);

            var approved = await _localUserRepository.ApproveUserAccountAsync(
                model.Email.Trim(),
                model.ProfessionalOperatorNumber.Trim(),
                externalUser,
                cancellationToken);

            _logger.LogInformation(
                "Approve account operation on repository for {Email} at professional operator {ExternalUserProfessionalOperatorNumber} completed with result {Result}",
                model.Email,
                model.ProfessionalOperatorNumber,
                approved);

            return approved
                ? ApproveAccountOutcome.Success
                : ApproveAccountOutcome.LocalAccountNotFound;
        }
    }

    public enum ApproveAccountOutcome
    {
        /// <summary>
        /// The provided professional operator number for the user account to approve does not match the professional operator number of the current <see cref="ExternalUser"/>.
        /// </summary>
        ProfessionalOperatorNumberMismatch,

        /// <summary>
        /// No local account was found using the email and professional operator number combination provided.
        /// </summary>
        LocalAccountNotFound,

        /// <summary>
        /// The local account was found and is now approved for access.
        /// </summary>
        Success
    }
}
