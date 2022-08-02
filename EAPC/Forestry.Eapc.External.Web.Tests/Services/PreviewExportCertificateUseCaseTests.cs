using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Configuration;
using Forestry.Eapc.External.Web.Models.Application;
using Forestry.Eapc.External.Web.Services;
using Forestry.Eapc.External.Web.Services.Certificate;
using Forestry.Eapc.External.Web.Services.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Services
{
    public class PreviewExportCertificateUseCaseTests
    {
        private const string TestUrlPrefix = "https://localhost:1234/TestEndpoint/";
        private readonly InMemoryApplicationRepository _applicationRepository = new ();
        private readonly FakeHandler _fakeHandler = new ();
        private readonly ExternalUser _user = UserFactory.CreateExternalUser();

        [Theory]
        [InlineData(ExportStatus.New, "generateCertificate")]
        [InlineData(ExportStatus.Reforwarded, "generateReExportCertificate")]
        [InlineData(null, "generateCertificate")]
        public async Task RequestShouldBeSentToCorrectEndpointBasedOnCertificateExportType(ExportStatus? exportStatus, string expectedEndpoint)
        {
            var expectedRequestUrl = TestUrlPrefix + expectedEndpoint;
            var applicationId = await SetupApplicationInRepositoryAsync(exportStatus);
            
            var sut = CreateSut();
            var result = await sut.GetCertificateAsync(applicationId, _user);

            Assert.NotNull(result);
            Assert.Equal(expectedRequestUrl, _fakeHandler.CapturedHttpRequestMessage!.RequestUri!.AbsoluteUri);
        }

        private async Task<string> SetupApplicationInRepositoryAsync(ExportStatus? exportStatus)
        {
            var application = new Application();
            application.Applicant.ExportStatus = exportStatus;
            application.Applicant.ProfessionalOperatorNumber = _user.ProfessionalOperatorNumber;

            var result = await _applicationRepository.UpsertAsync(application, _user);
            return result.Value.Identifier!;
        }

        private PreviewExportCertificateUseCase CreateSut()
        {
            var client = new HttpClient(_fakeHandler) {BaseAddress = new Uri(TestUrlPrefix)};
            
            var options = new PhytoCertificatePreviewSettings
                {DefaultSignatureName = "Test Signature", DefaultPlaceOfIssue = "Swindon"};

            var result = new PreviewExportCertificateUseCase(
                _applicationRepository,
                new CertificateGenerationProxy(client, new OptionsWrapper<PhytoCertificatePreviewSettings>(options)),
                new NullLogger<PreviewExportCertificateUseCase>());

            return result;
        }

        private class FakeHandler : HttpClientHandler
        {
            public HttpRequestMessage? CapturedHttpRequestMessage { get; private set; }

            /// <inheritdoc />
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                CapturedHttpRequestMessage = request;
                var result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(new MemoryStream());
                return Task.FromResult(result);
            }
        }
    }
}
