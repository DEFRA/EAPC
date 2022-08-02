using System;
using System.Collections.Generic;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Tests
{
    internal static class ApplicationFactory
    {
        internal static Application CreateApplication(Action<Application>? callback = null)
        {
            var result = CreateValidApplication();
            callback?.Invoke(result);
            return result;
        }

        internal static Application CreateValidApplication()
        {
            var result = new Application
            {
                Identifier = Guid.NewGuid().ToString(),
                ReferenceIdentifier = Guid.NewGuid().ToString(),
                CreationDate = DateTime.UtcNow,
                Applicant = new Applicant
                {
                    Email = "foo@qxlva.com",
                    ProfessionalOperatorNumber = "123456",
                    ExportStatus = ExportStatus.New,
                    Telephone = "01249 751096",
                    Region = HomeNation.England,
                    CompanyName = "Test Company & Sons",
                    PersonName = "Tester O'Flannigan"
                },
                Section1 = new Section1
                {
                    ExporterName = "Foo",
                    ExporterAddress = new Address
                    {
                        Line1 = "Line 1",
                        Line2 = "Line 2",
                        Line3 = "Line 3",
                        Line4 = "Line 4",
                        PostalCode = "Line Postcode",
                    }
                },
                Section2 =new Section2
                {
                    InspectionNotRequired = true
                },
                Section3 = new Section3
                {
                    AddressOfConsignee = new Address
                    {
                        Line1 = "Line 1",
                        Line2 = "Line 2",
                        Line3 = "Line 3"
                    },
                    PortOfExport = "Port of Export",
                    PortOfImport = "Port of Import",
                    DateOfExport = DateTime.UtcNow.AddDays(7),
                    CountryOfDestination = "India",
                    NameOfConsignee = "O'Handler Handling",
                },
                Section4 = new Section4
                {
                    CountryOfDestination = "India",
                    MeansOfConveyance = TransportType.AirFreight,
                    WhereGrowns = new []{"Surrey"},
                    DescriptionOfProducts = "Big tables",
                    BotanicalNames = new []{"Wood"},
                    Quantity = new List<Quantity> {new Quantity {Amount = 1, Unit = QuantityUnit.KG}},
                    CommodityType = "Logs"
                },
                Section5 = new Section5
                {
                    Treatment = TreatmentType.HeatTreated,
                    Chemical = TreatmentChemical.MethylBromide
                },
                Section6 = new Section6
                {
                    AdditionalDeclarationsNotRequired = true
                },
                Section7 = new Section7
                {
                    CertificateDeliveryAddress = new Address
                    {
                        ContactName = "Certificate delivery contact name",
                        Line1 = "Line 1",
                        Line2 = "Line 2",
                        Line3 = "Line 3",
                        Line4 = "Line 4",
                        PostalCode = "Line Postcode",
                    },
                    CustomerCreditNumber = "12345"
                },
                SupportingDocumentsSection = new SupportingDocumentsSection
                {
                    SupportingDocumentationNotRequired = true
                }
            };
            return result;
        }
    }
}
