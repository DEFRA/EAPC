using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Forestry.Eapc.External.Web.Configuration;
using Forestry.Eapc.External.Web.Infrastructure.User;
using Forestry.Eapc.External.Web.Models.Repository;
using Forestry.Eapc.External.Web.Services.Repositories.DataVerse;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Forestry.Eapc.External.Web.Services.Repositories.Users
{
    /// <summary>
    /// Implementation of the <see cref="ILocalUserRepository"/> contract that uses Dataverse contact entities
    /// to store local profile data for users.
    /// </summary>
    public class DataverseUserRepository : PowerappsRepositoryBase, ILocalUserRepository
    {
        private const string ContactEntityName = "contacts";
        private const string ClaimsIdentityReference = "dataverse";
        private readonly ILogger<PowerappsRepositoryBase> _logger;

        /// <inheritdoc />
        public DataverseUserRepository(
            IHttpClientFactory httpClientFactory, 
            ConfidentialClientApplicationAuthentication authenticationHandler, 
            IOptions<EapcEnvironmentSettings> configuration, 
            ILogger<PowerappsRepositoryBase> logger) 
            : base(httpClientFactory, authenticationHandler, configuration.Value.PowerappsAuthentication, logger)
        {
            _logger = logger ?? new NullLogger<PowerappsRepositoryBase>();
        }

        /// <inheritdoc />
        public async Task HandleUserLoginAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
        {
            var userIdentifier = user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdentifier))
            {
                return;
            }

            var model = await FindContactByIdentityProviderIdAsync(userIdentifier, cancellationToken);

            if (model != null)
            {
                // we found a match to a contact, so map contact model to a new claims identity and add it to our result
                var claimsIdentity = CreateClaimsIdentityFromContactModel(model);
                user.AddIdentity(claimsIdentity);
            }
        }
        
        /// <inheritdoc />
        public async Task<ExternalUser> SaveProfileDataAsync(ExternalUser externalUser, CancellationToken cancellationToken = default)
        {
            var dataverseModel = new DataverseContactModel
            {
                FirstName = externalUser.GivenName,
                LastName = externalUser.Surname,
                AddressLine1 = externalUser.StreetAddressLine1,
                AddressLine2 = externalUser.StreetAddressLine2,
                AddressLine3 = externalUser.StreetAddressLine3,
                AddressLine4 = externalUser.StreetAddressLine4,
                PostalCode = externalUser.PostalCode,
                Telephone = externalUser.Telephone,
                CreditAccountReference = externalUser.CreditAccountReference,
                IsCreditTermsAndConditionsSignup = externalUser.SignedUpToCreditTermsAndConditions,
                CompanyName = externalUser.CompanyName,
                HomeNation = externalUser.HomeNation.HasValue ? MapEnum<HomeNation>(externalUser.HomeNation.Value) : null,
                ProfessionalOperatorNumber = externalUser.ProfessionalOperatorNumber,
                IdentityProviderId = externalUser.IdentityProviderId,
                DataVerseId = externalUser.LocalProfileId,
                IsApprovedByProfessionalOperator = externalUser.IsApprovedAccount,
                EmailAddress = externalUser.Email,
                RegistrationConfirmationEmailTarget = externalUser.RegistrationEmailRecipient
            };

            var baseUrl = CreateEntityPathString(ContactEntityName);
            var request = new HttpRequestMessage();
            JsonSerializerOptions options = new();

            // this should only be possible if the user is doing something odd like logging in in two browsers at the same time?
            if (string.IsNullOrWhiteSpace(dataverseModel.DataVerseId) && !string.IsNullOrWhiteSpace(externalUser.IdentityProviderId))
            {
                // double check there's no profile for the given b2c provider id
                var existing = await FindContactByIdentityProviderIdAsync(externalUser.IdentityProviderId, cancellationToken);

                if (existing != null)
                {
                    dataverseModel.DataVerseId = existing.DataVerseId;
                }
            }

            if (string.IsNullOrWhiteSpace(dataverseModel.DataVerseId))
            {
                // new contact entity
                request.RequestUri = new Uri(baseUrl);
                request.Method = HttpMethod.Post;
                options.IgnoreNullValues = true; // this ensures for new entity requests we don't serialize the DataVerseId property of the model to the request body
            }
            else
            {
                // update existing contact entity
                var url = $"{baseUrl}({dataverseModel.DataVerseId})";
                request.RequestUri = new Uri(url);
                request.Method = HttpMethod.Patch;
            }

            var jsonBody = JsonSerializer.Serialize(dataverseModel, new JsonSerializerOptions {IgnoreNullValues = true});
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            SetRequestToAskForDataRepresentationInResponse(request);

            var response = await SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Unable to update profile data at contacts endpoint as response was status code {HttpStatusCode}", response.StatusCode);
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("OData Contacts endpoint response content was {ErrorResponseContext}", responseBody);
                
                throw new RepositoryException($"Received a HTTP status code {response.StatusCode} response from the OData contacts endpoint");
            }

            // read the response body into a DataverseContactModel instance
            var crmEntityFromResponse = await JsonSerializer.DeserializeAsync<DataverseContactModel>(
                await response.Content.ReadAsStreamAsync(cancellationToken), 
                cancellationToken: cancellationToken);
            
            // create a list of claims and initialize it with a claims identity built from the DataverseContactModel we got from the response
            List<ClaimsIdentity> identities = new()
            {
                CreateClaimsIdentityFromContactModel(crmEntityFromResponse!)
            };

            // copy any ClaimsIdentity instances from the provided external user that did not originate from this class
            identities.AddRange(externalUser.Principal.Identities.Where(x => x.AuthenticationType != ClaimsIdentityReference));

            // return a new ExternalUser instance with the updated claims based on data returned from Dataverse
            return new ExternalUser(new ClaimsPrincipal(identities));

            static T MapEnum<T>(Enum value) where T: Enum
            {
                T result = (T) Enum.Parse(typeof(T), value.ToString());
                return result;
            }
        }

        /// <inheritdoc />
        public async Task<bool> ApproveUserAccountAsync(string email, string professionalOperatorNumber, ExternalUser externalUser, CancellationToken cancellationToken)
        {
            var model = await FindContactByEmailIdAsync(email, professionalOperatorNumber, cancellationToken);

            if (model == null)
            {
                return false;
            }

            if (model.IsApprovedByProfessionalOperator)
            {
                _logger.LogInformation(
                    "Not sending approve account request for account {DataVerseId} and email {Email} as it is already approved",
                    model.DataVerseId,
                    model.EmailAddress);
                
                return true;
            }

            // send the request to update the Contact entity
            var baseUrl = CreateEntityPathString(ContactEntityName);
            var url = $"{baseUrl}({model.DataVerseId})";
            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            var updateModel = new DataverseApproveContactModel {IsApprovedByProfessionalOperator = true};
            var jsonBody = JsonSerializer.Serialize(updateModel, new JsonSerializerOptions {IgnoreNullValues = true});
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Unable to update profile data at contacts endpoint as response was status code {HttpStatusCode}", response.StatusCode);
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("OData Contacts endpoint response content was {ErrorResponseContext}", responseBody);
                
                throw new RepositoryException($"Received a HTTP status code {response.StatusCode} response from the OData contacts endpoint");
            }

            return true;
        }

        private async Task<DataverseContactModel?> FindContactByIdentityProviderIdAsync(string identityProviderId, CancellationToken cancellationToken)
        {
            var baseUrl = CreateEntityPathString(ContactEntityName);
            var searchString = $"?$filter=cr671_identityproviderid eq '{identityProviderId}'";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, baseUrl + searchString);
            var response = await base.SendAsync(requestMessage, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Unable to query details from contacts endpoint to locate user by identity provider id as response was status code {HttpStatusCode}", response.StatusCode);
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("OData Contacts endpoint response content was {ErrorResponseContext}", responseBody);
                
                throw new RepositoryException($"Received a HTTP status code {response.StatusCode} response from the OData contacts endpoint");
            }

            var responseContent = await response.Content.ReadFromJsonAsync<QueryMultipleResponseModel<DataverseContactModel>>(cancellationToken: cancellationToken);
            _logger.LogDebug("Received a count of {ContactCount} contacts matching identity provided id {IdentityProviderId}", responseContent?.Values.Length, identityProviderId);
            return responseContent?.Values.SingleOrDefault();
        }

        private async Task<DataverseContactModel?> FindContactByEmailIdAsync(string email, string professionalOperatorNumber, CancellationToken cancellationToken)
        {
            var baseUrl = CreateEntityPathString(ContactEntityName);
            var searchString = $"?$filter=emailaddress1 eq '{HttpUtility.UrlEncode(email)}' and cr671_professionaloperatornumber eq '{professionalOperatorNumber}'";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, baseUrl + searchString);
            var response = await base.SendAsync(requestMessage, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Unable to query details from contacts endpoint to locate user by email and professional operator number as response was status code {HttpStatusCode}", response.StatusCode);
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("OData Contacts endpoint response content was {ErrorResponseContext}", responseBody);
                
                throw new RepositoryException($"Received a HTTP status code {response.StatusCode} response from the OData contacts endpoint");
            }

            var responseContent = await response.Content.ReadFromJsonAsync<QueryMultipleResponseModel<DataverseContactModel>>(cancellationToken: cancellationToken);
            _logger.LogDebug("Received a count of {ContactCount} contacts matching email {Email} and professional operator number {ProfessionalOperatorNumber}", responseContent?.Values.Length, email, professionalOperatorNumber);
            return responseContent?.Values.SingleOrDefault();
        }

        private ClaimsIdentity CreateClaimsIdentityFromContactModel(DataverseContactModel model)
        {
            var claims = new List<Claim>();
            AddIfNotNull(claims, EapcClaimTypes.DataVerseInstanceId, model.DataVerseId);
            AddIfNotNull(claims, ClaimTypes.GivenName, model.FirstName);
            AddIfNotNull(claims, ClaimTypes.Surname, model.LastName);
            AddIfNotNull(claims, ClaimTypes.StreetAddress, model.AddressLine1);
            AddIfNotNull(claims, EapcClaimTypes.StreetAddressLine2, model.AddressLine2);
            AddIfNotNull(claims, EapcClaimTypes.StreetAddressLine3, model.AddressLine3);
            AddIfNotNull(claims, EapcClaimTypes.StreetAddressLine4, model.AddressLine4);
            AddIfNotNull(claims, ClaimTypes.PostalCode, model.PostalCode);
            AddIfNotNull(claims, ClaimTypes.OtherPhone, model.Telephone);
            AddIfNotNull(claims, EapcClaimTypes.ProfessionalOperatorNumber, model.ProfessionalOperatorNumber);
            AddIfNotNull(claims, EapcClaimTypes.CreditAccountReference, model.CreditAccountReference);
            AddIfNotNull(claims, EapcClaimTypes.CompanyName, model.CompanyName);
            AddIfNotNull(claims, EapcClaimTypes.HomeNation, model.HomeNation.ToString());
            AddIfNotNull(claims, EapcClaimTypes.ApprovedAccount, model.IsApprovedByProfessionalOperator.ToString());
            AddIfNotNull(claims, EapcClaimTypes.HasAgreedCreditTermsAndConditions, model.IsCreditTermsAndConditionsSignup.ToString());
            AddIfNotNull(claims, EapcClaimTypes.RegistrationConfirmationEmailRecipient, model.RegistrationConfirmationEmailTarget);
            
            var result = new ClaimsIdentity(claims, ClaimsIdentityReference);
            return result;
        }

        private void AddIfNotNull(List<Claim> list, string claimType, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _logger.LogDebug("Skipping claim {ClaimType} as no value was found within Dataverse", claimType);
            }
            else
            {
                _logger.LogDebug("Adding claim {ClaimType} as with value {ClaimValue} read from Dataverse", claimType, value);
                list.Add(new Claim(claimType, value));    
            }
        }
    }
}
