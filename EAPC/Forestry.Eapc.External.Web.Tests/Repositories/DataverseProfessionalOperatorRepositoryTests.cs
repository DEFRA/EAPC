using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Forestry.Eapc.External.Web.Configuration;
using Forestry.Eapc.External.Web.Services.Repositories.ProfessionalOperator;
using Forestry.Eapc.External.Web.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Repositories
{
    public class DataverseProfessionalOperatorRepositoryTests
    {
        private Mock<HttpMessageHandler> MockMessageHandler;
        private ProfessionalOperatorRegistrationEnvironmentSettings Configuration;
        private readonly IFixture Fixture = new Fixture();

        [Fact]
        public async Task ReturnsNullIfNoExternalUserProvided()
        {
            var sut = GetSut();

            var result = await sut.GetAsync(null, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task ReturnsNullIfExternalUserHasNoOperatorNumber()
        {
            var sut = GetSut();
            var user = UserFactory.CreateExternalUserWithoutOperatorNumber();

            var result = await sut.GetAsync(user, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task SendsRequestToExpectedUrl()
        {
            var sut = GetSut();
            var user = UserFactory.CreateExternalUser();

            var filter = $"$filter=endswith({Configuration.ProfessionalOperatorNumberField},'{user.ProfessionalOperatorNumber}')";
            var select = $"$select={Configuration.ProfessionalOperatorNumberField}";
            var expand = $"$expand={Configuration.OperatorLookupField}($select={Configuration.OperatorEmailField})";

            var expectedRequestUri = $"{Configuration.PowerappsAuthentication.ApiUrl}api/data/v9.1/{Configuration.OperatorRegistrationTable}s?{filter}&{select}&{expand}";
            
            var responseContent =
                "{\"@odata.context\": \"https://orgc832ced4.crm11.dynamics.com/api/data/v9.1/$metadata#cr671_operatorregistrations(cr671_registrationnumber,cr671_Operator(cr671_emailaddress1))\",\"value\":[]}";

            MockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                });

            var result = await sut.GetAsync(user, CancellationToken.None);

            Assert.Null(result);

            MockMessageHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Get && x.RequestUri.AbsoluteUri == expectedRequestUri), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task ReturnsNullWhenNoValueReturnedInResponse()
        {
            var sut = GetSut();
            var user = UserFactory.CreateExternalUser();

            var responseContent =
                "{\"@odata.context\": \"https://orgc832ced4.crm11.dynamics.com/api/data/v9.1/$metadata#cr671_operatorregistrations(cr671_registrationnumber,cr671_Operator(cr671_emailaddress1))\",\"value\":[]}";

            MockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                });

            var result = await sut.GetAsync(user, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task ReturnsNullWhenUnexpectedResponseReceived()
        {
            var sut = GetSut();
            var user = UserFactory.CreateExternalUser();

            var responseContent = new
            {
                error = new
                {
                    code = "0x80040217",
                    message = "this is not the content we are expecting"
                }
            };

            MockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(responseContent)),
                });

            var result = await sut.GetAsync(user, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task ReturnsNullWhenNoContactEmailFoundForOperatorRegistration()
        {
            var sut = GetSut();
            var user = UserFactory.CreateExternalUser();

            var responseContent =
                "{\"@odata.context\": \"https://orgc832ced4.crm11.dynamics.com/api/data/v9.1/$metadata#cr671_operatorregistrations(cr671_registrationnumber,cr671_Operator(cr671_emailaddress1))\",\"value\":[{\"@odata.etag\":\"W/\\\"3380092\\\"\",\"cr671_registrationnumber\":\"FC-123456\",\"cr671_operatorregistrationid\":\"12e2d697-2221-ec11-b6e6-000d3a0cd039\",\"" + Configuration.OperatorLookupField + "\":{}}]}";

            MockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                });

            var result = await sut.GetAsync(user, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task ReturnsProfessionalOperatorEmailWhenFound()
        {
            var sut = GetSut();
            var user = UserFactory.CreateExternalUser();
            const string expectedEmail = "matt@qxlva.com";

            var responseContent =
                "{\"@odata.context\": \"https://orgc832ced4.crm11.dynamics.com/api/data/v9.1/$metadata#cr671_operatorregistrations(cr671_registrationnumber,cr671_Operator(cr671_emailaddress1))\",\"value\":[{\"@odata.etag\":\"W/\\\"3380092\\\"\",\"cr671_registrationnumber\":\"FC-123456\",\"cr671_operatorregistrationid\":\"12e2d697-2221-ec11-b6e6-000d3a0cd039\",\"" + Configuration.OperatorLookupField + "\":{\"" + Configuration.OperatorEmailField + "\":\"matt@qxlva.com\",\"cr671_operatorid\":\"9c73f7a0-2121-ec11-b6e6-000d3a0cd039\"}}]}";

            MockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                });

            var result = await sut.GetAsync(user, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(expectedEmail, result?.KeyContactEmail);
        }

        private DataverseProfessionalOperatorRepository GetSut()
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            MockMessageHandler = new Mock<HttpMessageHandler>();
            var client = new HttpClient(MockMessageHandler.Object);
            mockFactory.Setup(x => x.CreateClient("ProfessionalOperator")).Returns(client);

            var mockAuthentication = new Mock<ProfessionalOperatorClientApplicationAuthentication>(
                new OptionsWrapper<ProfessionalOperatorRegistrationEnvironmentSettings>(
                    new ProfessionalOperatorRegistrationEnvironmentSettings {PowerappsAuthentication = new()}),
                new NullLogger<ProfessionalOperatorClientApplicationAuthentication>());
            mockAuthentication.Setup(x => x.AuthenticateAsync())
                .ReturnsAsync(new FakeAuthenticationResult(Fixture.Create<string>()));

            Configuration = Fixture.Create<ProfessionalOperatorRegistrationEnvironmentSettings>();
            Configuration.PowerappsAuthentication.ApiUrl = "https://orgc832ced4.crm11.dynamics.com/";

            return new DataverseProfessionalOperatorRepository(
                mockFactory.Object,
                mockAuthentication.Object,
                Options.Create(Configuration),
                new NullLogger<DataverseProfessionalOperatorRepository>());
        }
    }
}