using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Models.Application;
using Forestry.Eapc.External.Web.Services;
using Forestry.Eapc.External.Web.Services.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Services
{
    public class ReplicateExistingApplicationUseCaseTests
    {
        private readonly InMemoryApplicationRepository _repository = new();

        [Fact]
        public async Task ShouldRejectNullValues()
        {
            var sut = CreateSut();
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.ReplicateAsync(null, UserFactory.CreateExternalUser(), CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.ReplicateAsync("application-id", null, CancellationToken.None));
        }

        [Fact]
        public async Task ShouldThrowWhenApplicationNotFoundByRepository()
        {
            var sut = CreateSut();
            await Assert.ThrowsAsync<ApplicationNotFoundException>(async () => await sut.ReplicateAsync("does not exist", UserFactory.CreateExternalUser(), CancellationToken.None));
        }

        [Fact]
        public async Task ShouldSendNewApplicationInstanceToRepository()
        {
            var externalUser = UserFactory.CreateExternalUser();
            var originalApplication = ApplicationFactory.CreateApplication(x => x.Applicant.ProfessionalOperatorNumber = externalUser.ProfessionalOperatorNumber);
            var upsertResult = await _repository.UpsertAsync(originalApplication, externalUser, CancellationToken.None);
            Assert.True(upsertResult.IsSuccess);
            originalApplication = upsertResult.Value;
            
            var sut = CreateSut();
            var newAppResult = await sut.ReplicateAsync(originalApplication.Identifier!, externalUser, CancellationToken.None);
            Assert.True(newAppResult.IsSuccess);
            var newApplication = newAppResult.Value;

            Assert.NotNull(newApplication);
            Assert.NotSame(originalApplication, newApplication);
            Assert.NotEqual(newApplication.Identifier, originalApplication.Identifier);
            
            var storedReplicatedApplication = await _repository.GetByIdAsync(newApplication.Identifier!, externalUser, CancellationToken.None);
            Assert.NotNull(storedReplicatedApplication);
        }

        [Fact]
        public async Task ReplicatedApplicationShouldNotCopySupportingDocumentation()
        {
            var externalUser = UserFactory.CreateExternalUser();
            var originalApplication = ApplicationFactory.CreateApplication(x =>
            {
                x.Applicant.ProfessionalOperatorNumber = externalUser.ProfessionalOperatorNumber;
                x.SupportingDocumentsSection.SupportingDocuments.Add(new SupportingDocument {Identifier = "document-1"});
                x.SupportingDocumentsSection.SupportingDocuments.Add(new SupportingDocument {Identifier = "document-2"});
            });
            var upsertResult = await _repository.UpsertAsync(originalApplication, externalUser, CancellationToken.None);
            Assert.True(upsertResult.IsSuccess);
            originalApplication = upsertResult.Value;

            var sut = CreateSut();
            var newAppResult = await sut.ReplicateAsync(originalApplication.Identifier!, externalUser, CancellationToken.None);
            Assert.True(newAppResult.IsSuccess);
            var newApplication = newAppResult.Value;
            var storedReplicatedApplication = await _repository.GetByIdAsync(newApplication.Identifier!, externalUser, CancellationToken.None);
            
            Assert.Empty(storedReplicatedApplication!.SupportingDocumentsSection!.SupportingDocuments);
        }

        [Fact]
        public async Task ReplicatedApplicationShouldNotCopyExportDate()
        {
            var externalUser = UserFactory.CreateExternalUser();
            var originalApplication = ApplicationFactory.CreateApplication(x =>
            {
                x.Applicant.ProfessionalOperatorNumber = externalUser.ProfessionalOperatorNumber;
                x.Section3.DateOfExport = DateTime.UtcNow;
            });
            var upsertResult = await _repository.UpsertAsync(originalApplication, externalUser, CancellationToken.None);
            Assert.True(upsertResult.IsSuccess);
            originalApplication = upsertResult.Value;

            var sut = CreateSut();
            var newAppResult = await sut.ReplicateAsync(originalApplication.Identifier!, externalUser, CancellationToken.None);
            Assert.True(newAppResult.IsSuccess);
            var newApplication = newAppResult.Value;
            var storedReplicatedApplication = await _repository.GetByIdAsync(newApplication.Identifier!, externalUser, CancellationToken.None);
            
            Assert.Null(storedReplicatedApplication!.Section3.DateOfExport);
        }

        [Fact]
        public async Task ReplicatedApplicationShouldNotCopyTreatmentDate()
        {
            var externalUser = UserFactory.CreateExternalUser();
            var originalApplication = ApplicationFactory.CreateApplication(x =>
            {
                x.Applicant.ProfessionalOperatorNumber = externalUser.ProfessionalOperatorNumber;
                x.Section5.DateOfTreatment = DateTime.UtcNow;
            });
            var upsertResult = await _repository.UpsertAsync(originalApplication, externalUser, CancellationToken.None);
            Assert.True(upsertResult.IsSuccess);
            originalApplication = upsertResult.Value;

            var sut = CreateSut();
            var newAppResult = await sut.ReplicateAsync(originalApplication.Identifier!, externalUser, CancellationToken.None);
            Assert.True(newAppResult.IsSuccess);
            var newApplication = newAppResult.Value;
            var storedReplicatedApplication = await _repository.GetByIdAsync(newApplication.Identifier!, externalUser, CancellationToken.None);
            
            Assert.Null(storedReplicatedApplication!.Section5.DateOfTreatment);
        }

        [Fact]
        public async Task ReplicatedApplicationShouldHaveCurrentDateAsCreationDate()
        {
            var externalUser = UserFactory.CreateExternalUser();
            var originalApplication = ApplicationFactory.CreateApplication(x =>
            {
                x.Applicant.ProfessionalOperatorNumber = externalUser.ProfessionalOperatorNumber;
                x.CreationDate = DateTime.UtcNow.AddDays(-7);
            });
            var upsertResult = await _repository.UpsertAsync(originalApplication, externalUser, CancellationToken.None);
            Assert.True(upsertResult.IsSuccess);
            originalApplication = upsertResult.Value;

            var sut = CreateSut();
            var newAppResult = await sut.ReplicateAsync(originalApplication.Identifier!, externalUser, CancellationToken.None);
            Assert.True(newAppResult.IsSuccess);
            var newApplication = newAppResult.Value;
            var storedReplicatedApplication = await _repository.GetByIdAsync(newApplication.Identifier!, externalUser, CancellationToken.None);
            
            Assert.Equal(DateTime.UtcNow.Date, storedReplicatedApplication!.CreationDate);
        }

        [Fact]
        public async Task ReplicatedApplicationShouldMaintainListsAndArrays()
        {
            var externalUser = UserFactory.CreateExternalUser();
            var originalApplication = ApplicationFactory.CreateApplication(x =>
            {
                x.Applicant.ProfessionalOperatorNumber = externalUser.ProfessionalOperatorNumber;
                x.Section4.WhereGrowns = new[] {"Where Grown 1", "Where Grown 2"};
                x.Section4.Quantity = new List<Quantity> {new() {Amount = 1, Unit = QuantityUnit.KG}, new() {Amount = 2, Unit = QuantityUnit.KG}};
                x.Section4.BotanicalNames = new[] {"Botanical Name 1", "Botanical Name 2"};
            });
            var upsertResult = await _repository.UpsertAsync(originalApplication, externalUser, CancellationToken.None);
            Assert.True(upsertResult.IsSuccess);
            originalApplication = upsertResult.Value;

            var sut = CreateSut();
            var newAppResult = await sut.ReplicateAsync(originalApplication.Identifier!, externalUser, CancellationToken.None);
            Assert.True(newAppResult.IsSuccess);
            var newApplication = newAppResult.Value;
            var storedReplicatedApplication = await _repository.GetByIdAsync(newApplication.Identifier!, externalUser, CancellationToken.None);
            
            Assert.Equal(2, storedReplicatedApplication!.Section4.WhereGrowns.Length);
            Assert.Equal(2, storedReplicatedApplication!.Section4.Quantity.Count);
            Assert.Equal(2, storedReplicatedApplication!.Section4.BotanicalNames.Length);
        }

        [Theory]
        [InlineData(ApplicationState.Draft)]
        [InlineData(ApplicationState.Submitted)]
        [InlineData(ApplicationState.Withdrawn)]
        [InlineData(ApplicationState.Issued)]
        public async Task ReplicatedApplicationShouldBeInDraftState(ApplicationState setupState)
        {
            var externalUser = UserFactory.CreateExternalUser();
            var originalApplication = ApplicationFactory.CreateApplication(x =>
            {
                x.Applicant.ProfessionalOperatorNumber = externalUser.ProfessionalOperatorNumber;
                x.State = setupState;
            });
            var upsertResult = await _repository.UpsertAsync(originalApplication, externalUser, CancellationToken.None);
            Assert.True(upsertResult.IsSuccess);
            originalApplication = upsertResult.Value;

            var sut = CreateSut();
            var newAppResult = await sut.ReplicateAsync(originalApplication.Identifier!, externalUser, CancellationToken.None);
            Assert.True(newAppResult.IsSuccess);
            var newApplication = newAppResult.Value;
            var storedReplicatedApplication = await _repository.GetByIdAsync(newApplication.Identifier!, externalUser, CancellationToken.None);
            
            Assert.Equal(ApplicationState.Draft, storedReplicatedApplication!.State);
        }

        [Fact]
        public async Task ReplicatedApplicationShouldContainLatestDataFromExternalUser()
        {
            var externalUser = UserFactory.CreateExternalUser();
            var originalApplication = ApplicationFactory.CreateApplication(x =>
            {
                x.Applicant.ProfessionalOperatorNumber = externalUser.ProfessionalOperatorNumber;
            });
            var upsertResult = await _repository.UpsertAsync(originalApplication, externalUser, CancellationToken.None);
            Assert.True(upsertResult.IsSuccess);
            originalApplication = upsertResult.Value;

            var sut = CreateSut();
            var newAppResult = await sut.ReplicateAsync(originalApplication.Identifier!, externalUser, CancellationToken.None);
            Assert.True(newAppResult.IsSuccess);
            var newApplication = newAppResult.Value;
            var storedReplicatedApplication = await _repository.GetByIdAsync(newApplication.Identifier!, externalUser, CancellationToken.None);
            
            Assert.Equal(externalUser.CompanyName, storedReplicatedApplication!.Applicant.CompanyName);
            Assert.Equal(externalUser.Email, storedReplicatedApplication.Applicant.Email);
            Assert.Equal(externalUser.FullName, storedReplicatedApplication.Applicant.PersonName);
            Assert.Equal(externalUser.HomeNation, storedReplicatedApplication.Applicant.Region);
            Assert.Equal(externalUser.Telephone, storedReplicatedApplication.Applicant.Telephone);
        }

        private ReplicateExistingApplicationUseCase CreateSut()
        {
            return new ReplicateExistingApplicationUseCase(
                SystemClock.Instance,
                _repository,
                new NullLogger<ReplicateExistingApplicationUseCase>());
        }
    }
}
