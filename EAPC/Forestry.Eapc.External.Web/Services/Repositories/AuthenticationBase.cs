using System;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace Forestry.Eapc.External.Web.Services.Repositories
{
    public abstract class AuthenticationBase
    {
        private readonly PowerappsAuthenticationSettings _configuration;
        private readonly ILogger<AuthenticationBase> _logger;

        public AuthenticationBase(PowerappsAuthenticationSettings configuration, ILogger<AuthenticationBase> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual async Task<AuthenticationResult?> AuthenticateAsync()
        {
            _logger.LogDebug("Attempting to authenticate using client secret from configuration");
            var app = ConfidentialClientApplicationBuilder.Create(_configuration.ClientId)
                .WithClientSecret(_configuration.ClientSecret)
                .WithAuthority(new Uri(_configuration.Authority))
                .Build();

            var scopes = new string[] { $"{_configuration.ApiUrl}.default" };
            return await app.AcquireTokenForClient(scopes).ExecuteAsync();
        }
    }
}