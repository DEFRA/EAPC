using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Forestry.Eapc.External.Web.Infrastructure
{
    public class NotLoggedInTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotLoggedInTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <inheritdoc />
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null; // stops our own tag name <not-logged-in> being written to the browse
            var user = _httpContextAccessor.HttpContext?.User;
            
            if (user != null && user.IsLoggedIn())
            {
                output.SuppressOutput();
            }
        }
    }
}
