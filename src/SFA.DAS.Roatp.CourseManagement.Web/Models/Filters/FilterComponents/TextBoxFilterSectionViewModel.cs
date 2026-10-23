using SFA.DAS.Roatp.CourseManagement.Web.Models.Filters.Abstract;
using static SFA.DAS.Roatp.CourseManagement.Web.Services.FilterService;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Filters.FilterComponents;

public sealed class TextBoxFilterSectionViewModel : FilterSection
{
    public string InputValue { get; set; } = string.Empty;

    public TextBoxFilterSectionViewModel()
    {
        FilterComponentType = FilterComponentType.TextBox;
    }
}
