using Forestry.Eapc.External.Web.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Forestry.Eapc.External.Web.Services.Repositories.ProfessionalOperator
{
    public class ProfessionalOperatorClientApplicationAuthentication : AuthenticationBase
    {
        private readonly ILogger<ProfessionalOperatorClientApplicationAuthentication> _logger;
        private readonly PowerappsAuthenticationSettings _configuration;

        public ProfessionalOperatorClientApplicationAuthentication(
            IOptions<ProfessionalOperatorRegistrationEnvironmentSettings> configuration,
            ILogger<ProfessionalOperatorClientApplicationAuthentication> logger)
        : base(configuration.Value.PowerappsAuthentication, logger)
        {
        }
    }
}