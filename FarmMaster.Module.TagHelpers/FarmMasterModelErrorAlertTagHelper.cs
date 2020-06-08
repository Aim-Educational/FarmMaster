using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;

namespace FarmMaster.TagHelpers
{
    [HtmlTargetElement("alert")]
    public class FarmMasterModelErrorAlertTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public bool FmModelError { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!this.FmModelError || this.ViewContext.ModelState.IsValid)
                return;

            var state = this.ViewContext.ModelState;
            var errors = state.Values.SelectMany(v => v.Errors);

            if (!errors.Any())
                return;

            output.Attributes.Add("show", true);
            output.Attributes.Add("type", "error");

            if (errors.Count() > 1)
            {
                var ul = new TagBuilder("ul");
                foreach (var error in errors)
                {
                    var li = new TagBuilder("li");
                    li.InnerHtml.Append(error.ErrorMessage);
                    ul.InnerHtml.AppendHtml(li);
                }

                output.PostContent.AppendHtml(ul);
            }
            else
            {
                var p = new TagBuilder("p");
                p.InnerHtml.Append(errors.First().ErrorMessage);
                output.PostContent.AppendHtml(p);
            }
        }
    }
}
