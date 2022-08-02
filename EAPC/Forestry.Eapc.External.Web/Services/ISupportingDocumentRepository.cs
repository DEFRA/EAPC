using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Forestry.Eapc.External.Web.Services
{
    public interface ISupportingDocumentRepository
    {
        Task<Result<string>> StoreSupportingDocumentContentAsync(string applicationIdentifier, ExternalUser user, string fileName, string mimeType, byte[] documentContentBytes, CancellationToken cancellationToken = default);

        Task<Result> DeleteSupportingDocumentAsync(string documentIdentifier, CancellationToken cancellationToken = default);
    }
}