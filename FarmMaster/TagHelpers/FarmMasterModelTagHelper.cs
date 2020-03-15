using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace FarmMaster.TagHelpers
{
    [HtmlTargetElement("textbox")]
    public class FarmMasterModelTagHelper : TagHelper
    {
        public ModelExpression FmModel { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if(this.FmModel == null)
                return;

            output.Attributes.Add("name", this.FmModel.Name);
            output.Attributes.Add("value", this.FmModel.Model);
        }
    }
}
