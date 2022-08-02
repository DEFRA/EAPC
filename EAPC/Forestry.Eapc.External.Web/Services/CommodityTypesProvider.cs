using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Forestry.Eapc.External.Web.Services
{
    public class CommodityTypesProvider
    {
        private static readonly Lazy<IReadOnlyCollection<string>> LazyCommodityTypes = new(LoadCommodityTypes);

        public IReadOnlyCollection<string> GetAll()
        {
            return LazyCommodityTypes.Value;
        }

        private static IReadOnlyCollection<string> LoadCommodityTypes()
        {
            var assembly = typeof(CountryProvider).Assembly;
            var resourceName = assembly
                .GetManifestResourceNames()
                .Single(x => x.EndsWith("DescriptionCategories.txt"));

            var result = new List<string>(22);
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream!);
            
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    result.Add(line);
                }
            }

            return result;
        }
    }
}
