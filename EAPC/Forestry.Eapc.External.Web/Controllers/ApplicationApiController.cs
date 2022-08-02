using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Configuration;
using Forestry.Eapc.External.Web.Services;
using Forestry.Eapc.External.Web.Services.Certificate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Forestry.Eapc.External.Web.Controllers
{
    [Route("api/applications/{applicationId}")]
    [ApiController]
    public class ApplicationApiController : ControllerBase
    {
        private readonly IApplicationUnattendedRepository _applicationRepository;
        private readonly CertificateGenerationProxy _proxy;
        private readonly IOptions<ApiSecuritySettings> _securityOptions;
        private readonly ILogger<ApplicationApiController> _logger;

        public ApplicationApiController(
            IApplicationUnattendedRepository applicationRepository,
            CertificateGenerationProxy proxy,
            IOptions<ApiSecuritySettings> securityOptions,
            ILogger<ApplicationApiController> logger)
        {
            _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
            _proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
            _securityOptions = securityOptions ?? throw new ArgumentNullException(nameof(securityOptions));
            _logger = logger ?? new NullLogger<ApplicationApiController>();
        }

        public async Task<IActionResult> CreatePdf(
            [FromRoute] string applicationId,
            [FromQuery] string? signatureName,
            [FromQuery] string? watermark,
            CancellationToken cancellationToken)
        {
            if (!CheckAuthorizedRequest())
            {
                _logger.LogError("Unauthorized API request blocked from remote IP address {RemoteIpAddress}", HttpContext.Connection.RemoteIpAddress);
                return Unauthorized();
            }

            var application = await _applicationRepository.GetByIdAsync(applicationId, cancellationToken);
            if (application == null)
            {
                _logger.LogWarning("The repository found no application matching id {ApplicationId}", applicationId);
                return Problem("No matching certificate application found.", statusCode: (int) HttpStatusCode.Gone, title: "Not Found");
            }

            var parameters = new CertificateGenerationParameters(signatureName, watermark);
            var certificatePdf = await _proxy.GetDraftCertificateAsync(application, parameters, cancellationToken);
            return File(certificatePdf, "application/pdf");
        }

        private bool CheckAuthorizedRequest()
        {
            var securityOptions = _securityOptions.Value;
            var expectedHeaderValue = securityOptions.AuthenticationHeaderValue;
            
            if (string.IsNullOrWhiteSpace(expectedHeaderValue))
            {
                _logger.LogWarning("No expected security header value has been set so allowing API request.");
                return true;
            }
            
            var headerValue = Request.Headers[securityOptions.AuthenticationHeaderKey].SingleOrDefault();
            if (!expectedHeaderValue.Equals(headerValue, StringComparison.Ordinal))
            {
                _logger.LogWarning(
                    "{Value '{HeaderValue}' read from HTTP Header {HeaderKey} does not match expected configured value",
                    headerValue, securityOptions.AuthenticationHeaderKey);

                return false;
            }

            return true;
        }
    }
}
