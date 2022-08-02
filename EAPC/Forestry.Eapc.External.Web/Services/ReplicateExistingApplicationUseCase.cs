using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Forestry.Eapc.External.Web.Models.Application;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;

namespace Forestry.Eapc.External.Web.Services
{
    public class ReplicateExistingApplicationUseCase
    {
        private readonly IClock _clock;
        private readonly IApplicationRepository _repository;
        private readonly ILogger<ReplicateExistingApplicationUseCase> _logger;

        public ReplicateExistingApplicationUseCase(
            IClock clock, 
            IApplicationRepository repository,
            ILogger<ReplicateExistingApplicationUseCase> logger)
        {
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? new NullLogger<ReplicateExistingApplicationUseCase>();
        }

        public async Task<Application> GetByIdAsync(string applicationId, ExternalUser user, CancellationToken cancellationToken)
        {
            var application = await _repository.GetByIdAsync(applicationId, user, cancellationToken);

            if (application == null)
            {
                _logger.LogError(
                    "Unable to locate an application with identifier {ApplicationId} for user with Professional Operator No {ProfessionalOperatorNumber}",
                    applicationId,
                    user.ProfessionalOperatorNumber);

                throw new ApplicationNotFoundException(
                    $"Unable to locate application {applicationId} for user with Professional Operator No {user.ProfessionalOperatorNumber}");
            }
            
            return application!;
        }

        public async Task<Result<Application>> ReplicateAsync(string applicationId, ExternalUser user, CancellationToken cancellationToken)
        {
            if (applicationId == null) throw new ArgumentNullException(nameof(applicationId));
            if (user == null) throw new ArgumentNullException(nameof(user));

            var application = await GetByIdAsync(applicationId, user, cancellationToken);
            var copy = CreateCopy(application, user);
            var result = await _repository.UpsertAsync(copy, user, cancellationToken);
            return result;
        }
        
        private Application CreateCopy(Application value, ExternalUser user)
        {
            var asJson = JsonSerializer.Serialize(value);
            var result = JsonSerializer.Deserialize<Application>(asJson);
            result!.Identifier = null;
            result.ReferenceIdentifier = null;
            result.State = ApplicationState.Draft;
            result.CreationDate = ClockHelper.GetToday(_clock);
            result.SupportingDocumentsSection.SupportingDocuments = new List<SupportingDocument>(0);
            result.Section3.DateOfExport = null;
            result.Section5.DateOfTreatment = null;
            result.ImplantUser(user);

            return result;
        }
    }
}
