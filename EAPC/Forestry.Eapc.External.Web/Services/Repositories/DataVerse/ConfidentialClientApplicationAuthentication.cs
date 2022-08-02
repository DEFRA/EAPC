using Forestry.Eapc.External.Web.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Forestry.Eapc.External.Web.Services.Repositories.DataVerse
{
    public class ConfidentialClientApplicationAuthentication : AuthenticationBase
    {
        private readonly ILogger<ConfidentialClientApplicationAuthentication> _logger;
        private readonly PowerappsAuthenticationSettings _configuration;

        public ConfidentialClientApplicationAuthentication(
            IOptions<EapcEnvironmentSettings> configuration,
            ILogger<ConfidentialClientApplicationAuthentication> logger)
        : base(configuration.Value.PowerappsAuthentication, logger)
        {
        }
    }
}