using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Forestry.Eapc.External.Web.Models.Application;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Forestry.Eapc.External.Web.Services
{
    public class SubmitApplicationUseCase
    {
        private readonly IApplicationRepository _repository;
        private readonly ILogger<SubmitApplicationUseCase> _logger;

        public SubmitApplicationUseCase(IApplicationRepository repository, ILogger<SubmitApplicationUseCase> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? new NullLogger<SubmitApplicationUseCase>();
        }

        public async Task<Result> SubmitAsync(Application application, ExternalUser user, CancellationToken cancellationToken = default)
        {
            if (application == null) throw new ArgumentNullException(nameof(application));
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (application.State != ApplicationState.Draft)
            {
                throw new InvalidOperationException(
                    $"Cannot submit application with identifier {application.Identifier} as the application is in a {application.State} state. To be submitted an application must be in a Draft state.");
            }

            if (application.Confirmation.AcceptTermsAndConditions == false)
            {
                throw new InvalidOperationException(
                    $"Cannot submit application with identifier {application.Identifier} as the terms and conditions have not been accepted.");
            }

            _logger.LogInformation(
                "Submitting application with internal identifier {Identifier} and reference {ReferenceIdentifier}", 
                application.Identifier,
                application.ReferenceIdentifier);

            application.State = ApplicationState.Submitted;

            return await _repository.UpsertAsync(application, user, cancellationToken)
                .Tap(() => _logger.LogInformation(
                    "Successfully submitted application with internal identifier {Identifier} and reference {ReferenceIdentifier}",
                    application.Identifier,
                    application.ReferenceIdentifier))
                .OnFailure((error) => _logger.LogError(
                    "Failed to submit application with internal identifier {Identifier} and reference {ReferenceIdentifier}, error: {Error}",
                    application.Identifier,
                    application.ReferenceIdentifier,
                    error));
        }
    }
}
