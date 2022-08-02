using System.Collections.Generic;

namespace Forestry.Eapc.External.Web.Models.Application;

public class SupportingDocumentsSection
{
        
    public bool SupportingDocumentationNotRequired { get; set; }

    public ICollection<SupportingDocument> SupportingDocuments { get; set; } = new List<SupportingDocument>();
}