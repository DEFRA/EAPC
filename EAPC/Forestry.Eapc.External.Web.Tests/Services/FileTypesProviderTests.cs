using Forestry.Eapc.External.Web.Services;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Services
{
    public class FileTypesProviderTests
    {
        [Theory]
        [InlineData("audio/aac", "AAC audio")]
        [InlineData("application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Microsoft Word (OpenXML)")]
        [InlineData("video/mp4", "MP4 audio")]
        public void LocatesMatchingFileTypeUsingKnownMimeType(string mimeType, string kindOfDocument)
        {
            var result = CreateSut().FindFileTypeByMimeType(mimeType);
            Assert.Equal(kindOfDocument, result!.KindOfDocument);
        }

        [Fact]
        public void ReturnsNullUsingUnknownMimetype()
        {
            var result = CreateSut().FindFileTypeByMimeType("foo/bar");
            Assert.Null(result);
        }

        [Theory]
        [InlineData("audio/aac", "AAC audio")]
        [InlineData("application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Microsoft Word (OpenXML)")]
        [InlineData("video/mp4", "MP4 audio")]
        public void WithFallbackLocatesMatchingFileTypeUsingKnownMimeType(string mimeType, string kindOfDocument)
        {
            var result = CreateSut().FindFileTypeByMimeTypeWithFallback(mimeType);
            Assert.Equal(kindOfDocument, result!.KindOfDocument);
        }

        [Fact]
        public void WithFallbackReturnsOctetStreamUsingUnknownMimetype()
        {
            var result = CreateSut().FindFileTypeByMimeTypeWithFallback("foo/bar");
            Assert.Equal("Any kind of binary data", result!.KindOfDocument);
        }

        private FileTypesProvider CreateSut() => new();
    }
}
