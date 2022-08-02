using System;
using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Models.Accounts;
using Forestry.Eapc.External.Web.Services;
using Forestry.Eapc.External.Web.Services.Repositories.Users;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Services
{
    public class ApproveAccountUseCaseTests
    {
        private readonly Mock<ILocalUserRepository> _localUserRepository = new Mock<ILocalUserRepository>();

        [Fact]
        public async Task RejectsRequestWhenProfessionalOperatorNumberProvidedDoesNotMatchCurrentUser()
        {
            var sut = CreateSut();
            var requestModel = CreateApproveAccountModel();
            var externalUser = UserFactory.CreateExternalUser();

            var result = await sut.ExecuteAsync(requestModel, externalUser, CancellationToken.None);
            Assert.Equal(ApproveAccountOutcome.ProfessionalOperatorNumberMismatch, result);
            _localUserRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ReturnsCorrectResultWhenRepositoryCannotFindAccount()
        {
            var cancellationToken = CancellationToken.None;
            var sut = CreateSut();
            var externalUser = UserFactory.CreateExternalUser();
            var requestModel = CreateApproveAccountModel(x => x.ProfessionalOperatorNumber = externalUser.ProfessionalOperatorNumber!);
            
            _localUserRepository
                .Setup(x => x.ApproveUserAccountAsync(requestModel.Email, requestModel.ProfessionalOperatorNumber, externalUser, cancellationToken))
                .ReturnsAsync(false);

            var result = await sut.ExecuteAsync(requestModel, externalUser, cancellationToken);
            Assert.Equal(ApproveAccountOutcome.LocalAccountNotFound, result);

            _localUserRepository.VerifyAll();
            _localUserRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ReturnsCorrectResultWhenAccountEnabledInRepository()
        {
            var cancellationToken = CancellationToken.None;
            var sut = CreateSut();
            var externalUser = UserFactory.CreateExternalUser();
            var requestModel = CreateApproveAccountModel(x => x.ProfessionalOperatorNumber = externalUser.ProfessionalOperatorNumber!);
            
            _localUserRepository
                .Setup(x => x.ApproveUserAccountAsync(requestModel.Email, requestModel.ProfessionalOperatorNumber, externalUser, cancellationToken))
                .ReturnsAsync(true);

            var result = await sut.ExecuteAsync(requestModel, externalUser, cancellationToken);
            Assert.Equal(ApproveAccountOutcome.Success, result);

            _localUserRepository.VerifyAll();
            _localUserRepository.VerifyNoOtherCalls();
        }

        private ApproveAccountUseCase CreateSut()
        {
            return new ApproveAccountUseCase(
                _localUserRepository.Object,
                new NullLogger<ApproveAccountModel>());
        }

        private static ApproveAccountModel CreateApproveAccountModel(Action<ApproveAccountModel>? callback = null)
        {
            var result = new ApproveAccountModel {Email = "test@qxlva.com", ProfessionalOperatorNumber = "547896"};
            callback?.Invoke(result);

            return result;
        }
    }
}
