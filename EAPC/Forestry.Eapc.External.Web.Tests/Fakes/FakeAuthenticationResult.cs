using System;
using System.Collections.Generic;
using Microsoft.Identity.Client;

namespace Forestry.Eapc.External.Web.Tests.Fakes
{
    public class FakeAuthenticationResult : AuthenticationResult
    {
        public FakeAuthenticationResult(string accessToken, bool isExtendedLifeTimeToken, string uniqueId, DateTimeOffset expiresOn, DateTimeOffset extendedExpiresOn, string tenantId, IAccount account, string idToken, IEnumerable<string> scopes, Guid correlationId, string tokenType = "Bearer", AuthenticationResultMetadata authenticationResultMetadata = null) : base(accessToken, isExtendedLifeTimeToken, uniqueId, expiresOn, extendedExpiresOn, tenantId, account, idToken, scopes, correlationId, tokenType, authenticationResultMetadata)
        {
        }

        public FakeAuthenticationResult(string accessToken, bool isExtendedLifeTimeToken, string uniqueId, DateTimeOffset expiresOn, DateTimeOffset extendedExpiresOn, string tenantId, IAccount account, string idToken, IEnumerable<string> scopes, Guid correlationId, AuthenticationResultMetadata authenticationResultMetadata, string tokenType = "Bearer") : base(accessToken, isExtendedLifeTimeToken, uniqueId, expiresOn, extendedExpiresOn, tenantId, account, idToken, scopes, correlationId, authenticationResultMetadata, tokenType)
        {
        }

        public FakeAuthenticationResult(string accessToken)
        : base(accessToken, true, "uniqueid", DateTimeOffset.MaxValue, DateTimeOffset.MaxValue, "tenantid", null, "idtoken", new string[0], Guid.NewGuid())
        {

        }
    }
}