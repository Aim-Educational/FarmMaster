using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace FarmMaster.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-for")]
    public class FarmMasterAspForTagHelper : TagHelper
    {
        static readonly string[] VALIDATION_CLASSES        = { "needs", "validation" };
        static readonly string   VALIDATION_DATA_ATTRIBUTE = "data-validation-rules";
        static readonly string   RULE_DELIMINIATOR         = "¬";

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        private List<string> ValidationRules;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);
            
            this.ValidationRules = new List<string>();
            foreach(var metadata in this.For.ModelExplorer.Metadata.ValidatorMetadata)
            {
                if(metadata is RequiredAttribute)
                    this.HandleRequired(context, output);
                else if(metadata is RegularExpressionAttribute)
                    this.HandleRegex(context, output, metadata as RegularExpressionAttribute);
            }

            if(this.ValidationRules.Count > 0)
                output.Attributes.Add(VALIDATION_DATA_ATTRIBUTE, this.ValidationRules.Aggregate((s1, s2) => $"{s1}{RULE_DELIMINIATOR}{s2}"));
        }

        void AddClasses(TagHelperOutput output)
        {
            foreach(var @class in VALIDATION_CLASSES)
                output.AddClass(@class, HtmlEncoder.Default);
        }

        void HandleRequired(TagHelperContext context, TagHelperOutput output)
        {
            this.AddClasses(output);

            if (this.For.Metadata.ModelType == typeof(bool))
                this.ValidationRules.Add("checked");
            else if (this.For.Metadata.ModelType == typeof(string))
                this.ValidationRules.Add("empty");
        }

        void HandleRegex(TagHelperContext context, TagHelperOutput output, RegularExpressionAttribute attrib)
        {
            this.AddClasses(output);
            this.ValidationRules.Add($"regex[{attrib.Pattern}]");
        }
    }
}
