using System.Security.Claims;

namespace Forestry.Eapc.External.Web.Infrastructure
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsLoggedIn(this ClaimsPrincipal principal)
            => principal.Identity != null && principal.Identity.IsAuthenticated;

        public static bool IsNotLoggedIn(this ClaimsPrincipal principal)
            => !IsLoggedIn(principal);
    }
}
