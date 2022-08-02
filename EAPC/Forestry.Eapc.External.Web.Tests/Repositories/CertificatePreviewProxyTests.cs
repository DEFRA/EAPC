using System;
using System.Collections.Generic;
using Forestry.Eapc.External.Web.Configuration;
using Forestry.Eapc.External.Web.Models.Application;
using Forestry.Eapc.External.Web.Services.Certificate;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Repositories
{
    public class ModelFactoryTests
    {
        private readonly PhytoCertificatePreviewSettings _settings = new();
        private readonly CertificateGenerationParameters _defaultParameters = new ();

        [Fact]
        public void UsesNonDefaultParametersWhenProvided()
        {
            var parameters = new CertificateGenerationParameters {SignatureName = "Bob", Watermark = "Foo"};
            var result = ModelFactory.CreateRequestModel(CreateApplication(), parameters, _settings);
            Assert.Equal("Bob", result.signatureName);
            Assert.Equal("Foo", result.watermarkText);
        }

        [Theory]
        [InlineData("Test Exporter", null, null, "\r\n\t", " ", null, "TEST EXPORTER")]
        [InlineData("Test Exporter", " Address Line 1", null, "\r\n\t", " ", "BS11 1AB ", "TEST EXPORTER\nADDRESS LINE 1\nBS11 1AB")]
        [InlineData("Test Exporter", "Address Line 1", "Address Line 2", "Address Line 3", "Address Line 4", "BS11 1AB ", "TEST EXPORTER\nADDRESS LINE 1\nADDRESS LINE 2\nADDRESS LINE 3\nADDRESS LINE 4\nBS11 1AB")]
        public void MapsApplicationDataToNameAndAddressOfExporter(
            string exporterName,
            string exporterAddressLine1,
            string exporterAddressLine2,
            string exporterAddressLine3,
            string exporterAddressLine4,
            string exporterAddressPostcode,
            string expectedValue)
        {
            var application = CreateApplication(x =>
            {
                x.Section1.ExporterName = exporterName;
                x.Section1.ExporterAddress.Line1 = exporterAddressLine1;
                x.Section1.ExporterAddress.Line2 = exporterAddressLine2;
                x.Section1.ExporterAddress.Line3 = exporterAddressLine3;
                x.Section1.ExporterAddress.Line4 = exporterAddressLine4;
                x.Section1.ExporterAddress.PostalCode = exporterAddressPostcode;
            });
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal(expectedValue, result.nameAndAddressOfExporter);
        }

        [Fact]
        public void MapsApplicationDataToCertificateReference()
        {
            var application = CreateApplication(x => x.ReferenceIdentifier = "UK/GB/2021/102");
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("No UK/GB/2021/102", result.certificateNumber);
        }

        [Theory]
        [InlineData("Test Consignee", null, null, "\r\n\t", " ", null, "TEST CONSIGNEE")]
        [InlineData(null, " Address Line 1", null, "\r\n\t", " ", "ADDRESS LINE 5 ", "ADDRESS LINE 1\nADDRESS LINE 5")]
        [InlineData("Test Consignee", " Address Line 1", null, "\r\n\t", " ", "ADDRESS LINE 5 ", "TEST CONSIGNEE\nADDRESS LINE 1\nADDRESS LINE 5")]
        [InlineData("Test Consignee", "Address Line 1", "Address Line 2", "Address Line 3", "Address Line 4", "Address Line 5 ", "TEST CONSIGNEE\nADDRESS LINE 1\nADDRESS LINE 2\nADDRESS LINE 3\nADDRESS LINE 4\nADDRESS LINE 5")]
        public void MapsApplicationDataToNameAndAddressOfConsignee(
            string consigneeName,
            string consigneeAddressLine1,
            string consigneeAddressLine2,
            string consigneeAddressLine3,
            string consigneeAddressLine4,
            string consigneeAddressLine5,
            string expectedValue)
        {
            var application = CreateApplication(x =>
            {
                x.Section3.NameOfConsignee = consigneeName;
                x.Section3.AddressOfConsignee.Line1 = consigneeAddressLine1;
                x.Section3.AddressOfConsignee.Line2 = consigneeAddressLine2;
                x.Section3.AddressOfConsignee.Line3 = consigneeAddressLine3;
                x.Section3.AddressOfConsignee.Line4 = consigneeAddressLine4;
                x.Section3.AddressOfConsignee.Line5 = consigneeAddressLine5;
            });
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal(expectedValue, result.declaredNameAndAddressOfConsignee);
        }

        [Theory]
        [InlineData("Line 1", "Line 5", "France", "LINE 1\nLINE 5\nFRANCE")]
        [InlineData("Line 1", "Line 5 FRANCE", "France", "LINE 1\nLINE 5 FRANCE")]
        [InlineData("Line 1", "FRANCE", "France", "LINE 1\nFRANCE")]
        [InlineData("Line 1", "Province of France", "France", "LINE 1\nPROVINCE OF FRANCE")]
        [InlineData("Line 1", "Province of Frances", "France", "LINE 1\nPROVINCE OF FRANCES\nFRANCE")]
        [InlineData("Line 1, France", "", "France", "LINE 1, FRANCE")]
        public void CountryOfDestinationShouldBeAppendedToAddressOfConsigneeIfLastAddressSegmentDoesNotEndWithCountryOfDestination(
                string addressLine1,
                string addressLine5,
                string countryOfDestination,
                string expectedValue)
        {
            var application = CreateApplication(x =>
            {
                x.Section3.AddressOfConsignee.Line1 = addressLine1;
                x.Section3.AddressOfConsignee.Line5 = addressLine5;
                x.Section3.CountryOfDestination = countryOfDestination;
            });
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal(expectedValue, result.declaredNameAndAddressOfConsignee);
        }

        [Theory]
        [InlineData(null, null, "")]
        [InlineData(TransportType.AirFreight, null, "AIR FREIGHT")]
        [InlineData(TransportType.SeaFreight, "", "SEA FREIGHT")]
        [InlineData(TransportType.Road, "foo", "ROAD")]
        [InlineData(TransportType.Other, "Helicopter", "HELICOPTER")]
        [InlineData(TransportType.Other, null, "")]
        [InlineData(TransportType.Other, "", "")]
        public void MapsApplicationDataToDeclaredMeansOfConveyance(TransportType? transportType, string otherText, string expectedValue)
        {
            var application = CreateApplication(x =>
            {
                x.Section4.MeansOfConveyance = transportType;
                x.Section4.MeansOfConveyanceOtherText = otherText;
            });
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal(expectedValue, result.declaredMeansOfConveyance);
        }

        [Fact]
        public void MapsApplicationDataToPlantProtectionOrganisationOf()
        {
            var application = CreateApplication(x => x.Section3.CountryOfDestination = "United States");
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("UNITED STATES", result.toPlantProtectionOrganisation);
        }

        [Fact]
        public void MapsApplicationDataToPlaceOfOrigin()
        {
            var application = CreateApplication(x => x.Section4.WhereGrowns = new [] {"Where Grown Value"});
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("WHERE GROWN VALUE", result.placeOfOrigin);

            application.Section4.WhereGrowns = new[] {"Value 1", "Value 2", "Value 3"};
            result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("VALUE 1, VALUE 2, VALUE 3", result.placeOfOrigin);
        }

        [Fact]
        public void MapsApplicationDataToDeclaredPointOfEntry()
        {
            var application = CreateApplication(x => x.Section3.PortOfImport = "Import Port");
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("IMPORT PORT", result.declaredPointOfEntry);
        }

        [Fact]
        public void MapsApplicationDataToQuantityDeclared()
        {
            var application = CreateApplication(x =>
            {
                x.Section4.Quantity.Add(new Quantity {Amount = 5, Unit = QuantityUnit.KG});
                x.Section4.Quantity.Add(new Quantity {Amount = 2, Unit = QuantityUnit.M3});
                x.Section4.Quantity.Add(new Quantity {Amount = 3, Unit = QuantityUnit.Other, OtherText = " Packages "});
            });
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("5 KG\n2 CUBIC METRES\n3 PACKAGES", result.quantityDeclared);
        }

        [Fact]
        public void MapsApplicationDataToDescriptionOfPackagesEtc()
        {
            var application = CreateApplication(x =>
            {
                x.Section4.DescriptionOfProducts = "Some Description Here";
                x.Section4.BotanicalNames = Array.Empty<string>();
            });
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("Some Description Here", result.description);

            application.Section4.BotanicalNames = new [] {"Woodus Lovlius"};
            result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("Some Description Here\n\nWoodus Lovlius", result.description);

            application.Section4.BotanicalNames = new [] {"Woodus Lovlius", "Loggus Minimus"};
            result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("Some Description Here\n\nWoodus Lovlius, Loggus Minimus", result.description);

            application.Section4.DescriptionOfProducts = null;
            result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("Woodus Lovlius, Loggus Minimus", result.description);
        }

        [Fact]
        public void MapsApplicationDataToAdditionalDeclaration()
        {
            var application = CreateApplication(x => x.Section6.AdditionalDeclarations = " Something line 1 \r\n\r\n Something line 2 \r\n");
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("Something line 1\nSomething line 2", result.additionalDeclaration);
        }

        [Fact]
        public void MapsApplicationDataToTreatment()
        {
            var application = CreateApplication(x => x.Section5.Treatment = TreatmentType.HeatTreated);
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("HEAT TREATED", result.disinfectionTreatment);
        }

        [Fact]
        public void MapsApplicationDataToChemicalActiveIngredient()
        {
            var application = CreateApplication(x =>
            {
                x.Section5.Chemical = TreatmentChemical.Other;
                x.Section5.ChemicalOtherText = "Water";
            });
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("WATER", result.disinfectionChemical);

            application.Section5.Chemical = TreatmentChemical.MethylBromide;
            result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("METHYL BROMIDE", result.disinfectionChemical);

            application.Section5.Chemical = TreatmentChemical.Phosphine;
            result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("PHOSPHINE", result.disinfectionChemical);

            application.Section5.Chemical = TreatmentChemical.SulfurylFluoride;
            result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("SULFURYL FLUORIDE", result.disinfectionChemical);

            application.Section5.Chemical = TreatmentChemical.None;
            result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("NONE", result.disinfectionChemical);
        }

        [Fact]
        public void MapsApplicationDataToDurationAndTemperature()
        {

        }

        [Fact]
        public void MapsApplicationDataToConcentration()
        {
            var application = CreateApplication(x => x.Section5.Concentration = "50 m/l");
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("50 M/L", result.disinfectionConcentration);
        }

        [Theory]
        [InlineData(1, "May 1st 2021")]
        [InlineData(2, "May 2nd 2021")]
        [InlineData(3, "May 3rd 2021")]
        [InlineData(4, "May 4th 2021")]
        [InlineData(5, "May 5th 2021")]
        [InlineData(10, "May 10th 2021")]
        [InlineData(11, "May 11th 2021")]
        [InlineData(20, "May 20th 2021")]
        [InlineData(21, "May 21st 2021")]
        [InlineData(22, "May 22nd 2021")]
        [InlineData(23, "May 23rd 2021")]
        [InlineData(24, "May 24th 2021")]
        [InlineData(25, "May 25th 2021")]
        [InlineData(30, "May 30th 2021")]
        [InlineData(31, "May 31st 2021")]
        public void MapsApplicationDataToTreatmentDate(int inputDay, string expectedDateOutput)
        {
            var application = CreateApplication(x => x.Section5.DateOfTreatment = new DateTime(2021, 5, inputDay));
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal(expectedDateOutput, result.disinfectionDate);
        }

        [Fact]
        public void MapsApplicationDataToAdditionalInformation()
        {
            var application = CreateApplication(x => x.Section5.AdditionalInformation = " additional information ");
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal("additional information", result.disInfectionAdditionalInfo);
        }

        [Fact]
        public void MapsConfigurationDataToPlaceOfIssue()
        {
            var application = CreateApplication();
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal(_settings.DefaultPlaceOfIssue.ToUpperInvariant(), result.placeOfIssue);
        }

        [Fact]
        public void MapsConfigurationDataToSignatureName()
        {
            var application = CreateApplication();
            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);
            Assert.Equal(_settings.DefaultSignatureName, result.signatureName);
        }

        /// <summary>
        /// Our PDF generation service falls over if any string value has "\r", "\t", "\b", or "\f".
        /// As such these characters are removed or replaced on every string value before being serialized to JSON.
        /// </summary>
        [Fact]
        public void ReplacesSpecialCharactersThatOtherwiseBreakThePdfGenerationService()
        {
            /* replacement will happen as follows

              \r = ""
              \t = "  " (two spaces)
              \b = ""
              \f = ""
            */
            const string testFaultString = "I\rwill\tbreak \b pdf\f";

            var application = CreateApplication(x =>
            {
                x.Applicant.ProfessionalOperatorNumber = testFaultString;
                x.Applicant.Email = testFaultString;
                x.Applicant.Telephone = testFaultString;
                x.Applicant.CompanyName = testFaultString;
                x.Applicant.PersonName = testFaultString;

                x.Section1.ExporterAddress.ContactName = testFaultString;
                x.Section1.ExporterAddress.Line1 = testFaultString;
                x.Section1.ExporterAddress.Line2 = testFaultString;
                x.Section1.ExporterAddress.Line3 = testFaultString;
                x.Section1.ExporterAddress.Line4 = testFaultString;
                x.Section1.ExporterAddress.Line5 = testFaultString;
                x.Section1.ExporterAddress.PostalCode = testFaultString;
                x.Section1.ExporterName = testFaultString;

                x.Section2.GoodsInspectionAddress.ContactName = testFaultString;
                x.Section2.GoodsInspectionAddress.Line1 = testFaultString;
                x.Section2.GoodsInspectionAddress.Line2 = testFaultString;
                x.Section2.GoodsInspectionAddress.Line3 = testFaultString;
                x.Section2.GoodsInspectionAddress.Line4 = testFaultString;
                x.Section2.GoodsInspectionAddress.Line5 = testFaultString;
                x.Section2.GoodsInspectionAddress.PostalCode = testFaultString;

                x.Section4.CommodityType = testFaultString;
                x.Section3.AddressOfConsignee.ContactName = testFaultString;
                x.Section3.AddressOfConsignee.Line1 = testFaultString;
                x.Section3.AddressOfConsignee.Line2 = testFaultString;
                x.Section3.AddressOfConsignee.Line3 = testFaultString;
                x.Section3.AddressOfConsignee.Line4 = testFaultString;
                x.Section3.AddressOfConsignee.Line5 = testFaultString;
                x.Section3.AddressOfConsignee.PostalCode = testFaultString;
                x.Section4.BotanicalNames = new [] {testFaultString};
                x.Section4.CertificateNumbersFromCountryOfOrigin = testFaultString;
                x.Section3.CountryOfDestination = testFaultString;
                x.Section4.DescriptionOfProducts = testFaultString;
                x.Section4.MeansOfConveyanceOtherText = testFaultString;
                x.Section4.MeansOfConveyance = TransportType.Other;
                x.Section3.NameOfConsignee = testFaultString;
                x.Section3.PortOfExport = testFaultString;
                x.Section3.PortOfImport = testFaultString;
                x.Section4.WhereGrowns = new []{testFaultString};
                x.Section4.Quantity = new List<Quantity> {new() {OtherText = testFaultString, Amount = 1, Unit = QuantityUnit.Other}};

                x.Section5.AdditionalInformation = testFaultString;
                x.Section5.Chemical = TreatmentChemical.Other;
                x.Section5.ChemicalOtherText = testFaultString;
                x.Section5.TreatmentOtherText = testFaultString;
                x.Section5.Treatment = TreatmentType.Other;
                x.Section5.Concentration = testFaultString;

                x.Section6.AdditionalDeclarations = testFaultString;

                x.Section7.CertificateDeliveryAddress.ContactName = testFaultString;
                x.Section7.CertificateDeliveryAddress.Line1 = testFaultString;
                x.Section7.CertificateDeliveryAddress.Line2 = testFaultString;
                x.Section7.CertificateDeliveryAddress.Line3 = testFaultString;
                x.Section7.CertificateDeliveryAddress.Line4 = testFaultString;
                x.Section7.CertificateDeliveryAddress.Line5 = testFaultString;
                x.Section7.CertificateDeliveryAddress.PostalCode = testFaultString;
                x.Section7.CustomerCreditNumber = testFaultString;
                x.Section7.CustomerPurchaseOrderNumber = testFaultString;
            });

            var result = ModelFactory.CreateRequestModel(application, _defaultParameters, _settings);

            Assert.Equal("I\nwill  break  pdf", result.additionalDeclaration);
            Assert.Equal("IWILL  BREAK  PDF", result.declaredMeansOfConveyance);
            Assert.Equal("IWILL  BREAK  PDF\nIWILL  BREAK  PDF\nIWILL  BREAK  PDF\nIWILL  BREAK  PDF\nIWILL  BREAK  PDF\nIWILL  BREAK  PDF\nIWILL  BREAK  PDF", result.declaredNameAndAddressOfConsignee);
            Assert.Equal("IWILL  BREAK  PDF", result.declaredPointOfEntry);
            Assert.Equal("Iwill  break  pdf\n\nIwill  break  pdf", result.description);
            Assert.Equal("Iwill  break  pdf", result.disInfectionAdditionalInfo);
            Assert.Equal("IWILL  BREAK  PDF", result.disinfectionChemical);
            Assert.Equal("IWILL  BREAK  PDF", result.disinfectionConcentration);
            Assert.Equal("IWILL  BREAK  PDF\nIWILL  BREAK  PDF\nIWILL  BREAK  PDF\nIWILL  BREAK  PDF\nIWILL  BREAK  PDF\nIWILL  BREAK  PDF", result.nameAndAddressOfExporter);
            Assert.Equal("IWILL  BREAK  PDF", result.placeOfOrigin);
            Assert.Equal("1 IWILL  BREAK  PDF", result.quantityDeclared);

        }

        private static Application CreateApplication(Action<Application>? callback = null)
        {
            var result = new Application();
            callback?.Invoke(result);
            return result;
        }
    }
}
