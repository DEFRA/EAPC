using Forestry.Eapc.External.Web.Services;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Services
{
    public class CountryProviderTests
    {
        [Fact]
        public void ShouldReturnCountriesFromResource()
        {
            var sut = new CountryProvider();
            var result = sut.GetCountries();

            Assert.Equal(195, result.Count);

            Assert.Contains(result,
                x => x.Code == "AF" && x.Name == "Afghanistan" && x.OfficialName == "The Islamic Republic of Afghanistan");

            Assert.Contains(result,
                x => x.Code == "ZW" && x.Name == "Zimbabwe" && x.OfficialName == "The Republic of Zimbabwe");
        }
    }
}
