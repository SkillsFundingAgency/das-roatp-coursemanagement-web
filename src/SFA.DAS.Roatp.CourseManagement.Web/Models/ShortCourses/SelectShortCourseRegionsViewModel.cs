using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public class SelectShortCourseRegionsViewModel : RegionsSubmitModel, IBackLink
{
    public List<ShortCourseRegionViewModel> AllRegions { get; set; }
    public SelectShortCourseRegionsViewModel(List<ShortCourseRegionViewModel> allRegions)
    {
        AllRegions = allRegions;
    }

    public IEnumerable<IGrouping<string, ShortCourseRegionViewModel>> SubregionsGroupedByRegions => AllRegions.GroupBy(x => x.RegionName).OrderBy(x => x.Key);

    public string SubmitButtonText { get; set; }
    public string Route { get; set; }
    public bool IsAddJourney { get; set; }
}
