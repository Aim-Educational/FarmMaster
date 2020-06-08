using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace FarmMaster.TagHelpers
{
    public enum FarmMasterModelTagHelperTimespanType
    {
        N_A,
        Days
    }

    [HtmlTargetElement("textbox", Attributes = "asp-for")]
    public class FarmMasterTextboxAspForTagHelper : TagHelper
    {
        public ModelExpression AspFor { get; set; }

        public FarmMasterModelTagHelperTimespanType Timespan { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (this.AspFor == null)
                return;

            output.Attributes.Add("name", this.AspFor.Name);

            if (this.AspFor.Model is TimeSpan timespanModel && this.Timespan != FarmMasterModelTagHelperTimespanType.N_A)
            {
                switch (this.Timespan)
                {
                    case FarmMasterModelTagHelperTimespanType.Days:
                        output.Attributes.Add("value", timespanModel.TotalDays);
                        break;

                    default: throw new NotImplementedException($"{this.Timespan}");
                }
            }
            else
                output.Attributes.Add("value", this.AspFor.Model);
        }
    }
}
