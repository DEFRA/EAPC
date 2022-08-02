using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Mapping;

namespace Forestry.Eapc.External.Web.Services
{
    /// <summary>
    /// Input query 
    /// </summary>
    public record AdditionalDeclarationQuery(string? CountryOfOrigin = null, string? CountryOfDestination = null, string? Species = null, string? Treatment = null);

    public record AdditionalDeclarationResult(string Value);

    /// <summary>
    /// Given an <see cref="AdditionalDeclarationQuery"/> query this class will produce a list of possible Additional Declarations that would
    /// need to be included on a phytosanitary certificate.
    /// </summary>
    public class AdditionalDeclarationsProvider
    {
        private static readonly Lazy<IReadOnlyCollection<RecommendationTableRow>> Recommendations = new(LoadRecommendations);

        public IEnumerable<AdditionalDeclarationResult> GetSuggestions(AdditionalDeclarationQuery query)
        {
            var recommendations = Recommendations.Value.AsQueryable();

            if (HasValue(query.CountryOfOrigin))
            {
                recommendations = recommendations.Where(x =>
                    HasValue(x.CountryOfOrigin) == false || x.CountryOfOrigin.Equals(query.CountryOfOrigin, StringComparison.InvariantCultureIgnoreCase));
            }

            if (HasValue(query.CountryOfDestination))
            {
                recommendations = recommendations.Where(x =>
                    HasValue(x.CountryOfDestination) == false || x.CountryOfDestination.Equals(query.CountryOfDestination, StringComparison.InvariantCultureIgnoreCase));
            }

            if (HasValue(query.Species))
            {
                recommendations = recommendations.Where(x =>
                    HasValue(x.Species) == false || x.Species.Equals(query.Species, StringComparison.InvariantCultureIgnoreCase));
            }

            if (HasValue(query.Treatment))
            {
                recommendations = recommendations.Where(x =>
                    HasValue(x.Treatment) == false || x.Treatment.Equals(query.Treatment, StringComparison.InvariantCultureIgnoreCase));
            }

            return recommendations
                .Select(x => new AdditionalDeclarationResult(x.AdditionalDeclaration))
                .Distinct()
                .OrderBy(x => x.Value, StringComparer.InvariantCultureIgnoreCase);
        }

        public IEnumerable<string> GetConfiguredSpecies() => Recommendations
            .Value
            .Select(x => x.Species)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.InvariantCultureIgnoreCase)
            .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase);

        private static bool HasValue(string? value) => !string.IsNullOrWhiteSpace(value);

        private static IReadOnlyCollection<RecommendationTableRow> LoadRecommendations()
        {
            var assembly = typeof(AdditionalDeclarationsProvider).Assembly;
            var resourceName = assembly
                .GetManifestResourceNames()
                .First(x => x.EndsWith("AdditionalDeclarationsSource.csv"));
            
            using var stream = assembly.GetManifestResourceStream(resourceName);
            var parserOptions = new CsvParserOptions(true, ',');
            var parser = new CsvParser<RecommendationTableRow>(parserOptions, new RecommendationTableRowMapping());
            var result = parser
                .ReadFromStream(stream, Encoding.UTF8)
                .Select(x => x.Result)
                .ToList();
            
            return result;
        }

        private class RecommendationTableRow
        {
            public string CountryOfOrigin { get; set; }
            public string CountryOfDestination { get; set; }
            public string Species { get; set; }
            public string Treatment { get; set; }
            public string AdditionalDeclaration { get; set; }
        }

        private class RecommendationTableRowMapping : CsvMapping<RecommendationTableRow>
        {
            public RecommendationTableRowMapping()
            {
                MapProperty(0, x => x.CountryOfOrigin);
                MapProperty(1, x => x.CountryOfDestination);
                MapProperty(2, x => x.Species);
                MapProperty(3, x => x.Treatment);
                MapProperty(4, x => x.AdditionalDeclaration);
            }
        }
    }
}
