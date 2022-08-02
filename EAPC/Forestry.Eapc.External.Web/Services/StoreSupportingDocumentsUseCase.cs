using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Forestry.Eapc.External.Web.Configuration;
using Forestry.Eapc.External.Web.Models.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Forestry.Eapc.External.Web.Services
{
    public class StoreSupportingDocumentsUseCase
    {
        private readonly ISupportingDocumentRepository _repository;
        private readonly SupportingDocumentsSettings _settings;
        private readonly ILogger<StoreSupportingDocumentsUseCase> _logger;

        public StoreSupportingDocumentsUseCase(ISupportingDocumentRepository repository, IOptions<SupportingDocumentsSettings> settings, ILogger<StoreSupportingDocumentsUseCase> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _settings = settings?.Value ?? new SupportingDocumentsSettings();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> StoreSupportingDocuments(
            Application application,
            ExternalUser user,
            IFormFileCollection supportingDocuments, 
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("Beginning StoreSupportingDocuments use case.");
            if (!supportingDocuments.Any())
            {
                _logger.LogDebug("No files provided, returning Success.");
                return Result.Success();
            }

            if (application.SupportingDocumentsSection.SupportingDocuments.Count + supportingDocuments.Count > _settings.MaxNumberDocuments)
            {
                _logger.LogDebug("Too many files provided, returning Failure.");
                return Result.Failure($"A maximum of {_settings.MaxNumberDocuments} documents may be attached to an application." );
            }

            var allSupportedExtensions = _settings.AllowedFileTypes.SelectMany(x => x.Extensions);
            var uploadedFileExtensions = supportingDocuments.Select(x => x.FileName.Substring(x.FileName.LastIndexOf('.')+1));

            if (uploadedFileExtensions.Any(x => !allSupportedExtensions.Any(y => y.Equals(x, StringComparison.InvariantCultureIgnoreCase))))
            {
                _logger.LogDebug("Files with invalid file types provided, returning Failure.");
                return Result.Failure("Only documents of the permitted file types may be attached to an application.");
            }

            bool storageFailed = false;
            bool fileTooBig = false;

            foreach (var supportingDocument in supportingDocuments)
            {
                await using var ms = new MemoryStream();
                await supportingDocument.OpenReadStream().CopyToAsync(ms, cancellationToken);
                var fileBytes = ms.ToArray();

                if (fileBytes.Length > _settings.MaxFileSizeBytes)
                {
                    _logger.LogDebug("Document {FileName} provided is {FileSize} bytes which is above maximum size, ignoring file.", supportingDocument.FileName, fileBytes.Length);
                    fileTooBig = true;
                    continue;
                }

                var storeDocResult = await _repository.StoreSupportingDocumentContentAsync(
                    application.Identifier!,
                    user,
                    supportingDocument.FileName,
                    supportingDocument.ContentType,
                    fileBytes,
                    cancellationToken);

                if (storeDocResult.IsFailure)
                {
                    _logger.LogError("Document {FileName} failed to upload with error {Error}", supportingDocument.FileName, storeDocResult.Error);
                    storageFailed = true;
                }
            }

            var error = string.Empty;
            if (storageFailed)
            {
                error += "One or more documents could not be uploaded, please try again. ";
            }

            if (fileTooBig)
            {
                error += $"One or more of the provided files was larger than the maximum permitted size ({FileSizeToStringConverter.SizeSuffix(_settings.MaxFileSizeBytes)}).";
            }

            return string.IsNullOrEmpty(error)
                ? Result.Success()
                : Result.Failure(error);
        }
    }
}