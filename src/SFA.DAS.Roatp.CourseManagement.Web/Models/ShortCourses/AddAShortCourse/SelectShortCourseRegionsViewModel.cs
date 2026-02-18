using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class SelectShortCourseRegionsViewModel : RegionsSubmitModel, IBackLink
{
    private readonly List<ShortCourseRegionViewModel> AllRegions;
    public SelectShortCourseRegionsViewModel(List<ShortCourseRegionViewModel> allRegions)
    {
        AllRegions = allRegions;
    }

    public ShortCourseBaseViewModel ShortCourseBaseModel { get; set; } = new ShortCourseBaseViewModel();

    public IEnumerable<IGrouping<string, ShortCourseRegionViewModel>> SubregionsGroupedByRegions => AllRegions.GroupBy(x => x.RegionName).OrderBy(x => x.Key);
}
