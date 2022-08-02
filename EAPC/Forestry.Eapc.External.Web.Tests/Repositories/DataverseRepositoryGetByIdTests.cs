using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Forestry.Eapc.External.Web.Configuration;
using Forestry.Eapc.External.Web.Models.Repository;
using Forestry.Eapc.External.Web.Services.Repositories.DataVerse;
using Forestry.Eapc.External.Web.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Repositories
{
    public class DataverseRepositoryGetByIdTests
    {
        private Mock<HttpMessageHandler> MockMessageHandler;
        private PowerappsAuthenticationSettings Configuration;
        private readonly IFixture Fixture = new Fixture();

        [Fact]
        public async Task ThrowsIfNoIdProvided()
        {
            var sut = GetSut();

            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await sut.GetByIdAsync(null, UserFactory.CreateExternalUser(), CancellationToken.None));
        }

        [Fact]
        public async Task WithApplicationNotFoundById()
        {
            var sut = GetSut();

            const string id = "0e58cf19-60c8-eb11-bacc-000d3a86e357";
            var expectedRequestUri = $"{Configuration.ApiUrl}api/data/v9.1/{CertificateApplication.EntityName}({id})";
            var responseContent = new
            {
                error = new
                {
                    code = "0x80040217",
                    message = "cr671_certificateapplication With Id = 0e58cf19-60c8-eb11-bacc-000d3a86e357 Does Not Exist"
                }
            };

            MockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent(JsonSerializer.Serialize(responseContent)),
                });

            var result = await sut.GetByIdAsync(id, UserFactory.CreateExternalUser(), CancellationToken.None);

            Assert.Null(result);

            MockMessageHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Get && x.RequestUri.AbsoluteUri == expectedRequestUri), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task WithApplicationFoundById()
        {
            var sut = GetSut();

            const string id = "0e58cf19-60c8-eb11-bacc-000d3a86e357";
            var expectedRequestUri = $"{Configuration.ApiUrl}api/data/v9.1/{CertificateApplication.EntityName}({id})";

            var responseContent = Fixture.Create<CertificateApplication>();

            var docsResponseContent = new QueryMultipleResponseModel<Annotation>
            {
                Values = new Annotation[0]
            };

            MockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri.AbsoluteUri.Contains(CertificateApplication.EntityName)), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(responseContent)),
                });

            MockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri.AbsoluteUri.Contains(CertificateApplication.EntityName) == false), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(docsResponseContent)),
                });

            var result = await sut.GetByIdAsync(id, UserFactory.CreateExternalUser(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(responseContent.Identifier, result.Identifier);
            Assert.Equal(responseContent.ReferenceIdentifier, result.ReferenceIdentifier);
            Assert.Empty(result.SupportingDocumentsSection.SupportingDocuments);

            MockMessageHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Get && x.RequestUri.AbsoluteUri == expectedRequestUri), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task WithApplicationFoundByIdAndHasSupportingDocument()
        {
            var sut = GetSut();

            const string id = "0e58cf19-60c8-eb11-bacc-000d3a86e357";
            var expectedRequestUri = $"{Configuration.ApiUrl}api/data/v9.1/{CertificateApplication.EntityName}({id})";

            var responseContent = Fixture.Create<CertificateApplication>();

            var docsResponseContent = new QueryMultipleResponseModel<Annotation>
            {
                Values = new[] { Fixture.Create<Annotation>() }
            };

            MockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri.AbsoluteUri.Contains(CertificateApplication.EntityName)), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(responseContent)),
                });

            MockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri.AbsoluteUri.Contains(CertificateApplication.EntityName) == false), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(docsResponseContent)),
                });

            var result = await sut.GetByIdAsync(id, UserFactory.CreateExternalUser(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(responseContent.Identifier, result.Identifier);
            Assert.Equal(responseContent.ReferenceIdentifier, result.ReferenceIdentifier);
            Assert.Single(result.SupportingDocumentsSection.SupportingDocuments);
            Assert.Equal(docsResponseContent.Values[0].Identifier, result.SupportingDocumentsSection.SupportingDocuments.Single().Identifier);

            MockMessageHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Get && x.RequestUri.AbsoluteUri == expectedRequestUri), ItExpr.IsAny<CancellationToken>());
        }

        private DataverseRepository GetSut()
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            MockMessageHandler = new Mock<HttpMessageHandler>();
            var client = new HttpClient(MockMessageHandler.Object);
            mockFactory.Setup(x => x.CreateClient("Dataverse")).Returns(client);

            var mockAuthentication = new Mock<ConfidentialClientApplicationAuthentication>(
                new OptionsWrapper<EapcEnvironmentSettings>(
                    new EapcEnvironmentSettings { PowerappsAuthentication = new() }),
                new NullLogger<ConfidentialClientApplicationAuthentication>());
            mockAuthentication.Setup(x => x.AuthenticateAsync())
                .ReturnsAsync(new FakeAuthenticationResult(Fixture.Create<string>()));

            Configuration = Fixture.Create<PowerappsAuthenticationSettings>();
            Configuration.ApiUrl = "https://orgc832ced4.crm11.dynamics.com/";
            var environment = new EapcEnvironmentSettings {PowerappsAuthentication = Configuration};

            return new DataverseRepository(
                mockFactory.Object,
                mockAuthentication.Object,
                Options.Create(environment),
                new NullLogger<DataverseRepository>());
        }
    }
}