using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Forestry.Eapc.External.Web.Models.Application;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Extensions;

namespace Forestry.Eapc.External.Web.Services
{
    public class CreateNewApplicationUseCase
    {
        private readonly IClock _clock;
        private readonly IApplicationRepository _repository;
        private readonly ILogger<CreateNewApplicationUseCase> _logger;

        public CreateNewApplicationUseCase(
            IClock clock, 
            IApplicationRepository repository,
            ILogger<CreateNewApplicationUseCase> logger)
        {
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<Application>> CreateAsync(ExternalUser user, CancellationToken cancellationToken = default)
        {
            var application = new Application();
            application.CreationDate = ClockHelper.GetToday(_clock);
            application.ImplantUser(user);

            var result = await _repository.UpsertAsync(application, user, cancellationToken);
            return result;
        }
    }
}
