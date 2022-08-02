using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services.Repositories.DataVerse
{
    public static class ModelMap
    {
        public static Models.Repository.CertificateApplication ToCertificateApplication(this Models.Application.Application value)
        {
            var result = new Models.Repository.CertificateApplication();

            result.State = MapEnum<Models.Repository.ApplicationState>(value.State);
            result.ReferenceIdentifier = value.ReferenceIdentifier;

            // applicant

            result.PersonName = value.Applicant.PersonName;
            result.CompanyName = value.Applicant.CompanyName;
            if (value.Applicant.Region.HasValue)
            {
                result.Region = MapEnum<Models.Repository.HomeNation>(value.Applicant.Region.Value);
            }

            result.ProfessionalOperatorNumber = value.Applicant.ProfessionalOperatorNumber;
            result.Email = value.Applicant.Email;
            result.Telephone = value.Applicant.Telephone;
            if (value.Applicant.ExportStatus.HasValue)
            {
                result.ExportStatus = value.Applicant.ExportStatus switch
                {
                    ExportStatus.New => Models.Repository.ExportStatus.New,
                    ExportStatus.Reforwarded => Models.Repository.ExportStatus.Reforwarded,
                    _ => result.ExportStatus
                };
            }
            else
            {
                result.ExportStatus = null;
            }

            // section 1

            result.ExporterName = value.Section1.ExporterName;
            result.ExporterAddressContactName = value.Section1.ExporterAddress.ContactName;
            result.ExporterAddressLine1 = value.Section1.ExporterAddress.Line1;
            result.ExporterAddressLine2 = value.Section1.ExporterAddress.Line2;
            result.ExporterAddressLine3 = value.Section1.ExporterAddress.Line3;
            result.ExporterAddressLine4 = value.Section1.ExporterAddress.Line4;
            result.ExporterAddressPostalCode = value.Section1.ExporterAddress.PostalCode;

            // section 2

            result.GoodsInspectionAddressContactName = value.Section2.GoodsInspectionAddress.ContactName;
            result.GoodsInspectionAddressLine1 = value.Section2.GoodsInspectionAddress.Line1;
            result.GoodsInspectionAddressLine2 = value.Section2.GoodsInspectionAddress.Line2;
            result.GoodsInspectionAddressLine3 = value.Section2.GoodsInspectionAddress.Line3;
            result.GoodsInspectionAddressLine4 = value.Section2.GoodsInspectionAddress.Line4;
            result.GoodsInspectionAddressPostalCode = value.Section2.GoodsInspectionAddress.PostalCode;
            result.GoodsInspectionAdditionalInformation = value.Section2.AdditionalInformation;
            result.GoodsInspectionNotRequired = value.Section2.InspectionNotRequired;

            // section 3
            result.CommodityType = value.Section4.CommodityType;
            result.ConsigneeName = value.Section3.NameOfConsignee;
            result.ConsigneeAddressContactName = value.Section3.AddressOfConsignee.ContactName;
            result.ConsigneeAddressLine1 = value.Section3.AddressOfConsignee.Line1;
            result.ConsigneeAddressLine2 = value.Section3.AddressOfConsignee.Line2;
            result.ConsigneeAddressLine3 = value.Section3.AddressOfConsignee.Line3;
            result.ConsigneeAddressLine4 = value.Section3.AddressOfConsignee.Line4;
            result.ConsigneeAddressLine5 = value.Section3.AddressOfConsignee.Line5;
            result.PortOfImport = value.Section3.PortOfImport;
            result.PortOfExport = value.Section3.PortOfExport;
            result.DateOfExport = value.Section3.DateOfExport;
            result.CountryOfDestination = value.Section3.CountryOfDestination;
            result.DescriptionOfProducts = value.Section4.DescriptionOfProducts;
            result.BotanicalName = string.Join(", ", value.Section4.BotanicalNames.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()));
            result.WhereGrown = string.Join(", ", value.Section4.WhereGrowns.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => TruncateString(x, 50)));
            result.CertificateNumbersFromCountryOfOrigin = value.Section4.CertificateNumbersFromCountryOfOrigin;
            result.MeansOfConveyanceOther = value.Section4.MeansOfConveyanceOtherText;
            if (value.Section4.MeansOfConveyance.HasValue)
            {
                result.MeansOfConveyance = MapEnum<Models.Repository.TransportType>(value.Section4.MeansOfConveyance.Value);
            }

            result.ConsignmentQuantity = ParseConsignmentQuantityToCrmString(value.Section4.Quantity);

            // section 4

            result.Treatment = value.Section5.Treatment == TreatmentType.Other ? value.Section5.TreatmentOtherText : value.Section5.Treatment?.ToDisplayString();
            result.Concentration = value.Section5.Concentration;
            result.Chemical = value.Section5.Chemical == TreatmentChemical.Other ? value.Section5.ChemicalOtherText : value.Section5.Chemical?.ToDisplayString();
            result.DateOfTreatment = value.Section5.DateOfTreatment;
            result.Duration = value.Section5.Duration;
            result.Temperature = value.Section5.Temperature;
            result.AdditionalInformation = value.Section5.AdditionalInformation;
            result.AdditionalDeclarations = value.Section6.AdditionalDeclarations;
            result.AdditionalDeclarationsNotRequired = value.Section6.AdditionalDeclarationsNotRequired;

            // section 5
            
            result.CertificateDeliveryAddressContactName = value.Section7.CertificateDeliveryAddress.ContactName;
            result.CertificateDeliveryAddressLine1 = value.Section7.CertificateDeliveryAddress.Line1;
            result.CertificateDeliveryAddressLine2 = value.Section7.CertificateDeliveryAddress.Line2;
            result.CertificateDeliveryAddressLine3 = value.Section7.CertificateDeliveryAddress.Line3;
            result.CertificateDeliveryAddressLine4 = value.Section7.CertificateDeliveryAddress.Line4;
            result.CertificateDeliveryAddressPostalCode = value.Section7.CertificateDeliveryAddress.PostalCode;
            result.PdfCopyRequested = value.Section7.PdfCopyRequested;
            result.CustomerPurchaseOrderNumber = value.Section7.CustomerPurchaseOrderNumber;
            result.CustomerCreditNumber = value.Section7.CustomerCreditNumber;

            result.AcceptTermsAndConditions = value.Confirmation.AcceptTermsAndConditions;

            result.SupportingDocumentationNotRequired =
                value.SupportingDocumentsSection.SupportingDocumentationNotRequired;

            return result;
        }

        // TODO these two methods really really need unit testing
        public static string ParseConsignmentQuantityToCrmString(List<Quantity> quantities)
        {
            var result = new StringBuilder();

            foreach (var quantity in quantities.Where(x => x.Amount > 0 && x.Unit.HasValue))
            {
                var unit = quantity.Unit == QuantityUnit.Other ? quantity.OtherText?.Trim() : quantity.Unit.ToString();
                result.Append($"{quantity.Amount} {unit}\r\n");
            }
            
            return result.ToString().Trim();
        }

        public static List<Quantity> ParseCrmConsignmentQuantityString(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new List<Quantity>(0);
            }

            var result = new List<Quantity>();
            using var reader = new StringReader(value);
            var line = reader.ReadLine();
            while (line != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    line = reader.ReadLine();
                    continue;
                };

                line = line.Trim();

                var firstSpace = line.IndexOf(' ');
                if (firstSpace <= 0)
                {
                    line = reader.ReadLine();
                    continue; // unparsable
                }

                var amountText = line.Substring(0, firstSpace);
                var restOfText = line.Substring(firstSpace);
                string? otherText = null;

                if (!decimal.TryParse(amountText.Trim(), out var amountInt))
                {
                    line = reader.ReadLine();
                    continue; // unparsable
                }

                if (!Enum.TryParse(restOfText.Trim(), true, out QuantityUnit unitEnum))
                {
                    otherText = restOfText.Trim();
                    unitEnum = QuantityUnit.Other;
                }

                result.Add(new Quantity {Amount = amountInt, Unit = unitEnum, OtherText = otherText});
                line = reader.ReadLine();
            }

            return result;
        }

        public static Models.Application.Application ToApplication(this Models.Repository.CertificateApplication value)
        {
            var result = new Models.Application.Application();

            result.Identifier = value.Identifier;
            result.ReferenceIdentifier = value.ReferenceIdentifier;
            result.State = MapEnum<Models.Application.ApplicationState>(value.State);
            result.CreationDate = value.CreationDate;

            // applicant

            result.Applicant = new Models.Application.Applicant
            {
                PersonName = value.PersonName,
                CompanyName = value.CompanyName,
                ProfessionalOperatorNumber = value.ProfessionalOperatorNumber,
                Email = value.Email,
                Telephone = value.Telephone
            };
            if (value.Region.HasValue)
            {
                result.Applicant.Region = MapEnum<Models.Application.HomeNation>(value.Region.Value);
            }

            if (value.ExportStatus.HasValue)
            {
                result.Applicant.ExportStatus = value.ExportStatus.Value switch
                {
                    Models.Repository.ExportStatus.New => ExportStatus.New,
                    Models.Repository.ExportStatus.Reforwarded => ExportStatus.Reforwarded,
                    _ => null
                };
            }

            // section 1

            result.Section1.ExporterName = value.ExporterName;
            
            result.Section1.ExporterAddress = new Models.Application.Address
            {
                ContactName = value.ExporterAddressContactName,
                Line1 = value.ExporterAddressLine1,
                Line2 = value.ExporterAddressLine2,
                Line3 = value.ExporterAddressLine3,
                Line4 = value.ExporterAddressLine4,
                PostalCode = value.ExporterAddressPostalCode
            };

            // section 2

            result.Section2.GoodsInspectionAddress = new Models.Application.Address
            {
                ContactName = value.GoodsInspectionAddressContactName,
                Line1 = value.GoodsInspectionAddressLine1,
                Line2 = value.GoodsInspectionAddressLine2,
                Line3 = value.GoodsInspectionAddressLine3,
                Line4 = value.GoodsInspectionAddressLine4,
                PostalCode = value.GoodsInspectionAddressPostalCode,
            };
            result.Section2.InspectionNotRequired =
                value.GoodsInspectionNotRequired.HasValue && value.GoodsInspectionNotRequired.Value;

            result.Section2.AdditionalInformation = value.GoodsInspectionAdditionalInformation;

            // section 3
            result.Section4.CommodityType = value.CommodityType;
            result.Section3.NameOfConsignee = value.ConsigneeName;
            result.Section3.AddressOfConsignee = new Models.Application.Address
            {
                ContactName = value.ConsigneeAddressContactName,
                Line1 = value.ConsigneeAddressLine1,
                Line2 = value.ConsigneeAddressLine2,
                Line3 = value.ConsigneeAddressLine3,
                Line4 = value.ConsigneeAddressLine4,
                Line5 = value.ConsigneeAddressLine5
            };
            result.Section3.PortOfImport = value.PortOfImport;
            result.Section3.PortOfExport = value.PortOfExport;
            result.Section3.DateOfExport = value.DateOfExport;
            result.Section3.CountryOfDestination = value.CountryOfDestination;

            // section 4
            result.Section4.DescriptionOfProducts = value.DescriptionOfProducts;
            result.Section4.BotanicalNames = value.BotanicalName?.Split(',').Select(x => x.Trim()).ToArray() ?? Array.Empty<string>();
            result.Section4.WhereGrowns = value.WhereGrown?.Split(',').Select(x => TruncateString(x, 50)).ToArray() ?? Array.Empty<string>();
            result.Section4.CertificateNumbersFromCountryOfOrigin = value.CertificateNumbersFromCountryOfOrigin;
            result.Section4.Quantity = ParseCrmConsignmentQuantityString(value.ConsignmentQuantity);
            result.Section4.MeansOfConveyanceOtherText = value.MeansOfConveyanceOther;
            if (value.MeansOfConveyance.HasValue)
            {
                result.Section4.MeansOfConveyance = MapEnum<Models.Application.TransportType>(value.MeansOfConveyance);
            }

            // section 5
            var parsedTreatmentType = TreatmentTypeExtensions.FromDisplayString(value.Treatment);
            var parsedTreatmentChemical = TreatmentChemicalExtensions.FromDisplayString(value.Chemical);
            result.Section5.Treatment = parsedTreatmentType.enumValue;
            result.Section5.TreatmentOtherText = parsedTreatmentType.otherText;
            result.Section5.Concentration = value.Concentration;
            result.Section5.Chemical = parsedTreatmentChemical.enumValue;
            result.Section5.ChemicalOtherText = parsedTreatmentChemical.otherText;
            result.Section5.DateOfTreatment = value.DateOfTreatment;
            result.Section5.Duration = value.Duration;
            result.Section5.Temperature = value.Temperature;
            result.Section5.AdditionalInformation = value.AdditionalInformation;

            // section 6
            result.Section6.AdditionalDeclarations = value.AdditionalDeclarations;
            result.Section6.AdditionalDeclarationsNotRequired = value.AdditionalDeclarationsNotRequired.HasValue && value.AdditionalDeclarationsNotRequired.Value;

            // section 7
            result.Section7.CertificateDeliveryAddress = new Models.Application.Address
            {
                ContactName = value.CertificateDeliveryAddressContactName,
                Line1 = value.CertificateDeliveryAddressLine1,
                Line2 = value.CertificateDeliveryAddressLine2,
                Line3 = value.CertificateDeliveryAddressLine3,
                Line4 = value.CertificateDeliveryAddressLine4,
                PostalCode = value.CertificateDeliveryAddressPostalCode
            };
            result.Section7.CustomerPurchaseOrderNumber = value.CustomerPurchaseOrderNumber;
            result.Section7.CustomerCreditNumber = value.CustomerCreditNumber;
            result.Section7.PdfCopyRequested = value.PdfCopyRequested.HasValue && value.PdfCopyRequested.Value;

            result.Confirmation.AcceptTermsAndConditions =
                value.AcceptTermsAndConditions.HasValue && value.AcceptTermsAndConditions.Value;

            result.SupportingDocumentsSection.SupportingDocumentationNotRequired =
                value.SupportingDocumentationNotRequired.HasValue && value.SupportingDocumentationNotRequired.Value;

            return result;
        }

        public static Models.Application.SupportingDocument ToSupportingDocument(this Models.Repository.Annotation value)
        {
            var result = new SupportingDocument();

            result.CreationDate = value.CreatedOn;
            result.Identifier = value.Identifier;
            result.Length = value.FileSize;
            result.MimeType = value.MimeType;
            result.Name = value.FileName;

            return result;
        }

        /// <summary>
        /// Maps from one type of enum to another.
        /// </summary>
        /// <remarks>
        /// To be used where both enum types have the exact same _text_ values, but different _integer_ values.
        /// </remarks>
        /// <typeparam name="T">The Enum type we are mapping To.</typeparam>
        /// <param name="value">The value to map.</param>
        /// <returns>The mapped enum value in the To type.</returns>
        private static T MapEnum<T>(Enum value) where T: Enum
        {
            T result = (T) Enum.Parse(typeof(T), value.ToString());
            return result;
        }

        private static string TruncateString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;

            value = value.Trim();
            return value.Substring(0, Math.Min(value.Length, maxLength));
        }
    }
}