using System.Collections.Generic;
using System.Security.Claims;
using Forestry.Eapc.External.Web.Infrastructure.User;
using Forestry.Eapc.External.Web.Services;

namespace Forestry.Eapc.External.Web.Tests
{
    internal static class UserFactory
    {
        internal static ClaimsPrincipal CreateClaimsPrincipal(bool withOperatorNumber = true)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(EapcClaimTypes.Emails, "test.user@qxlva.com"));
            claims.Add(new Claim(ClaimTypes.GivenName, "Test-Given-Name"));
            claims.Add(new Claim(ClaimTypes.Surname, "Test-Surname"));
            claims.Add(new Claim(ClaimTypes.OtherPhone, "01234 567890"));
            claims.Add(new Claim(EapcClaimTypes.CompanyName, "Test Company"));
            if (withOperatorNumber)
            {
                claims.Add(new Claim(EapcClaimTypes.ProfessionalOperatorNumber, "4587412569"));
            }
            claims.Add(new Claim(EapcClaimTypes.CreditAccountReference, "45871102525"));
            claims.Add(new Claim(EapcClaimTypes.HomeNation, "England"));
            var identity = new ClaimsIdentity(claims, "test-setup");
            var result = new ClaimsPrincipal(identity);
            return result;
        }

        internal static ExternalUser CreateExternalUser() => new(CreateClaimsPrincipal());

        internal static ExternalUser CreateExternalUserWithoutOperatorNumber() => new(CreateClaimsPrincipal(false));
    }
}
