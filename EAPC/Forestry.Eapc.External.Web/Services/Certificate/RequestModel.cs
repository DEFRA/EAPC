namespace Forestry.Eapc.External.Web.Services.Certificate
{
    public class RequestModel
    {
        public string? watermarkText { get; set; } = "DRAFT";
        public string nameAndAddressOfExporter { get; set; }
        public string certificateNumber { get; set; }
        public string declaredNameAndAddressOfConsignee { get; set; }
        public string toPlantProtectionOrganisation { get; set; }
        public string placeOfOrigin { get; set; }
        public string declaredMeansOfConveyance { get; set; }
        public string declaredPointOfEntry { get; set; }
        public string description { get; set; }
        public string quantityDeclared { get; set; }
        public string additionalDeclaration { get; set; }
        public string disinfectionTreatment { get; set; }
        public string disinfectionChemical { get; set; }
        public string disinfectionDurationAndTemperature { get; set; }
        public string disinfectionConcentration { get; set; }
        public string disinfectionDate { get; set; }
        public string disInfectionAdditionalInfo { get; set; }
        public string placeOfIssue { get; set; }
        public string signatureDate { get; set; }
        public string signatureName { get; set; }
    }
}
