using System;
using Forestry.Eapc.External.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Forestry.Eapc.External.Web.Infrastructure
{
    public class KeyContactTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KeyContactTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <inheritdoc />
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null; // stops our own tag name <key-contact> being written to the browse
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || new ExternalUser(user).IsProfessionalOperatorKeyContact == false)
            {
                output.SuppressOutput();
            }
        }
    }
}
