using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.TagHelpers;

[ExcludeFromCodeCoverage]
[HtmlTargetElement(Attributes = "asp-show")]
public class ShowTagHelper : TagHelper
{
    public bool? AspShow { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!AspShow.GetValueOrDefault())
        {
            output.SuppressOutput();
        }
    }
}
