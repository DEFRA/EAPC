using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Forestry.Eapc.External.Web.Services
{
    public class BotanicalNamesProvider
    {
        private static readonly Lazy<IReadOnlyCollection<string>> LazyBotanicalNames = new(LoadBotanicalNames);

        public IReadOnlyCollection<string> GetBotanicalNames()
        {
            return LazyBotanicalNames.Value;
        }

        private static IReadOnlyCollection<string> LoadBotanicalNames()
        {
            var assembly = typeof(CountryProvider).Assembly;
            var resourceName = assembly
                .GetManifestResourceNames()
                .Single(x => x.EndsWith("BotanicalNames.txt"));

            var result = new List<string>(20);
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
