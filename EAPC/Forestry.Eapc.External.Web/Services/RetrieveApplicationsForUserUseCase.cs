using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Models.Application;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Forestry.Eapc.External.Web.Services
{
    public class RetrieveApplicationsForUserUseCase
    {
        private readonly IApplicationRepository _repository;
        private readonly ILogger<RetrieveApplicationsForUserUseCase> _logger;

        public RetrieveApplicationsForUserUseCase(IApplicationRepository repository, ILogger<RetrieveApplicationsForUserUseCase> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? new NullLogger<RetrieveApplicationsForUserUseCase>();
        }

        public async Task<IReadOnlyCollection<Application>> RetrieveForUserAsync(ExternalUser user, CancellationToken cancellationToken)
        {
            var applications = await _repository.GetAllForUserAsync(user, false, cancellationToken);
            return applications;
        }

    }
}