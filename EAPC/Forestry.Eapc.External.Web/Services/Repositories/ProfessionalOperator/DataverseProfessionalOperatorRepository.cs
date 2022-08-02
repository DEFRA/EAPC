using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Forestry.Eapc.External.Web.Configuration;
using Forestry.Eapc.External.Web.Models.Repository;
using Forestry.Eapc.External.Web.Services.Repositories.DataVerse;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Forestry.Eapc.External.Web.Services.Repositories.ProfessionalOperator
{
    /// <summary>
    /// Implementation of <see cref="IProfessionalOperatorRepository"/> that retrieves information from Dataverse.
    /// </summary>
    public class DataverseProfessionalOperatorRepository : IProfessionalOperatorRepository
    {
        private readonly ProfessionalOperatorClientApplicationAuthentication _authenticationHandler;
        private readonly ILogger<DataverseProfessionalOperatorRepository> _logger;
        private readonly HttpClient _httpClient;
        private readonly ProfessionalOperatorRegistrationEnvironmentSettings _configuration;

        public DataverseProfessionalOperatorRepository(
            IHttpClientFactory httpClientFactory,
            ProfessionalOperatorClientApplicationAuthentication authenticationHandler,
            IOptions<ProfessionalOperatorRegistrationEnvironmentSettings> configuration,
            ILogger<DataverseProfessionalOperatorRepository> logger)
        {
            if (httpClientFactory == null)
                throw new ArgumentNullException(nameof(httpClientFactory));

            _authenticationHandler = authenticationHandler ?? throw new ArgumentNullException(nameof(authenticationHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClientFactory.CreateClient("ProfessionalOperator");
            _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<ProfessionalOperator?> GetAsync(ExternalUser externalUser, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(externalUser?.ProfessionalOperatorNumber))
            {
                _logger.LogDebug("No professional operator number was given for the external user, returning null");
                return null;
            }

            await AuthenticateAsync();

            var filter = $"$filter=endswith({_configuration.ProfessionalOperatorNumberField},'{externalUser.ProfessionalOperatorNumber}')";
            var select = $"$select={_configuration.ProfessionalOperatorNumberField}";
            var expand = $"$expand={_configuration.OperatorLookupField}($select={_configuration.OperatorEmailField})";
            var url = $"{_configuration.PowerappsAuthentication.ApiUrl}api/data/v9.1/{_configuration.OperatorRegistrationTable}s?{filter}&{select}&{expand}";

            _logger.LogDebug("Attempting to retrieve professional operator details from POR Environment Dataverse for PO number {ProfessionalOperatorNumber} at endpoint {url}", externalUser.ProfessionalOperatorNumber, url);

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode == false)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken: cancellationToken);
                _logger.LogError("Received unsuccessful response with code {StatusCode} and message {Error}", response.StatusCode, error?.Error?.Message);
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            var keyContactEmail = TryParseEmailFromResponseContent(responseContent);
            if (keyContactEmail.HasValue)
            {
                return new ProfessionalOperator(keyContactEmail.Value);
            }

            _logger.LogDebug("Could not parse Professional Operator details response for professional operator number {ProfessionalOperatorNumber}", externalUser.ProfessionalOperatorNumber);
            return null;
        }

        private async Task AuthenticateAsync()
        {
            var authenticationResult = await _authenticationHandler.AuthenticateAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult!.AccessToken);
        }

        private Maybe<string> TryParseEmailFromResponseContent(string responseContent)
        {
            try
            {
                using JsonDocument document = JsonDocument.Parse(responseContent);
                var docEl = document.RootElement;
                var registrationEls = docEl.GetProperty("value").EnumerateArray();
                var operatorEl = registrationEls.First().GetProperty(_configuration.OperatorLookupField);
                var result = operatorEl.GetProperty(_configuration.OperatorEmailField).GetString() ?? null;

                if (!string.IsNullOrWhiteSpace(result))
                {
                    return Maybe<string>.From(result);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception caught attempting to parse Professional Operator Registrations response");
            }
            return Maybe<string>.None;
        }
    }
}