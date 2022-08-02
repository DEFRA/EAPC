using System;
using System.Text.Json.Serialization;

namespace Forestry.Eapc.External.Web.Models.Repository
{
    public class CertificateApplication
    {
        [JsonIgnore]
        public const string EntityName = "cr671_certificateapplications";

        [JsonIgnore]
        public const string EntityNameSingular = "cr671_certificateapplication";

        [JsonPropertyName("cr671_certificateapplicationid")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Identifier { get; set; }

        [JsonPropertyName("cr671_referenceidentifier")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ReferenceIdentifier { get; set; }

        [JsonPropertyName("createdon")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? CreationDate { get; set; }

        private string? _createdByContactLink;
        [JsonPropertyName("cr671_CreatedByContact@odata.bind")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CreatedByContact
        {
            get => _createdByContactLink;
            set => _createdByContactLink = $"/contacts({value})";
        }
        
        [JsonPropertyName("cr671_applicationstate")]
        public ApplicationState State { get; set; }

        // Applicant fields
        [JsonPropertyName("cr671_applicantpersonname")]
        public string? PersonName { get; set; }

        [JsonPropertyName("cr671_applicantcompanyname")]
        public string? CompanyName { get; set; }

        [JsonPropertyName("cr671_applicanthomenation")]
        public HomeNation? Region { get; set; }

        [JsonPropertyName("cr671_applicantprofessionaloperatornumber")]
        public string? ProfessionalOperatorNumber { get; set; }

        [JsonPropertyName("cr671_applicantemail")]
        public string? Email { get; set; }

        [JsonPropertyName("cr671_applicanttelephone")]
        public string? Telephone { get; set; }

        // Section 1

        [JsonPropertyName("cr671_exportername")]
        public string? ExporterName { get; set; }
        
        [JsonPropertyName("cr671_exporteraddresscontactname")]
        public string? ExporterAddressContactName { get; set; }

        [JsonPropertyName("cr671_exporteraddressline1")]
        public string? ExporterAddressLine1 { get; set; }

        [JsonPropertyName("cr671_exporteraddressline2")]
        public string? ExporterAddressLine2 { get; set; }

        [JsonPropertyName("cr671_exporteraddressline3")]
        public string? ExporterAddressLine3 { get; set; }

        [JsonPropertyName("cr671_exporteraddressline4")]
        public string? ExporterAddressLine4 { get; set; }
        
        [JsonPropertyName("cr671_exporteraddresspostalcode")]
        public string? ExporterAddressPostalCode { get; set; }

        [JsonPropertyName("cr671_exportstatus")]
        public ExportStatus? ExportStatus { get; set; }
        
        // Section 2

        [JsonPropertyName("cr671_goodsinspectionaddresscontactname")]
        public string? GoodsInspectionAddressContactName { get; set; }

        [JsonPropertyName("cr671_goodsinspectionaddressline1")]
        public string? GoodsInspectionAddressLine1 { get; set; }

        [JsonPropertyName("cr671_goodsinspectionaddressline2")]
        public string? GoodsInspectionAddressLine2 { get; set; }

        [JsonPropertyName("cr671_goodsinspectionaddressline3")]
        public string? GoodsInspectionAddressLine3 { get; set; }

        [JsonPropertyName("cr671_goodsinspectionaddressline4")]
        public string? GoodsInspectionAddressLine4 { get; set; }

        [JsonPropertyName("cr671_goodsinspectionaddresspostalcode")]
        public string? GoodsInspectionAddressPostalCode { get; set; }

        [JsonPropertyName("cr671_goodsinspectionadditionalinformation")]
        public string? GoodsInspectionAdditionalInformation { get; set; }


        // Section 3
        [JsonPropertyName("cr671_commoditytype")]
        public string? CommodityType { get; set; }

        [JsonPropertyName("cr671_consigneename")]
        public string? ConsigneeName { get; set; }

        [JsonPropertyName("cr671_consigneeaddresscontactname")]
        public string? ConsigneeAddressContactName { get; set; }

        [JsonPropertyName("cr671_consigneeaddressline1")]
        public string? ConsigneeAddressLine1 { get; set; }

        [JsonPropertyName("cr671_consigneeaddressline2")]
        public string? ConsigneeAddressLine2 { get; set; }

        [JsonPropertyName("cr671_consigneeaddressline3")]
        public string? ConsigneeAddressLine3 { get; set; }

        [JsonPropertyName("cr671_consigneeaddressline4")]
        public string? ConsigneeAddressLine4 { get; set; }

        [JsonPropertyName("cr671_consigneeaddressline5")]
        public string? ConsigneeAddressLine5 { get; set; }

        [JsonPropertyName("cr671_portofimport")]
        public string? PortOfImport { get; set; }

        [JsonPropertyName("cr671_portofexport")]
        public string? PortOfExport { get; set; }

        [JsonPropertyName("cr671_dateofexport")]
        public DateTime? DateOfExport { get; set; }
        
        [JsonPropertyName("cr671_countryofdestination")]
        public string? CountryOfDestination { get; set; }

        [JsonPropertyName("cr671_descriptionofproducts")]
        public string? DescriptionOfProducts { get; set; }

        [JsonPropertyName("cr671_botanicalname")]
        public string? BotanicalName { get; set; }

        [JsonPropertyName("cr671_wheregrown")]
        public string? WhereGrown { get; set; }

        [JsonPropertyName("cr671_certificatenumbersfromcountryoforigin")]
        public string? CertificateNumbersFromCountryOfOrigin { get; set; }

        [JsonPropertyName("cr671_meansofconveyance")]
        public TransportType? MeansOfConveyance { get; set; }

        [JsonPropertyName("cr671_meansofconveyanceother")]
        public string? MeansOfConveyanceOther { get; set; }

        [JsonPropertyName("cr671_consignmentquantity")]
        public string? ConsignmentQuantity { get; set; }

        // Section 4

        [JsonPropertyName("cr671_treatment")]
        public string? Treatment { get; set; }

        [JsonPropertyName("cr671_concentration")]
        public string? Concentration { get; set; }

        [JsonPropertyName("cr671_chemical")]
        public string? Chemical { get; set; }

        [JsonPropertyName("cr671_dateoftreatment")]
        public DateTime? DateOfTreatment { get; set; }

        [JsonPropertyName("cr671_duration")]
        public int? Duration { get; set; }

        [JsonPropertyName("cr671_temperature")]
        public int? Temperature { get; set; }

        [JsonPropertyName("cr671_additionaldeclarations")]
        public string? AdditionalDeclarations { get; set; }

        [JsonPropertyName("cr671_additionalinformation")]
        public string? AdditionalInformation { get; set; }

        // Section 5
        [JsonPropertyName("cr671_certificatedeliveryaddresscontactname")]
        public string? CertificateDeliveryAddressContactName { get; set; }

        [JsonPropertyName("cr671_certificatedeliveryaddressline1")]
        public string? CertificateDeliveryAddressLine1 { get; set; }

        [JsonPropertyName("cr671_certificatedeliveryaddressline2")]
        public string? CertificateDeliveryAddressLine2 { get; set; }

        [JsonPropertyName("cr671_certificatedeliveryaddressline3")]
        public string? CertificateDeliveryAddressLine3 { get; set; }

        [JsonPropertyName("cr671_certificatedeliveryaddressline4")]
        public string? CertificateDeliveryAddressLine4 { get; set; }

        [JsonPropertyName("cr671_certificatedeliveryaddresspostalcode")]
        public string? CertificateDeliveryAddressPostalCode { get; set; }

        [JsonPropertyName("cr671_pdfcopyrequested")]
        public bool? PdfCopyRequested { get; set; }

        [JsonPropertyName("cr671_customerpurchaseordernumber")]
        public string? CustomerPurchaseOrderNumber { get; set; }

        [JsonPropertyName("cr671_customercreditnumber")]
        public string? CustomerCreditNumber { get; set; }

        [JsonPropertyName("cr671_accepttermsandconditions")]
        public bool? AcceptTermsAndConditions { get; set; }


        [JsonPropertyName("cr671_additionaldeclarationsnotrequired")]
        public bool? AdditionalDeclarationsNotRequired { get; set; }

        [JsonPropertyName("cr671_goodsinspectionnotrequired")]
        public bool? GoodsInspectionNotRequired { get; set; }

        [JsonPropertyName("cr671_supportingdocumentationnotrequired")]
        public bool? SupportingDocumentationNotRequired { get; set; }
    }
}