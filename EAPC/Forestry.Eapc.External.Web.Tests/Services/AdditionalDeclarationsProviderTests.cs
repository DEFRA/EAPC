using System.Linq;
using Forestry.Eapc.External.Web.Services;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Services
{
    public class AdditionalDeclarationsProviderTests
    {
        [Fact]
        public void WhenNoMatchingResults()
        {
            var query = new AdditionalDeclarationQuery("Mars");
            var results = CreateSut().GetSuggestions(query);
            Assert.Empty(results);
        }

        [Fact]
        public void WhenMatchingResults()
        {
            var query = new AdditionalDeclarationQuery("Canada", Species: "Ash");
            var results = CreateSut().GetSuggestions(query).ToList();
            Assert.Equal(2, results.Count);
            Assert.Contains(results, x => x.Value == "Consignment complies with Annex II. 1. B Point 1(B) of Commission Implementing Decision (EU) 2015/893.");
            Assert.Contains(results, x => x.Value == "Consignment is in accordance with European Union requirements laid down in Commission Implementing Regulation (EU) 2020/918.");
        }

        [Fact]
        public void WhenNoQueryValuesProvidedShouldReturnAllResults()
        {
            var query = new AdditionalDeclarationQuery();
            var results = CreateSut().GetSuggestions(query).ToList();
            Assert.Equal(11, results.Count);
        }

        [Theory]
        [InlineData("Canada", null, "Ash", null, 2)]
        [InlineData("United States", null, "Oak", null, 1)]
        [InlineData(null, null, "Oak", null, 2)]
        [InlineData("canada", null, "ash", null, 2)]
        [InlineData("canada", null, "", null, 2)]
        [InlineData("Germany", null, "", null, 1)]
        [InlineData("Germany", null, "any", null, 1)]
        public void QueryCombinationTests(string origin, string destination, string species, string treatment, int expectedResultCount)
        {
            var query = new AdditionalDeclarationQuery(origin, destination, species, treatment);
            var results = CreateSut().GetSuggestions(query);
            Assert.Equal(expectedResultCount, results.Count());
        }

        private AdditionalDeclarationsProvider CreateSut() => new AdditionalDeclarationsProvider();

    }
}
