using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Forestry.Eapc.External.Web.Infrastructure.SecurityHeaders
{
    /// <summary>
    /// An ASP.NET middleware for adding security headers.
    /// Sourced from: https://andrewlock.net/adding-default-security-headers-in-asp-net-core/
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SecurityHeadersPolicy _policy;

        /// <summary>
        /// Instantiates a new <see cref="SecurityHeadersMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="policy">An instance of the <see cref="SecurityHeadersPolicy"/> which can be applied.</param>
        public SecurityHeadersMiddleware(RequestDelegate next, SecurityHeadersPolicy policy)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            _next = next;
            _policy = policy;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.Response;

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var headers = response.Headers;

            foreach (var headerValuePair in _policy.SetHeaders)
            {
                headers[headerValuePair.Key] = headerValuePair.Value;
            }

            foreach (var header in _policy.RemoveHeaders)
            {
                headers.Remove(header);
            }

            await _next(context);
        }

    }
}