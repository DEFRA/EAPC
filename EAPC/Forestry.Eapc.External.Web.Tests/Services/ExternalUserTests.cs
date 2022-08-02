using System.Security.Claims;
using AutoFixture;
using Forestry.Eapc.External.Web.Models.Application;
using Forestry.Eapc.External.Web.Services;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Services
{
    public class ExternalUserTests
    {
        private static readonly IFixture FixtureInstance = new Fixture();

        [Fact]
        public void CanSetValuesWhereSupported()
        {
            var sut = UserFactory.CreateExternalUser();

            // this uses reflection to save on the amount of get and set calls that needed to be made
            SetThenCheck(sut, nameof(ExternalUser.GivenName), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.Surname), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.ProfessionalOperatorNumber), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.Telephone), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.CompanyName), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.StreetAddressLine1), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.StreetAddressLine2), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.StreetAddressLine3), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.StreetAddressLine4), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.PostalCode), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.CreditAccountReference), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.SignedUpToCreditTermsAndConditions), FixtureInstance.Create<bool>());
            SetThenCheck(sut, nameof(ExternalUser.HomeNation), FixtureInstance.Create<HomeNation>());
            SetThenCheck(sut, nameof(ExternalUser.IsApprovedAccount), FixtureInstance.Create<bool>());
        }

        [Fact]
        public void CanSetValuesOnAnEmptyUserWithNoExistingClaims()
        {
            var sut = new ExternalUser(new ClaimsPrincipal());

            // this uses reflection to save on the amount of get and set calls that needed to be made
            SetThenCheck(sut, nameof(ExternalUser.GivenName), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.Surname), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.ProfessionalOperatorNumber), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.Telephone), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.CompanyName), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.StreetAddressLine1), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.StreetAddressLine2), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.StreetAddressLine3), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.StreetAddressLine4), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.PostalCode), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.CreditAccountReference), FixtureInstance.Create<string>());
            SetThenCheck(sut, nameof(ExternalUser.SignedUpToCreditTermsAndConditions), FixtureInstance.Create<bool>());
            SetThenCheck(sut, nameof(ExternalUser.HomeNation), FixtureInstance.Create<HomeNation>());
            SetThenCheck(sut, nameof(ExternalUser.IsApprovedAccount), FixtureInstance.Create<bool>());
        }

        [Fact]
        public void CanNullValuesWhereSupported()
        {
            var sut = UserFactory.CreateExternalUser();

            // the boolean properties are not nullable so are omitted from this
            SetThenCheckNull(sut, nameof(ExternalUser.GivenName));
            SetThenCheckNull(sut, nameof(ExternalUser.Surname));
            SetThenCheckNull(sut, nameof(ExternalUser.ProfessionalOperatorNumber));
            SetThenCheckNull(sut, nameof(ExternalUser.Telephone));
            SetThenCheckNull(sut, nameof(ExternalUser.CompanyName));
            SetThenCheckNull(sut, nameof(ExternalUser.StreetAddressLine1));
            SetThenCheckNull(sut, nameof(ExternalUser.StreetAddressLine2));
            SetThenCheckNull(sut, nameof(ExternalUser.StreetAddressLine3));
            SetThenCheckNull(sut, nameof(ExternalUser.StreetAddressLine4));
            SetThenCheckNull(sut, nameof(ExternalUser.PostalCode));
            SetThenCheckNull(sut, nameof(ExternalUser.CreditAccountReference));
            SetThenCheckNull(sut, nameof(ExternalUser.HomeNation));
        }

        private static void SetThenCheckNull(ExternalUser obj, string propertyName)
        {
            var valueAfterSetGet = SetThenGet(obj, propertyName, null);
            Assert.Null(valueAfterSetGet);
        }

        private static void SetThenCheck(ExternalUser obj, string propertyName, object? setValue)
        {
            var valueAfterSetGet = SetThenGet(obj, propertyName, setValue);
            Assert.Equal(setValue, valueAfterSetGet);
        }
        
        private static object? SetThenGet(ExternalUser obj, string propertyName, object? setValue)
        {
            SetValue(obj, propertyName, setValue);
            return GetValue(obj, propertyName);
        }

        private static object? GetValue(ExternalUser obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName)!.GetValue(obj);
        }

        private static void SetValue(ExternalUser obj, string propertyName, object? value)
        {
            obj.GetType().GetProperty(propertyName)!.SetValue(obj, value);
        }
    }
}
