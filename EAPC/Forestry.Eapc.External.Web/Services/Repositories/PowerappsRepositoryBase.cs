using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Configuration;
using Forestry.Eapc.External.Web.Services.Repositories.DataVerse;
using Microsoft.Extensions.Logging;

namespace Forestry.Eapc.External.Web.Services.Repositories
{
    public abstract class PowerappsRepositoryBase
    {
        private readonly ConfidentialClientApplicationAuthentication _authenticationHandler;
        private readonly ILogger<PowerappsRepositoryBase> _logger;
        private readonly PowerappsAuthenticationSettings _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Creates a new instance of a <see cref="PowerappsRepositoryBase"/>.
        /// </summary>
        /// <param name="httpClientFactory">A HTTP client factory.</param>
        /// <param name="authenticationHandler">A service which to authenticate the connection to Powerapps.</param>
        /// <param name="configuration">Configuration settings for the Powerapps endpoint to use.</param>
        /// <param name="logger">An <see cref="ILogger{TCategoryName}"/> implementation.</param>
        protected PowerappsRepositoryBase(
            IHttpClientFactory httpClientFactory,
            ConfidentialClientApplicationAuthentication authenticationHandler,
            PowerappsAuthenticationSettings configuration,
            ILogger<PowerappsRepositoryBase> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _authenticationHandler = authenticationHandler ?? throw new ArgumentNullException(nameof(authenticationHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Creates the base API endpoint pertinent to a given entity.
        /// </summary>
        /// <param name="entityName">The name of the entity/table to use in the endpoint string.</param>
        /// <returns>The API endpoint for requests to be sent against a given entity/table name.</returns>
        public string CreateEntityPathString(string entityName) => $"{_configuration.ApiUrl}api/data/v9.1/{entityName}";

        /// <summary>
        /// Adds the required HTTP header to request that the OData endpoint returns the JSON for the entity to us in the response payload.
        /// </summary>
        /// <param name="requestMessage"></param>
        protected void SetRequestToAskForDataRepresentationInResponse(HttpRequestMessage requestMessage)
        {
            if (requestMessage == null) throw new ArgumentNullException(nameof(requestMessage));

            if (requestMessage.Content == null)
            {
                throw new ArgumentException(
                    "Unable to set required request header as the provided request message has no content",
                    nameof(requestMessage));
            }

            requestMessage.Content.Headers.TryAddWithoutValidation("Prefer", "return=representation");
        }

        /// <summary>
        /// Sends the provided <paramref name="requestMessage"/> to the Powerapps OData endpoint using the configured <see cref="HttpClient"/>.
        /// This methods carries out authentication ahead of sending a request using details from the <see cref="PowerappsAuthenticationSettings"/>
        /// options included within the constructor of the current instance.
        /// </summary>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> to send.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> representing the response from the Powerapps OData endpoint.</returns>
        /// <exception cref="RepositoryException">Thrown if authentication fails and thus no access token can be obtained to include in the request.</exception>
        protected virtual async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            if (requestMessage == null) throw new ArgumentNullException(nameof(requestMessage));

            _logger.LogDebug("Acquiring authentication token for powerapps API endpoint");
            var authenticationResult = await _authenticationHandler.AuthenticateAsync();

            if (authenticationResult == null)
            {
                throw new RepositoryException("Failed to acquire an access token to use in the request to the powerapps endpoint");
            }

            using var httpClient = _httpClientFactory.CreateClient("Dataverse");
            
            _logger.LogDebug("Acquired authentication token for powerapps API endpoint with value {AccessToken}", authenticationResult.AccessToken);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult!.AccessToken);

            _logger.LogInformation("Sending request to powerapps endpoint at {Endpoint}", requestMessage.RequestUri);
            var result = await httpClient.SendAsync(requestMessage, cancellationToken);
            _logger.LogInformation("Response received powerapps endpoint at {Endpoint} with response code {HttpResponseCode}", requestMessage.RequestUri, result.StatusCode);

            return result;
        }
    }
}
