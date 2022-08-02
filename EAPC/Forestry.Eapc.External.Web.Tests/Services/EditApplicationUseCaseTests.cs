using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Models.Application;
using Forestry.Eapc.External.Web.Services;
using Forestry.Eapc.External.Web.Services.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Services
{
    public class EditApplicationUseCaseTests
    {
        private readonly InMemoryApplicationRepository _repository = new();

        /// <summary>
        /// Although the professional operator number field on the form is read-only this test exists to ensure
        /// that if a user has edited the HTML content directly then we override the professional operator number
        /// and reset it server-side before saving.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldAlwaysSetApplicantProfessionalOperatorNumberToValueLinkedToCurrentUser()
        {
            // add to the in memory repository
            var user = UserFactory.CreateExternalUser();
            var application = new Application();
            application.Applicant.ProfessionalOperatorNumber = user.ProfessionalOperatorNumber;
            var upsertResult = await _repository.UpsertAsync(application, user);
            Assert.True(upsertResult.IsSuccess);
            application = upsertResult.Value;
            
            // invoke the use case service
            application.Applicant.ProfessionalOperatorNumber = "some other value";
            var sut = CreateSut();
            await sut.SaveChangesAsync(application, user, CancellationToken.None);
            Assert.Equal(user.ProfessionalOperatorNumber, application.Applicant.ProfessionalOperatorNumber);

            application = await _repository.GetByIdAsync(application.Identifier!, user);
            Assert.Equal(user.ProfessionalOperatorNumber, application!.Applicant.ProfessionalOperatorNumber);
        }

        private EditApplicationUseCase CreateSut()
        {
            var result = new EditApplicationUseCase(_repository, new NullLogger<EditApplicationUseCase>());
            return result;
        }
    }
}
