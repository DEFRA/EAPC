using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Forestry.Eapc.External.Web.Configuration;
using Forestry.Eapc.External.Web.Models.Application;
using Forestry.Eapc.External.Web.Models.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Forestry.Eapc.External.Web.Services.Repositories.DataVerse
{
    /// <summary>
    /// Implementation of <see cref="IApplicationRepository"/> and <see cref="ISupportingDocumentRepository"/> that stores data within Powerapps/Dataverse.
    /// </summary>
    public class DataverseRepository : IApplicationRepository, IApplicationUnattendedRepository, ISupportingDocumentRepository
    {
        private readonly ConfidentialClientApplicationAuthentication _authenticationHandler;
        private readonly ILogger<DataverseRepository> _logger;
        private readonly PowerappsAuthenticationSettings _configuration;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Creates a new instance of a <see cref="DataverseRepository"/>.
        /// </summary>
        /// <param name="httpClientFactory">An <see cref="IHttpClientFactory"/> to provide the <see cref="HttpClient"/> with
        /// which to communicate with the Dataverse Web API.</param>
        /// <param name="authenticationHandler">A service with which to authenticate the connection to Dataverse.</param>
        /// <param name="configuration">Configuration settings for the Dataverse endpoint to use.</param>
        /// <param name="logger">An <see cref="ILogger{TCategoryName}"/> implementation.</param>
        public DataverseRepository(
            IHttpClientFactory httpClientFactory,
            ConfidentialClientApplicationAuthentication authenticationHandler,
            IOptions<EapcEnvironmentSettings> configuration,
            ILogger<DataverseRepository> logger)
        {
            if (httpClientFactory == null)
                throw new ArgumentNullException(nameof(httpClientFactory));

            _authenticationHandler = authenticationHandler ?? throw new ArgumentNullException(nameof(authenticationHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClientFactory.CreateClient("Dataverse");
            _configuration = configuration?.Value?.PowerappsAuthentication ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <inheritdoc />
        public async Task<Application?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            
            await AuthenticateAsync();
            var url = $"{_configuration.ApiUrl}api/data/v9.1/{CertificateApplication.EntityName}({id})";

            _logger.LogDebug("Attempting to retrieve Application from Dataverse for id {id} at endpoint {url}", id, url);

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode == false)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken: cancellationToken);
                _logger.LogError("Received unsuccessful response with code {StatusCode} and message {Error}", response.StatusCode, error?.Error?.Message);
                return null;
            }

            _logger.LogDebug("Received successful response with code {StatusCode}", response.StatusCode);
            var responseContent = await response.Content.ReadFromJsonAsync<CertificateApplication>(cancellationToken: cancellationToken);
            var result = responseContent?.ToApplication();
            return result;
        }

        /// <inheritdoc />
        public async Task<Application?> GetByIdAsync(string id, ExternalUser user, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            
            await AuthenticateAsync();

            var url = $"{_configuration.ApiUrl}api/data/v9.1/{CertificateApplication.EntityName}({id})";

            _logger.LogDebug("Attempting to retrieve Application from Dataverse for id {id} at endpoint {url}", id, url);

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode == false)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken: cancellationToken);
                _logger.LogError("Received unsuccessful response with code {StatusCode} and message {Error}", response.StatusCode, error?.Error?.Message);
                return null;
            }

            _logger.LogDebug("Received successful response with code {StatusCode}", response.StatusCode);
            var responseContent = await response.Content.ReadFromJsonAsync<CertificateApplication>(cancellationToken: cancellationToken);
            var result = responseContent?.ToApplication();
            if (result != null)
            {
                result.SupportingDocumentsSection.SupportingDocuments = await GetSupportingDocumentsForApplicationAsync(id, user, cancellationToken);
            }
            return result;
        }

        /// <inheritdoc />
        public async Task<ReadOnlyCollection<Application>> GetAllForUserAsync(ExternalUser user, bool retrieveSupportingDocuments, CancellationToken cancellationToken = default)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await AuthenticateAsync();

            var url = $"{_configuration.ApiUrl}api/data/v9.1/{CertificateApplication.EntityName}?$filter=cr671_applicantprofessionaloperatornumber eq '{user.ProfessionalOperatorNumber}'";

            _logger.LogDebug("Attempting to retrieve Applications from Dataverse for professional operator number {ProfessionalOperatorNumber} at endpoint {url}", user.ProfessionalOperatorNumber, url);

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode == false)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken: cancellationToken);
                _logger.LogError("Received unsuccessful response with code {StatusCode} and message {Error}", response.StatusCode, error?.Error?.Message);
                return new ReadOnlyCollection<Application>(new List<Application>(0));
            }

            var responseContent = await response.Content.ReadFromJsonAsync<QueryMultipleResponseModel<CertificateApplication>>(cancellationToken: cancellationToken);
            var models = responseContent?.Values.Select(x => x.ToApplication()).ToList() ?? new List<Application>(0);
            _logger.LogDebug("Received successful response with code {StatusCode} and {Count} applications", response.StatusCode, models.Count);

            if (retrieveSupportingDocuments)
            {
                foreach (var model in models)
                {
                    model.SupportingDocumentsSection.SupportingDocuments =
                        await GetSupportingDocumentsForApplicationAsync(model.Identifier!, user, cancellationToken);
                }
            }

            return new ReadOnlyCollection<Application>(models);
        }

        /// <inheritdoc />
        public async Task<Result<Application>> UpsertAsync(Application application, ExternalUser user, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(application.Identifier))
            {
                return await PostAsync(application, user, cancellationToken);
            }

            var patchResult = await PatchAsync(application, cancellationToken);
            return patchResult.IsSuccess
                ? Result.Success(application)
                : patchResult.ConvertFailure<Application>();
        }

        public async Task<Result<string>> StoreSupportingDocumentContentAsync(string applicationIdentifier, ExternalUser user, string fileName, string mimeType, byte[] documentContentBytes, CancellationToken cancellationToken = default)
        {
            var documentBody = Convert.ToBase64String(documentContentBytes);

            var model = new Annotation
            {
                ApplicationIdentifier = $"/{CertificateApplication.EntityName}({applicationIdentifier})",
                FileName = fileName,
                MimeType = mimeType,
                DocumentBody = documentBody,
                Subject = $"Uploaded supporting document {fileName} by user {user.ProfessionalOperatorNumber}",
                NoteText = $"Uploaded supporting document {fileName}"
            };

            var jsonBody = JsonSerializer.Serialize(model);

            await AuthenticateAsync();

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var url = $"{_configuration.ApiUrl}api/data/v9.1/annotations";

            _logger.LogDebug("Attempting to post new Annotation entity to endpoint {url}", url);

            var response = await _httpClient.PostAsync(url, content, cancellationToken);

            if (response.IsSuccessStatusCode == false)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken: cancellationToken);
                _logger.LogError("Received unsuccessful response with code {StatusCode} and message {Error}", response.StatusCode, error?.Error?.Message);
                return Result.Failure<string>(error?.Error?.Message);
            }

            var entityPath = response.Headers.GetValues("OData-EntityId").Single();
            var entityId = entityPath.Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries)[1];

            _logger.LogDebug("Received successful response with code {StatusCode} and new entity id {EntityId}", response.StatusCode, entityId);

            return Result.Success(entityId);
        }

        public async Task<Result> DeleteSupportingDocumentAsync(string documentIdentifier, CancellationToken cancellationToken = default)
        {
            await AuthenticateAsync();

            var url = $"{_configuration.ApiUrl}api/data/v9.1/annotations({documentIdentifier})";
            _logger.LogDebug("Attempting to delete Annotation entity with id {AnnotationIdentifier}", documentIdentifier);

            var response = await _httpClient.DeleteAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode == false)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken: cancellationToken);
                _logger.LogError("Received unsuccessful response with code {StatusCode} and message {Error}", response.StatusCode, error?.Error?.Message);
                return Result.Failure(error?.Error?.Message);
            }

            return Result.Success();
        }

        private async Task<Result<Application>> PostAsync(Application application, ExternalUser externalUser, CancellationToken cancellationToken)
        {
            var repositoryModel = application.ToCertificateApplication();
            repositoryModel.CreatedByContact = externalUser.LocalProfileId;

            var jsonBody = JsonSerializer.Serialize(repositoryModel);

            await AuthenticateAsync();

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            content.Headers.TryAddWithoutValidation("Prefer", "return=representation"); // this header tells the OData endpoint to return the JSON for the created entity to us in the response payload
            var url = $"{_configuration.ApiUrl}api/data/v9.1/{CertificateApplication.EntityName}";

            _logger.LogDebug("Attempting to post new Application entity to endpoint {url}", url);

            var response = await _httpClient.PostAsync(url, content, cancellationToken);

            if (response.IsSuccessStatusCode == false)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken: cancellationToken);
                _logger.LogError("Received unsuccessful response with code {StatusCode} and message {Error}", response.StatusCode, error?.Error?.Message);
                return Result.Failure<Application>(error?.Error?.Message);
            }
            
            var createdCrmEntity = await JsonSerializer.DeserializeAsync<CertificateApplication>(
                await response.Content.ReadAsStreamAsync(cancellationToken), 
                cancellationToken: cancellationToken);

            _logger.LogDebug("Received successful response with code {StatusCode} and new entity id {EntityId}", response.StatusCode, createdCrmEntity!.Identifier);
            var result = createdCrmEntity.ToApplication();
            return Result.Success(result);
        }

        private async Task<Result> PatchAsync(Application application, CancellationToken cancellationToken)
        {
            var repositoryModel = application.ToCertificateApplication();
            var jsonBody = JsonSerializer.Serialize(repositoryModel);

            await AuthenticateAsync();

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var url = $"{_configuration.ApiUrl}api/data/v9.1/{CertificateApplication.EntityName}({application.Identifier})";

            _logger.LogDebug("Attempting to patch Application entity with id {id} to endpoint {url}", application.Identifier, url);

            var response = await _httpClient.PatchAsync(url, content, cancellationToken);

            if (response.IsSuccessStatusCode == false)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken: cancellationToken);
                _logger.LogError("Received unsuccessful response with code {StatusCode} and message {Error}", response.StatusCode, error?.Error?.Message);
                return Result.Failure(error?.Error?.Message);
            }

            _logger.LogDebug("Received successful response with code {StatusCode}", response.StatusCode);
            return Result.Success();
        }

        private async Task AuthenticateAsync()
        {
            var authenticationResult = await _authenticationHandler.AuthenticateAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult!.AccessToken);
        }

        private async Task<ReadOnlyCollection<SupportingDocument>> GetSupportingDocumentsForApplicationAsync(string applicationIdentifier, ExternalUser user, CancellationToken cancellationToken = default)
        {
            await AuthenticateAsync();

            var url = $"{_configuration.ApiUrl}api/data/v9.1/annotations?$filter=isdocument eq true and objecttypecode eq %27{CertificateApplication.EntityNameSingular}%27 and _objectid_value eq %27{applicationIdentifier}%27 and endswith(subject, '{user.ProfessionalOperatorNumber}')";

            _logger.LogDebug("Attempting to retrieve Annotations from Dataverse for certificate application id {ApplicationIdentifier} at endpoint {url}", applicationIdentifier, url);

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode == false)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken: cancellationToken);
                _logger.LogError("Received unsuccessful response with code {StatusCode} and message {Error}", response.StatusCode, error?.Error?.Message);
                return new ReadOnlyCollection<SupportingDocument>(new List<SupportingDocument>(0));
            }

            var responseContent = await response.Content.ReadFromJsonAsync<QueryMultipleResponseModel<Annotation>>(cancellationToken: cancellationToken);
            var models = responseContent?.Values.Select(x => x.ToSupportingDocument()).ToList() ?? new List<SupportingDocument>(0);
            _logger.LogDebug("Received successful response with code {StatusCode} and {Count} annotations", response.StatusCode, models.Count);

            return new ReadOnlyCollection<SupportingDocument>(models);
        }
    }
}