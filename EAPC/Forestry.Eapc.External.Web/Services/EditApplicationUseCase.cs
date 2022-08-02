using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Forestry.Eapc.External.Web.Models.Application;
using Microsoft.Extensions.Logging;

namespace Forestry.Eapc.External.Web.Services
{
    public class EditApplicationUseCase
    {
        private readonly IApplicationRepository _repository;
        private readonly ILogger<EditApplicationUseCase> _logger;

        public EditApplicationUseCase(IApplicationRepository repository, ILogger<EditApplicationUseCase> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Application?> GetByIdAsync(string id, ExternalUser user, CancellationToken cancellationToken = default)
        {
            var application = await _repository.GetByIdAsync(id, user, cancellationToken);
            return application;
        }

        public async Task<Result> SaveChangesAsync(Application application, ExternalUser user, CancellationToken cancellationToken)
        {
            if (application.State != ApplicationState.Draft)
            {
                return Result.Failure(
                    $"Cannot save changes to application with identifier {application.Identifier} as the application is in a {application.State} state. To be updated an application must be in a Draft state.");
            }

            application.Applicant.ProfessionalOperatorNumber = user.ProfessionalOperatorNumber; // this is set just in case a user has edited the HTML content themselves and overridden our read-only field
            var saveResult = await _repository.UpsertAsync(application, user, cancellationToken);
            return saveResult.IsSuccess
                ? Result.Success()
                : saveResult.ConvertFailure();
        }
    }
}
