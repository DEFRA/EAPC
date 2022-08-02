using System;
using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Models.Profile;
using Forestry.Eapc.External.Web.Services.Repositories;
using Forestry.Eapc.External.Web.Services.Repositories.ProfessionalOperator;
using Forestry.Eapc.External.Web.Services.Repositories.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Forestry.Eapc.External.Web.Services
{
    public class SignUpToUseSystemUseCase
    {
        private readonly ILocalUserRepository _localUserRepository;
        private readonly IProfessionalOperatorRepository _professionalOperatorRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SignUpToUseSystemUseCase> _logger;

        public SignUpToUseSystemUseCase(
            ILocalUserRepository localUserRepository,
            IProfessionalOperatorRepository professionalOperatorRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SignUpToUseSystemUseCase> logger)
        {
            _localUserRepository = localUserRepository ?? throw new ArgumentNullException(nameof(localUserRepository));
            _professionalOperatorRepository = professionalOperatorRepository ?? throw new ArgumentNullException(nameof(professionalOperatorRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? new NullLogger<SignUpToUseSystemUseCase>();
        }

        public async Task<ApplyProfileOutcome> ApplyProfileAsync(
            ExternalUser user, 
            UserProfileModel profileData, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting apply profile action of SignUpToUseSystemUseCase for user with id {UserId}", user.IdentityProviderId);
            _logger.LogDebug("Calling professional operator repository to determine if user is a key contact for {ProfessionalOperatorNumber}", profileData.ProfessionalOperatorNumber);
            
            try
            {
                user.ProfessionalOperatorNumber ??= profileData.ProfessionalOperatorNumber;

                var professionalOperator = await _professionalOperatorRepository.GetAsync(user, cancellationToken);

                if (professionalOperator == null)
                {
                    _logger.LogWarning(
                        "Apply user profile operation aborted as the professional operator reference {ProfessionalOperatorNumber} was not found by the repository",
                        profileData.ProfessionalOperatorNumber);

                    user.ProfessionalOperatorNumber = null;
                    return ApplyProfileOutcome.ProfessionalOperatorNotFound;
                }

                user.GivenName = profileData.FirstName;
                user.Surname = profileData.LastName;
                user.ProfessionalOperatorNumber = profileData.ProfessionalOperatorNumber;
                user.Telephone = profileData.TelephoneNumber;
                user.CompanyName = profileData.CompanyName;
                user.StreetAddressLine1 = profileData.AddressLine1;
                user.StreetAddressLine2 = profileData.AddressLine2;
                user.StreetAddressLine3 = profileData.AddressLine3;
                user.StreetAddressLine4 = profileData.AddressLine4;
                user.PostalCode = profileData.PostalCode;
                user.CreditAccountReference = profileData.CreditReferenceNumber;
                user.SignedUpToCreditTermsAndConditions = profileData.AcceptsCreditTermsAndConditions;
                user.HomeNation = profileData.HomeNation;
                user.IsApprovedAccount = professionalOperator.IsKeyContact(user);
                user.RegistrationEmailRecipient = professionalOperator.KeyContactEmail;
                
                _logger.LogDebug("Saving profile data to repository for user with id {UserId} with approved account flag {IsApprovedAccount}", user.IdentityProviderId, user.IsApprovedAccount);
                
                var updatedExternalUser = await _localUserRepository.SaveProfileDataAsync(
                    user,
                    cancellationToken);
                _logger.LogDebug("Saved profile data to repository or user with id {UserId}", user.IdentityProviderId);

                await RefreshSigninAsync(updatedExternalUser);

                var result = user.IsApprovedAccount
                    ? ApplyProfileOutcome.UserCanAccessApplications
                    : ApplyProfileOutcome.AccountRequiresApproval;

                _logger.LogInformation("Completed apply profile action of SignUpToUseSystemUseCase for user with id {UserId}, returning {Result}", user.IdentityProviderId, result);
                return result;

            }
            catch (RepositoryException ex)
            {
                _logger.LogError("Exception encountered when attempting to sign the user yp to the system", ex);
                return ApplyProfileOutcome.OperationFailed;
            }
        }

        private async Task RefreshSigninAsync(ExternalUser user)
        {
            if (_httpContextAccessor.HttpContext == null)
                return;

            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user.Principal);
        }
    }

    public enum ApplyProfileOutcome
    {
        /// <summary>
        /// No profile information was saved as the professional operator number does not match a professional operator as determined by the <see cref="IProfessionalOperatorRepository"/>.
        /// </summary>
        ProfessionalOperatorNotFound,

        /// <summary>
        /// The profile data was saved successfully, however the user is not a key contact for the professional operator and we require approval to proceed.
        /// </summary>
        AccountRequiresApproval,

        /// <summary>
        /// The profile data was saved successfully and the user may access phytosanitary certificate applications.
        /// </summary>
        UserCanAccessApplications,

        /// <summary>
        /// Due to an error communicating with at least one of the back-end services the profile information may not have been saved correctly and the operation could be retried by the user.
        /// </summary>
        OperationFailed
    }
}
