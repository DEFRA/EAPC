using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Forestry.Eapc.External.Web.Models.Application;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace Forestry.Eapc.External.Web.Services.Repositories
{
    /// <summary>
    /// This is a temporary implementation of the <see cref="IApplicationRepository"/> interface in lieu of having the instance
    /// that connects to DataVerse.
    /// </summary>
    public class InMemoryApplicationRepository : IApplicationRepository
    {
        private readonly ConcurrentDictionary<string, Application> _applications = new();
        private readonly JsonSerializerOptions _jsonSerializerOptions = new();

        public InMemoryApplicationRepository()
        {
            _jsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        }
        
        /// <inheritdoc />
        public Task<Application?> GetByIdAsync(string id, ExternalUser user, CancellationToken cancellationToken = default)
        {
            if (!_applications.TryGetValue(id, out var result))
            {
                return Task.FromResult<Application?>(null);
            }

            if (result.Applicant.ProfessionalOperatorNumber != user.ProfessionalOperatorNumber)
            {
                return Task.FromResult<Application?>(null);
            }

            return Task.FromResult<Application?>(result);
        }

        /// <inheritdoc />
        public Task<Result<Application>> UpsertAsync(Application application, ExternalUser user, CancellationToken cancellationToken = default)
        {
            // create a copy of the application by serializing and deserializing from JSON
            var json = JsonSerializer.Serialize(application, _jsonSerializerOptions);
            var result = JsonSerializer.Deserialize<Application>(json, _jsonSerializerOptions);

            if (result!.Identifier == null)
            {
                result.Identifier = Guid.NewGuid().ToString();
                result.ReferenceIdentifier = $"fake/identifier/{_applications.Count + 1}";
            }
            
            _applications.AddOrUpdate(result.Identifier, result, (_, _) => result);
            return Task.FromResult(Result.Success(result));
        }

        /// <inheritdoc />
        public Task<ReadOnlyCollection<Application>> GetAllForUserAsync(ExternalUser user, bool retrieveSupportingDocuments, CancellationToken cancellationToken = default)
        {
            var applications = _applications
                .Values
                .Where(x => x.Applicant.ProfessionalOperatorNumber == user.ProfessionalOperatorNumber)
                .ToList()
                .AsReadOnly();

            return Task.FromResult(applications);
        }
    }
}
