using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Forestry.Eapc.External.Web.Models.Profile;
using Forestry.Eapc.External.Web.Services;
using Forestry.Eapc.External.Web.Services.Repositories;
using Forestry.Eapc.External.Web.Services.Repositories.ProfessionalOperator;
using Forestry.Eapc.External.Web.Services.Repositories.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Services
{
    public class SignUpToUseSystemUseCaseTests
    {
        private static readonly IFixture FixtureInstance = new Fixture();

        private readonly Mock<ILocalUserRepository> _mockUserRepository = new();

        private readonly Mock<IProfessionalOperatorRepository> _mockProfessionalOperatorRepository = new();

        [Fact]
        public async Task ErrorCheckingIfKeyContact()
        {
            var externalUser = UserFactory.CreateExternalUser();
            var cancellationToken = CancellationToken.None;
            
            _mockProfessionalOperatorRepository
                .Setup(x => x.GetAsync(externalUser, cancellationToken))
                .ThrowsAsync(new RepositoryException("test exception"));

            var sut = CreateSut();
            var result = await sut.ApplyProfileAsync(externalUser, new UserProfileModel(), cancellationToken);
            Assert.Equal(ApplyProfileOutcome.OperationFailed, result);

            _mockProfessionalOperatorRepository.Verify();
            _mockUserRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ProfessionalOperatorNotFound()
        {
            var externalUser = UserFactory.CreateExternalUser();
            var cancellationToken = CancellationToken.None;
            
            _mockProfessionalOperatorRepository
                .Setup(x => x.GetAsync(externalUser, cancellationToken))
                .ReturnsAsync((ProfessionalOperator?)null);

            var sut = CreateSut();
            var result = await sut.ApplyProfileAsync(externalUser, new UserProfileModel(), cancellationToken);
            Assert.Equal(ApplyProfileOutcome.ProfessionalOperatorNotFound, result);

            _mockProfessionalOperatorRepository.Verify();
            _mockUserRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UserProfileDataMappedToUserProvidedToRepository()
        {
            var externalUser = UserFactory.CreateExternalUser();
            var cancellationToken = CancellationToken.None;
            var userProfileData = FixtureInstance
                .Build<UserProfileModel>().With(x => x.ProfessionalOperatorNumber, "123456")
                .Create();

            _mockProfessionalOperatorRepository
                .Setup(x => x.GetAsync(externalUser, cancellationToken))
                .ReturnsAsync(new ProfessionalOperator("test2@qxlva.com"));

            var sut = CreateSut();
            var result = await sut.ApplyProfileAsync(externalUser, userProfileData, cancellationToken);
            Assert.Equal(ApplyProfileOutcome.AccountRequiresApproval, result);

            _mockProfessionalOperatorRepository.Verify();
            _mockUserRepository.Verify(x => x.SaveProfileDataAsync(It.Is<ExternalUser>(user =>
                user.GivenName == userProfileData.FirstName &&
                user.Surname == userProfileData.LastName &&
                user.ProfessionalOperatorNumber == userProfileData.ProfessionalOperatorNumber &&
                user.Telephone == userProfileData.TelephoneNumber &&
                user.CompanyName == userProfileData.CompanyName &&
                user.StreetAddressLine1 == userProfileData.AddressLine1 &&
                user.StreetAddressLine2 == userProfileData.AddressLine2 &&
                user.StreetAddressLine3 == userProfileData.AddressLine3 &&
                user.StreetAddressLine4 == userProfileData.AddressLine4 &&
                user.PostalCode == userProfileData.PostalCode &&
                user.CreditAccountReference == userProfileData.CreditReferenceNumber &&
                user.SignedUpToCreditTermsAndConditions == userProfileData.AcceptsCreditTermsAndConditions &&
                user.HomeNation == userProfileData.HomeNation
            ), cancellationToken));
            _mockUserRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UserIsNotKeyContact()
        {
            var externalUser = UserFactory.CreateExternalUser();
            var cancellationToken = CancellationToken.None;
            var userProfileData = new UserProfileModel();

            _mockProfessionalOperatorRepository
                .Setup(x => x.GetAsync(externalUser, cancellationToken))
                .ReturnsAsync(new ProfessionalOperator("test2@qxlva.com"));

            var sut = CreateSut();
            var result = await sut.ApplyProfileAsync(externalUser, userProfileData, cancellationToken);
            Assert.Equal(ApplyProfileOutcome.AccountRequiresApproval, result);

            _mockProfessionalOperatorRepository.Verify();
            _mockUserRepository.Verify(x => x.SaveProfileDataAsync(It.Is<ExternalUser>(user =>
                user.IsApprovedAccount == false &&
                user.RegistrationEmailRecipient == "test2@qxlva.com"
            ), cancellationToken));
            _mockUserRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UserIsKeyContact()
        {
            var externalUser = UserFactory.CreateExternalUser();
            var cancellationToken = CancellationToken.None;
            var userProfileData = new UserProfileModel();

            _mockProfessionalOperatorRepository
                .Setup(x => x.GetAsync(externalUser, cancellationToken))
                .ReturnsAsync(new ProfessionalOperator(externalUser.Email!));

            var sut = CreateSut();
            var result = await sut.ApplyProfileAsync(externalUser, userProfileData, cancellationToken);
            Assert.Equal(ApplyProfileOutcome.UserCanAccessApplications, result);

            _mockProfessionalOperatorRepository.Verify();
            _mockUserRepository.Verify(x => x.SaveProfileDataAsync(It.Is<ExternalUser>(user =>
                user.IsApprovedAccount == true && 
                user.RegistrationEmailRecipient == user.Email
            ), cancellationToken));
            _mockUserRepository.VerifyNoOtherCalls();
        }

        private SignUpToUseSystemUseCase CreateSut()
        {
            return new SignUpToUseSystemUseCase(
                _mockUserRepository.Object,
                _mockProfessionalOperatorRepository.Object,
                Mock.Of<IHttpContextAccessor>(),
                new NullLogger<SignUpToUseSystemUseCase>());
        }
    }
}
