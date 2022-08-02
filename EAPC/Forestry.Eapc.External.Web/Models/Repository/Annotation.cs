using System;
using System.Text.Json.Serialization;

namespace Forestry.Eapc.External.Web.Models.Repository
{
    public class Annotation
    {
        [JsonPropertyName("objectid_cr671_certificateapplication@odata.bind")]
        public string ApplicationIdentifier { get; set; }

        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("notetext")]
        public string NoteText { get; set; }

        [JsonPropertyName("filename")]
        public string FileName { get; set; }

        [JsonPropertyName("mimetype")]
        public string MimeType { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("filesize")]
        public int FileSize { get; set; }

        [JsonPropertyName("isdocument")] 
        public bool IsDocument { get; set; } = true;

        [JsonPropertyName("documentbody")]
        public string DocumentBody { get; set; }

        [JsonPropertyName("createdon")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? CreatedOn { get; set; }

        [JsonPropertyName("annotationid")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Identifier { get; set; }
    }
}