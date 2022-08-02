using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CSharpFunctionalExtensions;
using Forestry.Eapc.External.Web.Configuration;
using Forestry.Eapc.External.Web.Models.Application;
using Forestry.Eapc.External.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Services
{
    public class StoreSupportingDocumentsUseCaseTests
    {
        private readonly Fixture Fixture = new Fixture();
        private Mock<ISupportingDocumentRepository> MockRepository;

        [Fact]
        public async Task WithNoFilesShouldJustReturnDocumentsListFromApplication()
        {
            var application = CreateTestApplication();
            var sut = CreateSut();

            var result = await sut.StoreSupportingDocuments(application, UserFactory.CreateExternalUser(), new FormFileCollection(), CancellationToken.None);

            Assert.True(result.IsSuccess);
            MockRepository.VerifyAll();  // should not have attempted to send to repository
        }

        [Fact]
        public async Task ShouldUploadGivenFiles()
        {
            var application = CreateTestApplication();
            var uploadedFiles = CreateUploadedFiles();
            var sut = CreateSut();

            var docId = Fixture.Create<string>();
            MockRepository
                .Setup(x => x.StoreSupportingDocumentContentAsync(It.IsAny<string>(), It.IsAny<ExternalUser>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(docId));

            var result = await sut.StoreSupportingDocuments(application, UserFactory.CreateExternalUser(), uploadedFiles, CancellationToken.None);

            Assert.True(result.IsSuccess);
            MockRepository.VerifyAll();
        }

        [Fact]
        public async Task ShouldReturnFailureWhenTooManyFilesProvided()
        {
            var application = CreateTestApplication(9);
            var uploadedFiles = CreateUploadedFiles(3);
            var sut = CreateSut();

            var result = await sut.StoreSupportingDocuments(application, UserFactory.CreateExternalUser(), uploadedFiles, CancellationToken.None);

            Assert.True(result.IsFailure);
            MockRepository.VerifyAll();
        }

        [Fact]
        public async Task ShouldReturnFailureWhenFileIsTooBig()
        {
            var application = CreateTestApplication();
            var uploadedFiles = CreateUploadedFiles(1);
            var sut = CreateSut(1);

            var result = await sut.StoreSupportingDocuments(application, UserFactory.CreateExternalUser(), uploadedFiles, CancellationToken.None);

            Assert.True(result.IsFailure);
            MockRepository.VerifyAll();
        }

        [Fact]
        public async Task ShouldReturnFailureWhenUploadToRepositoryFails()
        {
            var application = CreateTestApplication();
            var uploadedFiles = CreateUploadedFiles(1);
            var sut = CreateSut();

            MockRepository
                .Setup(x => x.StoreSupportingDocumentContentAsync(It.IsAny<string>(), It.IsAny<ExternalUser>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<string>("error"));

            var result = await sut.StoreSupportingDocuments(application, UserFactory.CreateExternalUser(), uploadedFiles, CancellationToken.None);

            Assert.True(result.IsFailure);
            MockRepository.VerifyAll();
        }

        [Fact]
        public async Task ShouldReturnFailureWhenInvalidFileTypesIncluded()
        {
            var application = CreateTestApplication();
            var uploadedFiles = CreateUploadedFiles(3, "doc", "exe");
            var sut = CreateSut();

            var result = await sut.StoreSupportingDocuments(application, UserFactory.CreateExternalUser(), uploadedFiles, CancellationToken.None);

            Assert.True(result.IsFailure);
            MockRepository.VerifyAll();
        }

        private IFormFileCollection CreateUploadedFiles(int numberOfFiles = 3, params string[] fileTypes)
        {
            if (fileTypes.Length == 0)
                fileTypes = new[] { "doc" };

            const string content = "this is an uploaded files";
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            var result = new FormFileCollection();
            for (int i = 0; i < numberOfFiles; i++)
            {
                var name = Fixture.Create<string>();
                var extension = fileTypes[i % fileTypes.Length];
                var file = new FormFile(
                    new MemoryStream(bytes),
                    0,
                    bytes.Length,
                    name,
                    $"{name}.{extension}")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/msword"
                };
                result.Add(file);
            }

            return result;
        }

        private Application CreateTestApplication(int numberOfExistingDocuments = 3)
        {
            var application = Fixture.Create<Application>();
            application.SupportingDocumentsSection.SupportingDocuments = Fixture.CreateMany<SupportingDocument>(numberOfExistingDocuments).ToList();
            return application;
        }

        private StoreSupportingDocumentsUseCase CreateSut(int? maxFileSize = null)
        {
            var settings = new SupportingDocumentsSettings();
            if (maxFileSize.HasValue)
                settings.MaxFileSizeBytes = maxFileSize.Value;

            MockRepository = new Mock<ISupportingDocumentRepository>();
            return new StoreSupportingDocumentsUseCase(
                MockRepository.Object,
                new OptionsWrapper<SupportingDocumentsSettings>(settings),
                new NullLogger<StoreSupportingDocumentsUseCase>());
        }
    }
}