using System;
using System.Collections.Generic;
using System.Linq;
using Forestry.Eapc.External.Web.Models.Application;
using Forestry.Eapc.External.Web.Services;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Services
{
    public class ValidationProviderTests
    {
        [Fact]
        public void ValidationSucceedsForValidApplication()
        {
            var application = CreateValidApplication();
            var results = CreateSut().Validate(application);
            Assert.Empty(results);
        }

        [Fact]
        public void ValidationAssertsDataOnApplicationRoot()
        {
            var sut = CreateSut();
            Assert.NotEmpty(sut.Validate(CreateApplication(x => x.Identifier = null)));
            Assert.NotEmpty(sut.Validate(CreateApplication(x => x.Identifier = "")));
            Assert.NotEmpty(sut.Validate(CreateApplication(x => x.ReferenceIdentifier = null)));
            Assert.NotEmpty(sut.Validate(CreateApplication(x => x.ReferenceIdentifier = "")));
            Assert.NotEmpty(sut.Validate(CreateApplication(x => x.CreationDate = null)));
        }

        [Fact]
        public void ValidationAssertsDataOnApplicant()
        {
            var sut = CreateSut();
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.Email = null)));
            var foo = sut.Validate(CreateApplication(x => x.Applicant.Email = ""));
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.Email = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.Email = "fobar.com")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.CompanyName = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.CompanyName = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.ExportStatus = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.PersonName = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.PersonName = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.ProfessionalOperatorNumber = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.ProfessionalOperatorNumber = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.ProfessionalOperatorNumber = "12345")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.ProfessionalOperatorNumber = "1234567")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.Region = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.Telephone = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.Telephone = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.Telephone = "01249 75109"))); // to short
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.Telephone = "01249 7510966"))); // to long
            Assert.Single(sut.Validate(CreateApplication(x => x.Applicant.Telephone = "01249 75109X"))); // with letter at end
        }

        [Fact]
        public void ValidationAssertsDataOnSection1()
        {
            var sut = CreateSut();
            Assert.Single(sut.Validate(CreateApplication(x => x.Section1.ExporterName = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section1.ExporterName = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section1.ExporterAddress.Line1 = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section1.ExporterAddress.Line1 = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section1.ExporterAddress.Line2 = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section1.ExporterAddress.Line2 = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section1.ExporterAddress.Line3 = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section1.ExporterAddress.Line3 = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section1.ExporterAddress.PostalCode = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section1.ExporterAddress.PostalCode = "")));
        }

        [Fact]
        public void ValidationAssertsDataOnSection2WhenDoesNotRequireAnInspection()
        {
            var doesNotRequireInspectionModel = CreateApplication(x => x.Section2.InspectionNotRequired = true);
            var sut = CreateSut();
            Assert.Empty(sut.Validate(doesNotRequireInspectionModel));
        }

        [Fact]
        public void ValidationAssertsDataOnSection6WhenDeclarationsNotRequired()
        {
            var doesNotRequireAdditionalDeclarationsModel = CreateApplication(x => x.Section6.AdditionalDeclarationsNotRequired= true);
            doesNotRequireAdditionalDeclarationsModel.Section2.InspectionNotRequired = true;

            var sut = CreateSut();
            Assert.Empty(sut.Validate(doesNotRequireAdditionalDeclarationsModel));
        }

        [Fact]
        public void ValidationAssertsDataOnSection2WhenRequiresAnInspection()
        {
            //All address fields missing when requires an inspection
            var requiresInspectionModel = CreateApplication(x => x.Section2.InspectionNotRequired = false);
//            requiresInspectionModel.Section6.AdditionalDeclarationsNotRequired = true;
            var sut = CreateSut();

            Assert.Single(sut.Validate(requiresInspectionModel));

            requiresInspectionModel.Section2.GoodsInspectionAddress.Line1 = "line1";
            Assert.Empty(sut.Validate(requiresInspectionModel));
        }

        [Fact]
        public void ValidationAssertsDataOnSection3()
        {
            var sut = CreateSut();
            Assert.Single(sut.Validate(CreateApplication(x => x.Section3.PortOfExport = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section3.PortOfExport = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section3.PortOfImport = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section3.PortOfImport = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section3.NameOfConsignee = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section3.NameOfConsignee = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section3.AddressOfConsignee.Line1 = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section3.AddressOfConsignee.Line1 = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section3.AddressOfConsignee.Line2 = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section3.AddressOfConsignee.Line2 = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section3.AddressOfConsignee.Line3 = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section3.AddressOfConsignee.Line3 = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section3.CountryOfDestination = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section3.CountryOfDestination = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section3.DateOfExport = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section3.DateOfExport = DateTime.UtcNow.AddDays(-1))));
        }

         [Fact]
        public void ValidationAssertsDataOnSection4()
        {
            var sut = CreateSut();
            
            Assert.Single(sut.Validate(CreateApplication(x => x.Section4.DescriptionOfProducts = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section4.DescriptionOfProducts = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section4.BotanicalNames = Array.Empty<string>())));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section4.WhereGrowns = Array.Empty<string>())));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section4.Quantity = new List<Quantity>(0))));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section4.Quantity[0].Unit = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section4.Quantity[0].Amount = 0)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section4.Quantity[0].Unit = QuantityUnit.Other)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section4.MeansOfConveyance = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section4.MeansOfConveyance = TransportType.Other)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section4.CommodityType = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section4.CommodityType = "")));
        }
        
        [Fact]
        public void ValidationAssertsDataOnSection5()
        {
            var sut = CreateSut();
            Assert.Single(sut.Validate(CreateApplication(x => x.Section5.Treatment = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section5.Treatment = TreatmentType.Other)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section5.Chemical = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section5.Chemical = TreatmentChemical.Other)));

            // the date of treatment must be before date of export
            var section3Errors = sut.Validate(CreateApplication(x =>
            {
                x.Section3.DateOfExport = DateTime.Today;
                x.Section5.DateOfTreatment = x.Section3.DateOfExport.Value.AddDays(1);
            }));

            Assert.Single(section3Errors.Where(x=>x.ErrorMessage.Equals("The date of export should occur after the date of treatment.")));
            Assert.Single(section3Errors.Where(x=>x.ErrorMessage.Equals("The date of treatment should occur before the date of export.")));

            section3Errors = sut.Validate(CreateApplication(x =>
            {
                x.Section3.DateOfExport = DateTime.Today;
                x.Section5.DateOfTreatment = x.Section3.DateOfExport;
            }));

            Assert.Single(section3Errors.Where(x=>x.ErrorMessage.Equals("The date of export should occur after the date of treatment.")));
            Assert.Single(section3Errors.Where(x=>x.ErrorMessage.Equals("The date of treatment should occur before the date of export.")));


            Assert.Empty(sut.Validate(CreateApplication(x =>
            {
                x.Section3.DateOfExport = DateTime.Today;
                x.Section5.DateOfTreatment = x.Section3.DateOfExport.Value.AddDays(-1);
            })));
            
            Assert.Empty(sut.Validate(CreateApplication(x =>
            {
                x.Section3.DateOfExport = DateTime.Today;
                x.Section5.DateOfTreatment = null;
            })));
        }

        [Fact]
        public void ValidationAssertsDataOnSection6WhenRequiresAdditionalDeclarations()
        {
            var requiresModel = CreateApplication(x => x.Section6.AdditionalDeclarationsNotRequired = false);
            var sut = CreateSut();
            var res = sut.Validate(requiresModel);
            Assert.Single(res);
            Assert.Equal("Additional Declaration(s) must be provided unless you have specified they are not required.",res[0].ErrorMessage);
        }

        [Fact]
        public void ValidationAssertsDataOnSection7()
        {
            var sut = CreateSut();
            Assert.Single(sut.Validate(CreateApplication(x => x.Section7.CertificateDeliveryAddress.ContactName = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section7.CertificateDeliveryAddress.ContactName = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section7.CertificateDeliveryAddress.Line1 = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section7.CertificateDeliveryAddress.Line1 = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section7.CertificateDeliveryAddress.Line2 = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section7.CertificateDeliveryAddress.Line2 = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section7.CertificateDeliveryAddress.Line3 = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section7.CertificateDeliveryAddress.Line3 = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section7.CertificateDeliveryAddress.PostalCode = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section7.CertificateDeliveryAddress.PostalCode = "")));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section7.CustomerCreditNumber = null)));
            Assert.Single(sut.Validate(CreateApplication(x => x.Section7.CustomerCreditNumber = "")));
        }

        [Fact]
        public void ValidationAssertsDataOnSupportingDocumentationWhenUserFlags()
        {
            var requiresModel = CreateApplication(x => x.SupportingDocumentsSection.SupportingDocumentationNotRequired = false);
            var sut = CreateSut();
            var res = sut.Validate(requiresModel);
            Assert.Single(res);
            Assert.Equal("Supporting documentation must be provided unless you have specified that it is not required.",res[0].ErrorMessage);
        }

        [Theory]
        [InlineData("01249751096")]
        [InlineData(" 01249751096 ")]
        [InlineData("01249 751096")]
        [InlineData("01249 751 096")]
        [InlineData("+441249751096")]
        [InlineData("+44 1249 751 096")]
        public void TestsWithValidTelephoneNumberCombinations(string value)
        {
            var sut = CreateSut();
            var errors = sut.Validate(CreateApplication(x => x.Applicant.Telephone = value));
            Assert.Empty(errors);
        }

        [Fact]
        public void MissingAllFieldsOnApplicantShouldReturnMultipleValidationErrors()
        {
            var sut = CreateSut();
            var errors = sut.Validate(CreateApplication(x => x.Applicant = new Applicant()));
            Assert.Equal(6, errors.Count);
        }

        [Fact]
        public void MissingAllFieldsOnSection1ShouldReturnMultipleValidationErrors()
        {
            var sut = CreateSut();
            var errors = sut.Validate(CreateApplication(x => x.Section1 = new Section1()));
            Assert.Equal(5, errors.Count);
        }
        
        [Fact]
        public void MissingAllFieldsOnSection3ShouldReturnMultipleValidationErrors()
        {
            var sut = CreateSut();
            var errors = sut.Validate(CreateApplication(x => x.Section3 = new Section3()));
            Assert.Equal(8, errors.Count);
        }

        [Fact]
        public void MissingAllFieldsOnSection4ShouldReturnMultipleValidationErrors()
        {
            var sut = CreateSut();
            var errors = sut.Validate(CreateApplication(x => x.Section4 = new Section4()));
            Assert.Equal(6, errors.Count);
        }

        [Fact]
        public void MissingAllFieldsOnSection5ShouldReturnMultipleValidationErrors()
        {
            var sut = CreateSut();
            var errors = sut.Validate(CreateApplication(x => x.Section5 = new Section5()));
            Assert.Equal(2, errors.Count);
        }

        [Fact]
        public void MissingAllFieldsOnSection6ShouldReturnMultipleValidationErrors()
        {
            var sut = CreateSut();
            var errors = sut.Validate(CreateApplication(x => x.Section6 = new Section6{AdditionalDeclarationsNotRequired = false}));
            Assert.Single(errors);
        }

        [Fact]
        public void MissingAllFieldsOnSection7ShouldReturnMultipleValidationErrors()
        {
            var sut = CreateSut();
            var errors = sut.Validate(CreateApplication(x => x.Section7 = new Section7()));
            Assert.Equal(6, errors.Count);
        }

        [Fact]
        public void MissingAllFieldsOnSection2ShouldReturnMultipleValidationErrors()
        {
            var sut = CreateSut();
            var errors = sut.Validate(CreateApplication(x => x.Section2 = new Section2 { InspectionNotRequired = false}));
            Assert.Single(errors); //Just add-line 1 is mandatory if inspection is required.
        }

        private ValidationProvider CreateSut() => new();

        private static Application CreateValidApplication() => ApplicationFactory.CreateValidApplication();

        private static Application CreateApplication(Action<Application> callback) =>
            ApplicationFactory.CreateApplication(callback);
    }
}
