using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Forestry.Eapc.External.Web.Models.Application;
using Forestry.Eapc.External.Web.Services;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace Forestry.Eapc.External.Web.Tests.Fakes
{
    public class FakeApplicationRepository : IApplicationRepository
    {
        private readonly List<Application> _applications = new();

        /// <inheritdoc />
        public Task<Result<Application>> UpsertAsync(Application application, ExternalUser user, CancellationToken cancellationToken = default)
        {
            // create a copy of the provided application and then set a new id value
            var options = new JsonSerializerOptions();
            options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            var json = JsonSerializer.Serialize(application, options);
            var result = JsonSerializer.Deserialize<Application>(json, options);
            result!.Identifier = Guid.NewGuid().ToString();
            result.ReferenceIdentifier = Guid.NewGuid().ToString();

            _applications.Add(result);
            return Task.FromResult(Result.Success(result));
        }

        /// <inheritdoc />
        public Task<Application?> GetByIdAsync(string id, ExternalUser user, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ReadOnlyCollection<Application>> GetAllForUserAsync(ExternalUser user, bool retrieveSupportingDocuments, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<Application> GetAll() => _applications.AsReadOnly();
    }
}
