using System;

namespace Forestry.Eapc.External.Web.Services
{
    public class SectionNavigationLinkModel
    {
        public int SectionNumber { get; }
     
        public string SectionAction { get; }

        public string SectionDisplayName { get; }

        public bool IsCurrent { get; set; }

        public bool HasErrors { get; set; }

        public bool DisplaySectionState { get; } = true;

        public SectionNavigationLinkModel(int sectionNumber, string sectionAction, string sectionDisplayName, bool displaySectionState = true)
        {
            if (sectionNumber <= 0) throw new ArgumentOutOfRangeException(nameof(sectionNumber));

            if (string.IsNullOrEmpty(sectionDisplayName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(sectionDisplayName));

            if (string.IsNullOrWhiteSpace(sectionAction))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(sectionAction));

            SectionNumber = sectionNumber;
            SectionAction = sectionAction;
            SectionDisplayName = sectionDisplayName;
            DisplaySectionState = displaySectionState;
        }
    }
}