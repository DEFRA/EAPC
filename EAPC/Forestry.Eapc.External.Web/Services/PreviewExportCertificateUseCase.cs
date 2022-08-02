using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Services.Certificate;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Forestry.Eapc.External.Web.Services
{
    public class PreviewExportCertificateUseCase
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly CertificateGenerationProxy _proxy;
        private readonly ILogger<PreviewExportCertificateUseCase> _logger;

        public PreviewExportCertificateUseCase(
            IApplicationRepository applicationRepository,
            CertificateGenerationProxy proxy,
            ILogger<PreviewExportCertificateUseCase> logger)
        {
            _applicationRepository = applicationRepository;
            _proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
            _logger = logger ?? new NullLogger<PreviewExportCertificateUseCase>();
        }

        public async Task<Stream> GetCertificateAsync(string applicationId, ExternalUser user, CancellationToken cancellationToken = default)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId, user, cancellationToken);

            if (application == null)
            {
                _logger.LogError(
                    "Unable to locate an application with identifier {ApplicationId} for user with Professional Operator No {ProfessionalOperatorNumber}", 
                    applicationId, 
                    user.ProfessionalOperatorNumber);

                throw new ApplicationNotFoundException(
                    $"Unable to locate application {applicationId} for user with Professional Operator No {user.ProfessionalOperatorNumber}");
            }

            var parameters = new CertificateGenerationParameters {Watermark = "DRAFT"};
            var certificatePdf = await _proxy.GetDraftCertificateAsync(application, parameters, cancellationToken);
            return certificatePdf;
        }
    }
}
