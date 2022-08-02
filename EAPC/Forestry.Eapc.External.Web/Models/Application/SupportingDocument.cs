using System;

namespace Forestry.Eapc.External.Web.Models.Application
{
    public class SupportingDocument
    {
        /// <summary>
        /// The identifier of the document in storage.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// The file name of the document.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The content type of the document.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// The length in bytes of the document.
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// The point in time that the supporting document was uploaded.
        /// </summary>
        public DateTime? CreationDate { get; set; }
    }
}