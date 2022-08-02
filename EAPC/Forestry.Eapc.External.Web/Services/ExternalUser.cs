using System;
using System.Linq;
using System.Security.Claims;
using Forestry.Eapc.External.Web.Infrastructure;
using Forestry.Eapc.External.Web.Infrastructure.User;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services
{
    public class ExternalUser
    {
        private const string LocalClaimLabel = "ExternalUserLocalSource";

        private readonly ClaimsPrincipal _principal;
        
        public ExternalUser(ClaimsPrincipal principal)
        {
            _principal = principal ?? throw new ArgumentNullException(nameof(principal));
            if (!_principal.Identities.Any(x => x.AuthenticationType == LocalClaimLabel))
            {
                _principal.AddIdentity(new ClaimsIdentity {Label = LocalClaimLabel});
            }
        }

        public bool IsLoggedIn => _principal.IsLoggedIn();

        public bool IsNotLoggedIn => _principal.IsNotLoggedIn();

        public string? IdentityProviderId => GetClaimValue(ClaimTypes.NameIdentifier);

        public string? LocalProfileId => GetClaimValue(EapcClaimTypes.DataVerseInstanceId);

        public string? ProfessionalOperatorNumber
        {
            get => GetClaimValue(EapcClaimTypes.ProfessionalOperatorNumber);
            set => SetClaimValue(EapcClaimTypes.ProfessionalOperatorNumber, value);
        }
        
        public string? Telephone
        {
            get => GetClaimValue(ClaimTypes.OtherPhone);
            set => SetClaimValue(ClaimTypes.OtherPhone, value);
        } 
        
        public string? Email 
        {
            get => GetClaimValue(EapcClaimTypes.Emails);
            set => SetClaimValue(EapcClaimTypes.Emails, value);
        }

        public string? GivenName
        {
            get => GetClaimValue(ClaimTypes.GivenName);
            set => SetClaimValue(ClaimTypes.GivenName, value);
        }

        public string? Surname
        {
            get => GetClaimValue(ClaimTypes.Surname);
            set => SetClaimValue(ClaimTypes.Surname, value);
        }

        public string FullName => string.Concat(GivenName, " ", Surname).Trim();

        public string? CreditAccountReference
        {
            get => GetClaimValue(EapcClaimTypes.CreditAccountReference);
            set => SetClaimValue(EapcClaimTypes.CreditAccountReference, value);
        }

        public string? CompanyName
        {
            get => GetClaimValue(EapcClaimTypes.CompanyName);
            set => SetClaimValue(EapcClaimTypes.CompanyName, value);
        }

        public string? StreetAddressLine1
        {
            get => GetClaimValue(ClaimTypes.StreetAddress);
            set => SetClaimValue(ClaimTypes.StreetAddress, value);
        }

        public string? StreetAddressLine2
        {
            get => GetClaimValue(EapcClaimTypes.StreetAddressLine2);
            set => SetClaimValue(EapcClaimTypes.StreetAddressLine2, value);
        }

        public string? StreetAddressLine3
        {
            get => GetClaimValue(EapcClaimTypes.StreetAddressLine3);
            set => SetClaimValue(EapcClaimTypes.StreetAddressLine3, value);
        }

        public string? StreetAddressLine4
        {
            get => GetClaimValue(EapcClaimTypes.StreetAddressLine4);
            set => SetClaimValue(EapcClaimTypes.StreetAddressLine4, value);
        }

        public string? PostalCode
        {
            get => GetClaimValue(ClaimTypes.PostalCode);
            set => SetClaimValue(ClaimTypes.PostalCode, value);
        }

        public bool SignedUpToCreditTermsAndConditions
        {
            get => bool.TryParse(GetClaimValue(EapcClaimTypes.HasAgreedCreditTermsAndConditions), out var asBool) && asBool;
            set => SetClaimValue(EapcClaimTypes.HasAgreedCreditTermsAndConditions, value.ToString());
        }

        public bool IsApprovedAccount
        {
            get
            {
                var stringClaim = GetClaimValue(EapcClaimTypes.ApprovedAccount);
                if (string.IsNullOrWhiteSpace(stringClaim) || !bool.TryParse(stringClaim, out var asBoolean))
                {
                    return false;
                }

                return asBoolean;
            }
            set => SetClaimValue(EapcClaimTypes.ApprovedAccount, value.ToString());
        }

        public HomeNation? HomeNation
        {
            get
            {
                var stringValue = GetClaimValue(EapcClaimTypes.HomeNation);

                if (string.IsNullOrWhiteSpace(stringValue) || !Enum.TryParse(typeof(HomeNation), stringValue, true, out var result))
                {
                    return null;
                }

                return (HomeNation?) result;
            }
            set => SetClaimValue(EapcClaimTypes.HomeNation, value?.ToString());
        }

        public string? RegistrationEmailRecipient
        {
            get => GetClaimValue(EapcClaimTypes.RegistrationConfirmationEmailRecipient);
            set => SetClaimValue(EapcClaimTypes.RegistrationConfirmationEmailRecipient, value);
        }

        /// <summary>
        /// Gets a boolean indicating whether the user is the key contact for their professional operator.
        /// </summary>
        public bool IsProfessionalOperatorKeyContact => string.IsNullOrWhiteSpace(Email) == false &&
                                                        Email.Equals(RegistrationEmailRecipient,
                                                            StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Gets a reference to the underlying claims principal for this user.
        /// </summary>
        public ClaimsPrincipal Principal => _principal;
        
        private string? GetClaimValue(string claimType)
        {
            return _principal
                .Claims
                .FirstOrDefault(x => x.Type == claimType)
                ?.Value;
        }

        private void SetClaimValue(string claimType, string? value)
        {
            ClaimsIdentity? identityWithClaim = Principal.Identities.FirstOrDefault(x => x.Claims.Any(x => x.Type == claimType));
            
            // if the claim exists then remove it
            if (identityWithClaim != null)
            {
                var claim = identityWithClaim.Claims.Single(x => x.Type == claimType);
                identityWithClaim.RemoveClaim(claim);
            }

            // add a new claim with the value to the identity that contained it originally, or our bucket Identity if was not found
            if (!string.IsNullOrWhiteSpace(value))
            {
                identityWithClaim ??= _principal.Identities.Single(x => x.Label == LocalClaimLabel);
                identityWithClaim.AddClaim(new Claim(claimType, value));
            }
        }
    }
}
