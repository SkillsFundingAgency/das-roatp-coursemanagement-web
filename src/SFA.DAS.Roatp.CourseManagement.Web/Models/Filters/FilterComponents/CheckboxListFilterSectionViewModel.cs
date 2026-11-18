using System.Collections.Generic;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Filters.Abstract;
using static SFA.DAS.Roatp.CourseManagement.Web.Services.FilterService;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Filters.FilterComponents;

public class CheckboxListFilterSectionViewModel : FilterSection
{
    public List<FilterItemViewModel> Items { get; set; } = [];

    public CheckboxListFilterSectionViewModel()
    {
        FilterComponentType = FilterComponentType.CheckboxList;
    }
}
