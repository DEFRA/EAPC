using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Infrastructure.User;
using Microsoft.AspNetCore.Http;

namespace Forestry.Eapc.External.Web.Infrastructure.TestUser
{
    /// <summary>
    /// Middleware for ensuring the current <see cref="HttpContext"/> has an authenticated user that contains
    /// some sample test data items.
    /// </summary>
    public class TestUserMiddleware
    {
        private readonly RequestDelegate _next;

        public TestUserMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = CreateTestUser();
            context.User = user;

            await _next(context);
        }

        private static ClaimsPrincipal CreateTestUser()
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(EapcClaimTypes.Emails, "test.user@qxlva.com"));
            claims.Add(new Claim(ClaimTypes.GivenName, "Test-Given-Name"));
            claims.Add(new Claim(ClaimTypes.Surname, "Test-Surname"));
            claims.Add(new Claim(ClaimTypes.OtherPhone, "01249 751000"));
            claims.Add(new Claim(EapcClaimTypes.ProfessionalOperatorNumber, "4587412569"));
            claims.Add(new Claim(EapcClaimTypes.CompanyName, "Timber Woods Export Ltd."));
            claims.Add(new Claim(EapcClaimTypes.CreditAccountReference, "45871102525"));
            claims.Add(new Claim(EapcClaimTypes.HomeNation, "England"));
            claims.Add(new Claim(EapcClaimTypes.ApprovedAccount, true.ToString()));
            claims.Add(new Claim(EapcClaimTypes.DataVerseInstanceId, "f211021f-eedf-eb11-bacb-000d3a86b00c"));
            var identity = new ClaimsIdentity(claims, "test-setup");
            var result = new ClaimsPrincipal(identity);
            return result;
        }
    }
}
