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
    public class DataverseRepositoryGetAllForUserTests
    {
        private Mock<HttpMessageHandler> MockMessageHandler;
        private PowerappsAuthenticationSettings Configuration;
        private readonly IFixture Fixture = new Fixture();

        [Fact]
        public async Task ThrowsIfNoUserProvided()
        {
            var sut = GetSut();

            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await sut.GetAllForUserAsync(null, false, CancellationToken.None));
        }

        [Fact]
        public async Task WhenNoApplicationsFoundForUser()
        {
            var sut = GetSut();

            var user = UserFactory.CreateExternalUser();
            var expectedRequestUri = $"{Configuration.ApiUrl}api/data/v9.1/{CertificateApplication.EntityName}?$filter=cr671_applicantprofessionaloperatornumber%20eq%20'{user.ProfessionalOperatorNumber}'";
            var responseContent = new QueryMultipleResponseModel<CertificateApplication>
            {
                Values = new CertificateApplication[0]
            };

            MockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(responseContent)),
                });

            var result = await sut.GetAllForUserAsync(user, false, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);

            MockMessageHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Get && x.RequestUri.AbsoluteUri == expectedRequestUri), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task WhenApplicationsFoundForUser()
        {
            var sut = GetSut();

            var user = UserFactory.CreateExternalUser();
            var expectedRequestUri = $"{Configuration.ApiUrl}api/data/v9.1/{CertificateApplication.EntityName}?$filter=cr671_applicantprofessionaloperatornumber%20eq%20'{user.ProfessionalOperatorNumber}'";
            var responseContent = new QueryMultipleResponseModel<CertificateApplication>
            {
                Values = Fixture.CreateMany<CertificateApplication>(2).ToArray()
            };

            MockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(responseContent)),
                });

            var result = await sut.GetAllForUserAsync(user, false, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, a => a.Identifier == responseContent.Values[0].Identifier);
            Assert.Contains(result, a => a.Identifier == responseContent.Values[1].Identifier);

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
                    new EapcEnvironmentSettings {PowerappsAuthentication = new()}),
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