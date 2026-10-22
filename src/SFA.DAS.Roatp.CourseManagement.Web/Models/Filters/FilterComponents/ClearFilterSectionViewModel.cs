using System.Collections.Generic;
using static SFA.DAS.Roatp.CourseManagement.Web.Services.FilterService;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Filters.FilterComponents;

public sealed class ClearFilterSectionViewModel
{
    public required FilterType FilterType { get; set; }
    public required string Title { get; set; }
    public List<ClearFilterItemViewModel> Items { get; set; } = [];
}
