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
    public class SubmitApplicationUseCaseTests
    {
        private readonly FakeApplicationRepository _fakeApplicationRepository = new();

        [Fact]
        public async Task ShouldTransitionApplicationStateToSubmitted()
        {
            var application = new Application {Confirmation = new Confirmation {AcceptTermsAndConditions = true}};
            var sut = CreateSut();
            await sut.SubmitAsync(application, UserFactory.CreateExternalUser());

            var storedApplication = _fakeApplicationRepository.GetAll().Single();
            Assert.Equal(ApplicationState.Submitted, storedApplication.State);
        }

        [Theory]
        [InlineData(ApplicationState.Submitted)]
        [InlineData(ApplicationState.Charged)]
        [InlineData(ApplicationState.Issued)]
        [InlineData(ApplicationState.Paid)]
        [InlineData(ApplicationState.Withdrawn)]
        [InlineData(ApplicationState.Unknown)]
        public async Task GuardsAgainstSubmissionOfApplicationsNotInDraftState(ApplicationState state)
        {
            var application = new Application {State = state, Confirmation = new Confirmation {AcceptTermsAndConditions = true}};
            var sut = CreateSut();
            
            await Assert.ThrowsAsync<InvalidOperationException>(async () => 
                await sut.SubmitAsync(application, UserFactory.CreateExternalUser()));
        }

        [Fact]
        public async Task GuardsAgainstSubmissionOfApplicationsWhereTermsAndConditionsNotAccepted()
        {
            var application = new Application { Confirmation = new Confirmation { AcceptTermsAndConditions = false } };
            var sut = CreateSut();

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await sut.SubmitAsync(application, UserFactory.CreateExternalUser()));
        }

        private SubmitApplicationUseCase CreateSut() => 
            new(_fakeApplicationRepository, new NullLogger<SubmitApplicationUseCase>());
    }
}
