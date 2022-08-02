using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Forestry.Eapc.External.Web.Configuration;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services.Certificate
{
    /// <summary>
    /// Maps data from an <see cref="Application"/> to a <see cref="RequestModel"/> for transmission to
    /// the remote PDF generation service.
    /// </summary>
    /// <remarks>
    /// This functionality has been extracted out into a stand-alone class so that various mappings can be unit
    /// tested between the two models independently of any transport concerns.
    /// </remarks>
    public static class ModelFactory
    {
        public static RequestModel CreateRequestModel(
            Application application,
            CertificateGenerationParameters parameters,
            PhytoCertificatePreviewSettings settings)
        {
            if (application == null) throw new ArgumentNullException(nameof(application));
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            var result = new RequestModel
            {
                watermarkText = parameters.Watermark,
                certificateNumber = "No " + application.ReferenceIdentifier?.ToUpperInvariant(),
                placeOfIssue = settings.DefaultPlaceOfIssue.ToUpperInvariant(),
                signatureName = string.IsNullOrWhiteSpace(parameters.SignatureName)
                    ? settings.DefaultSignatureName
                    : parameters.SignatureName,
                signatureDate = CreateFormattedDate(DateTime.Now),
                nameAndAddressOfExporter = ConcatLines(
                    application.Section1.ExporterName,
                    application.Section1.ExporterAddress.Line1,
                    application.Section1.ExporterAddress.Line2,
                    application.Section1.ExporterAddress.Line3,
                    application.Section1.ExporterAddress.Line4,
                    application.Section1.ExporterAddress.PostalCode).ToUpperInvariant(),
                declaredNameAndAddressOfConsignee = BuildConsigneeAddress(application.Section3).ToUpperInvariant(),
                toPlantProtectionOrganisation =
                    ConcatLines(application.Section3.CountryOfDestination).ToUpperInvariant(),
                declaredMeansOfConveyance =
                    ConcatLines(TransportTypeExtensions.CreateCertificateString(application.Section4.MeansOfConveyance,
                        application.Section4.MeansOfConveyanceOtherText)).ToUpperInvariant(),
                additionalDeclaration = ReadMultiLineString(application.Section6.AdditionalDeclarations),
                disInfectionAdditionalInfo = ConcatLines(application.Section5.AdditionalInformation),
                declaredPointOfEntry = ConcatLines(application.Section3.PortOfImport).ToUpperInvariant(),
                disinfectionDate = CreateFormattedDate(application.Section5.DateOfTreatment),
                disinfectionChemical =
                    ConcatLines(TreatmentChemicalExtensions.CreateCertificateString(application.Section5.Chemical,
                        application.Section5.ChemicalOtherText)).ToUpperInvariant(),
                disinfectionConcentration = ConcatLines(application.Section5.Concentration).ToUpperInvariant(),
                disinfectionTreatment =
                    ConcatLines(TreatmentTypeExtensions.CreateCertificateString(application.Section5.Treatment,
                        application.Section5.TreatmentOtherText)).ToUpperInvariant(),
                disinfectionDurationAndTemperature =
                    CreateDurationAndTemperatureString(application.Section5).ToUpperInvariant(),
                placeOfOrigin = CommaJoinLines(application.Section4.WhereGrowns).ToUpperInvariant(),
                description = CreateDescriptionOfProducts(application.Section4),
                quantityDeclared = CreateQuantity(application.Section4).ToUpperInvariant()
            };

            MakeSafeForPdfService(result);
            return result;
        }

        private static string CreateQuantity(Section4 section)
        {
            return ConcatLines(
                section.Quantity.Select(x => x.CreateCertificateString()).ToArray());
        }

        private static string ReadMultiLineString(string? value)
        {
            if (value == null)
                return string.Empty;

            using var reader = new StringReader(value);
            List<string> lines = new();
            var line = reader.ReadLine();
            while (line != null)
            {
                lines.Add(line);
                line = reader.ReadLine();
            }

            return ConcatLines(lines.ToArray());
        }

        private static string BuildConsigneeAddress(Section3 section3)
        {
            var result = ConcatLines(
                section3.NameOfConsignee,
                section3.AddressOfConsignee.Line1,
                section3.AddressOfConsignee.Line2,
                section3.AddressOfConsignee.Line3,
                section3.AddressOfConsignee.Line4,
                section3.AddressOfConsignee.Line5);

            if (!string.IsNullOrWhiteSpace(section3.CountryOfDestination) &&
                !result.EndsWith(section3.CountryOfDestination, StringComparison.InvariantCultureIgnoreCase))
            {
                result = result + "\n" + section3.CountryOfDestination;
            }

            return result;
        }

        private static string CreateDescriptionOfProducts(Section4 section)
        {
            var description = section.DescriptionOfProducts;
            var botanicalNames = CommaJoinLines(section.BotanicalNames);

            var result = description + "\n\n" + botanicalNames;
            return result.Trim();
        }

        private static string CreateDurationAndTemperatureString(Section5 section)
        {
            var result = string.Empty;

            var duration = section.Duration;
            var temperature = section.Temperature;

            if (duration.HasValue)
            {
                result = result + duration.Value + " mins ";
            }

            if (temperature.HasValue)
            {
                result = result + temperature.Value + "C";
            }

            return result.Trim();
        }

        private static string CreateFormattedDate(DateTime? value)
        {
            if (!value.HasValue)
                return string.Empty;

            var month = value.Value.ToString("MMMM");
            var year = value.Value.ToString("yyyy");
            var numericDay = value.Value.Day;

            var suffix = numericDay switch
            {
                1 => "st",
                21 => "st",
                31 => "st",
                2 => "nd",
                22 => "nd",
                3 => "rd",
                23 => "rd",
                _ => "th"
            };

            return $"{month} {numericDay}{suffix} {year}";
        }

        private static string ConcatLines(params string?[] lines)
        {
            return string.Join("\n", lines.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!.Trim()));
        }

        private static string CommaJoinLines(params string?[] lines)
        {
            return string.Join(", ", lines.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!.Trim()));
        }

        private static void MakeSafeForPdfService(RequestModel model)
        {
            var properties = model.GetType().GetProperties();
            foreach (var property in properties)
            {
                property.SetValue(model,
                    ReplaceSpecialCharactersThatBreakThePdfService(property.GetValue(model) as string));
            }

            static string ReplaceSpecialCharactersThatBreakThePdfService(string? value)
            {
                if (value == null)
                    return string.Empty;

                return value
                    .Replace("\r", string.Empty)
                    .Replace("\t", "  ")
                    .Replace("\b", string.Empty)
                    .Replace("\f", string.Empty);
            }
        }
    }
}
