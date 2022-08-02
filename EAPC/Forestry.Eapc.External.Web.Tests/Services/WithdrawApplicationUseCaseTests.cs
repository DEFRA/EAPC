using System;
using System.Linq;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Models.Application;
using Forestry.Eapc.External.Web.Services;
using Forestry.Eapc.External.Web.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Services
{
    public class WithdrawApplicationUseCaseTests
    {
        private readonly FakeApplicationRepository _fakeApplicationRepository = new();

        [Theory]
        [InlineData(ApplicationState.Draft)]
        [InlineData(ApplicationState.Submitted)]
        public async Task ShouldTransitionApplicationStateToWithdrawn(ApplicationState state)
        {
            var application = new Application()
            {
                State = state
            };
            var sut = CreateSut();
            await sut.SetApplicationWithdrawnAsync(application, UserFactory.CreateExternalUser());

            var storedApplication = _fakeApplicationRepository.GetAll().Single();
            Assert.Equal(ApplicationState.Withdrawn, storedApplication.State);
        }

        [Theory]
        [InlineData(ApplicationState.Charged)]
        [InlineData(ApplicationState.Issued)]
        [InlineData(ApplicationState.Paid)]
        [InlineData(ApplicationState.Withdrawn)]
        [InlineData(ApplicationState.Unknown)]
        public async Task GuardsAgainstWithdrawalOfApplicationsNotInDraftOrSubmittedState(ApplicationState state)
        {
            var application = new Application { State = state };
            var sut = CreateSut();

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await sut.SetApplicationWithdrawnAsync(application, UserFactory.CreateExternalUser()));
        }

        private WithdrawApplicationUseCase CreateSut() =>
            new(_fakeApplicationRepository, new NullLogger<WithdrawApplicationUseCase>());
    }
}