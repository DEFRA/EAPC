using System;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Models.Application;
using Forestry.Eapc.External.Web.Services;
using Forestry.Eapc.External.Web.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Services
{
    public class CreateNewApplicationUseCaseTests
    {
        private static readonly DateTime UtcNow = DateTime.UtcNow;
        private readonly IClock _fixedTimeClock = new FakeClock(Instant.FromDateTimeUtc(UtcNow));

        [Fact]
        public async Task ShouldUseRepositoryToCreateNewApplication()
        {
            var sut = CreateSut();
            var user = UserFactory.CreateExternalUser();
            var application = await sut.CreateAsync(user);
            Assert.True(application.IsSuccess);
            Assert.NotNull(application.Value.Identifier);
        }

        [Fact]
        public async Task CreatedApplicationShouldBeDraft()
        {
            var sut = CreateSut();
            var user = UserFactory.CreateExternalUser();
            var application = await sut.CreateAsync(user);

            Assert.Equal(ApplicationState.Draft, application.Value.State);
        }

        [Fact]
        public async Task CreatedApplicationShouldHaveCreationDate()
        {
            var sut = CreateSut();
            var user = UserFactory.CreateExternalUser();
            var application = await sut.CreateAsync(user);

            Assert.Equal(UtcNow.Date, application.Value.CreationDate);
        }

        [Fact]
        public async Task ShouldPopulateApplicationWithDetailsFromUser()
        {
            var sut = CreateSut();
            var user = UserFactory.CreateExternalUser();
            var application = await sut.CreateAsync(user);

            var applicantModel = application.Value.Applicant;
            Assert.Equal(user.ProfessionalOperatorNumber, applicantModel.ProfessionalOperatorNumber);
            Assert.Equal(user.Telephone, applicantModel.Telephone);
            Assert.Equal(user.Email, applicantModel.Email);
            Assert.Equal(user.CompanyName, applicantModel.CompanyName);
            Assert.Equal(user.FullName, applicantModel.PersonName);
            Assert.Equal(user.CreditAccountReference, application.Value.Section7.CustomerCreditNumber);
        }

        [Fact]
        public async Task ShouldPopulateExportStatusAsReforwardingIsDescoped()
        {
            var sut = CreateSut();
            var user = UserFactory.CreateExternalUser();
            var application = await sut.CreateAsync(user);

            Assert.Equal(ExportStatus.New, application.Value.Applicant.ExportStatus);
        }

        private CreateNewApplicationUseCase CreateSut()
        {
            return new(
                _fixedTimeClock,
                new FakeApplicationRepository(),
                new NullLogger<CreateNewApplicationUseCase>());
        }
    }
}
