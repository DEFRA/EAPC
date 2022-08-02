using System;
using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Models.Application;
using Microsoft.Extensions.Logging;

namespace Forestry.Eapc.External.Web.Services
{
    public class WithdrawApplicationUseCase
    {
        private readonly IApplicationRepository _repository;
        private readonly ILogger<WithdrawApplicationUseCase> _logger;

        public WithdrawApplicationUseCase(IApplicationRepository repository, ILogger<WithdrawApplicationUseCase> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Application?> GetByIdAsync(string id, ExternalUser user, CancellationToken cancellationToken = default)
        {
            var application = await _repository.GetByIdAsync(id, user, cancellationToken);
            return application;
        }

        public async Task SetApplicationWithdrawnAsync(Application application, ExternalUser user, CancellationToken cancellationToken = default)
        {
            if (application == null) throw new ArgumentNullException(nameof(application));
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (application.State != ApplicationState.Draft && application.State != ApplicationState.Submitted)
            {
                throw new InvalidOperationException(
                    $"Cannot withdraw application with identifier {application.Identifier} as the application is in a {application.State} state.");
            }

            application.State = ApplicationState.Withdrawn;
            await _repository.UpsertAsync(application, user, cancellationToken);
        }
    }
}