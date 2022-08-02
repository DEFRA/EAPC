using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Forestry.Eapc.External.Web.Infrastructure
{
    [HtmlTargetElement("input", Attributes = DisabledAttributeName)]
    public class InputTagHelper : TagHelper
    {
        private const string DisabledAttributeName = "asp-disabled";

        [HtmlAttributeName(DisabledAttributeName)]
        public bool Disabled { get; set; }

        /// <inheritdoc />
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (Disabled)
            {
                output.Attributes.Add("disabled", "disabled");
            }
        }
    }
}
