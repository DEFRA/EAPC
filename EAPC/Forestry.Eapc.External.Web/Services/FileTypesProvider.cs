using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Mapping;

namespace Forestry.Eapc.External.Web.Services
{
    /// <summary>
    /// Maps common known combinations file types, file extensions, and MIME types.
    /// </summary>
    /// <remarks>
    /// This implementation is backed by a .csv file sourced from https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types
    /// </remarks>
    public class FileTypesProvider
    {
        private static readonly Lazy<IReadOnlyCollection<FileType>> LazyFileTypes = new(LoadFileTypes);
        
        /// <summary>
        /// Locates and returns a <see cref="FileType"/> where the <see cref="FileType.MimeType"/> value matches the provided <paramref name="value"/>.
        /// </summary>
        /// <remarks>
        /// The search comparison on mime type is case-insensitive.
        /// </remarks>
        /// <param name="value">A string representation of a file mime type.</param>
        /// <returns>A <see cref="FileType"/> instance or null if no match was found.</returns>
        public FileType? FindFileTypeByMimeType(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var fileTypes = LazyFileTypes.Value;
            var result = fileTypes.FirstOrDefault(x => value.Equals(x.MimeType, StringComparison.InvariantCultureIgnoreCase));
            return result;
        }

        /// <summary>
        /// Locates and returns a <see cref="FileType"/> where the <see cref="FileType.MimeType"/> value matches the provided <paramref name="value"/>.
        /// If no match is found using the provided <paramref name="value"/> then a <see cref="FileType"/> instance will be returned where the
        /// <see cref="FileType.MimeType"/> value equals the generic "application/octet-stream".
        /// </summary>
        /// <param name="value">A string representation of a file mime type.</param>
        /// <returns>A <see cref="FileType"/> instance.</returns>
        public FileType FindFileTypeByMimeTypeWithFallback(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            const string fallbackMimeType = "application/octet-stream";
            var result = FindFileTypeByMimeType(value) ?? FindFileTypeByMimeType(fallbackMimeType);

            return result!;
        }
        
        private static IReadOnlyCollection<FileType> LoadFileTypes()
        {
            var assembly = typeof(FileTypesProvider).Assembly;
            var resourceName = assembly
                .GetManifestResourceNames()
                .Single(x => x.EndsWith("file-types.csv"));

            using var stream = assembly.GetManifestResourceStream(resourceName);

            var csvParserOptions = new CsvParserOptions(true, ',');
            var parser = new CsvParser<FileType>(csvParserOptions, new FileTypeMapping());
            
            var result = parser
                .ReadFromStream(stream, Encoding.UTF8)
                .Select(x => x.Result)
                .ToList()
                .AsReadOnly();

            return result;
        }

        private class FileTypeMapping : CsvMapping<FileType>
        {
            public FileTypeMapping()
            {
                MapProperty(0, x => x.Extension);
                MapProperty(1, x => x.KindOfDocument);
                MapProperty(2, x => x.MimeType);
            }
        }
    }

#pragma warning disable 8618
    public class FileType
    {
        public string Extension { get; set; }
        public string KindOfDocument { get; set; }
        public string MimeType { get; set; }
    }
#pragma warning restore 8618
}
