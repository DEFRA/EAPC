using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Forestry.Eapc.External.Web.Infrastructure
{
    public class LoggedInTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggedInTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <inheritdoc />
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null; // stops our own tag name <logged-in> being written to the browse
            var user = _httpContextAccessor.HttpContext?.User;
            
            if (user == null || user.IsNotLoggedIn())
            {
                output.SuppressOutput();
            }
        }
    }
}
