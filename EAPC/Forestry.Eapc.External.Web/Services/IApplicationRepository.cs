using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Forestry.Eapc.External.Web.Models.Application;

namespace Forestry.Eapc.External.Web.Services
{
    public interface IApplicationRepository
    {
        Task<Application?> GetByIdAsync(string id, ExternalUser user, CancellationToken cancellationToken = default);

        Task<ReadOnlyCollection<Application>> GetAllForUserAsync(ExternalUser user, bool retrieveSupportingDocuments, CancellationToken cancellationToken = default);
        
        Task<Result<Application>> UpsertAsync(Application application, ExternalUser user, CancellationToken cancellationToken = default);
    }
}
