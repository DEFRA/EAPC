namespace Forestry.Eapc.External.Web.Configuration
{
    public class SupportingDocumentsSettings
    {
        public int MaxFileSizeBytes { get; set; } = 4194304;

        public int MaxNumberDocuments { get; set; } = 10;

        public AllowedFileType[] AllowedFileTypes = new[]
        {
            new AllowedFileType { Extensions = new[] { "JPG", "JPEG", "PNG" }, Description = "Image" },
            new AllowedFileType { Extensions = new[] { "DOC", "DOCX" }, Description = "Word Document" },
            new AllowedFileType { Extensions = new[] { "PDF" }, Description = "PDF Document" }
        };
    }

    public class AllowedFileType
    {
        public string[] Extensions { get; set; }
        public string Description { get; set; }
    }
}