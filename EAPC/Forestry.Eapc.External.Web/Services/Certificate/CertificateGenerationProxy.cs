using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Forestry.Eapc.External.Web.Configuration;
using Forestry.Eapc.External.Web.Models.Application;
using Microsoft.Extensions.Options;

namespace Forestry.Eapc.External.Web.Services.Certificate
{
    public class CertificateGenerationProxy
    {
        private const string NewExportEndpoint = "generateCertificate";
        private const string ReforwardingEndpoint = "generateReExportCertificate";

        private readonly HttpClient _client;
        private readonly IOptions<PhytoCertificatePreviewSettings> _settings;

        public CertificateGenerationProxy(HttpClient client, IOptions<PhytoCertificatePreviewSettings> settings)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<Stream> GetDraftCertificateAsync(
            Application application,
            CertificateGenerationParameters parameters,
            CancellationToken cancellationToken = default)
        {
            var url = application.Applicant.ExportStatus == ExportStatus.Reforwarded
                ? ReforwardingEndpoint
                : NewExportEndpoint;
            
            var model = ModelFactory.CreateRequestModel(application, parameters, _settings.Value);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Defines the parameters to use when generating a certificate that will override the default configured values.
    /// </summary>
    public record CertificateGenerationParameters(string? SignatureName = null, string? Watermark = null);
}
