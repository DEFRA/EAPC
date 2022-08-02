using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Mapping;

namespace Forestry.Eapc.External.Web.Services
{
    public class CountryProvider
    {
        private static readonly Lazy<IReadOnlyCollection<Country>> LazyCountries = new(LoadCountries);

        public IReadOnlyCollection<Country> GetCountries()
        {
            return LazyCountries.Value;
        }

        private static IReadOnlyCollection<Country> LoadCountries()
        {
            var assembly = typeof(CountryProvider).Assembly;
            var resourceName = assembly
                .GetManifestResourceNames()
                .Single(x => x.EndsWith("FCDO_Geographical_Names_Index.csv"));

            using var stream = assembly.GetManifestResourceStream(resourceName);

            var csvParserOptions = new CsvParserOptions(true, ',');
            var parser = new CsvParser<Country>(csvParserOptions, new CountryMapping());
            
            var result = parser
                .ReadFromStream(stream, Encoding.UTF8)
                .Select(x => x.Result)
                .ToList()
                .AsReadOnly();

            return result;
        }

        private class CountryMapping : CsvMapping<Country>
        {
            public CountryMapping()
            {
                MapProperty(0, x => x.Code);
                MapProperty(1, x => x.Name);
                MapProperty(2, x => x.OfficialName);
            }
        }
    }

#pragma warning disable 8618
    public class Country
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string OfficialName { get; set; }
    }
#pragma warning restore 8618
}
