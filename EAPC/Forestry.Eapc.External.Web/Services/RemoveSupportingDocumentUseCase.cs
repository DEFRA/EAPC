using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Forestry.Eapc.External.Web.Services
{
    public class RemoveSupportingDocumentUseCase
    {
        private readonly ISupportingDocumentRepository _repository;
        private readonly ILogger<RemoveSupportingDocumentUseCase> _logger;

        public RemoveSupportingDocumentUseCase(ISupportingDocumentRepository repository, ILogger<RemoveSupportingDocumentUseCase> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> RemoveSupportingDocument(
            string documentIdentifier,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("Beginning RemoveSupportingDocument use case for document with id {DocumentIdentifier}", documentIdentifier);
            var result = await _repository.DeleteSupportingDocumentAsync(documentIdentifier, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Document with id {DocumentIdentifier} failed to be removed with error {Error}", documentIdentifier, result.Error);
                return Result.Failure("The supporting document could not be deleted at this time, please try again.");
            }

            return Result.Success();
        }
    }
}