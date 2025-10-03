using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard
{
    public class AddStandardAddRegionsViewModel : RegionsSubmitModel, IBackLink
    {
        private readonly List<RegionViewModel> AllRegions;
        public AddStandardAddRegionsViewModel(List<RegionViewModel> allRegions)
        {
            AllRegions = allRegions;
        }

        public IEnumerable<IGrouping<string, RegionViewModel>> SubregionsGroupedByRegions => AllRegions.GroupBy(x => x.RegionName).OrderBy(x => x.Key);
    }
}
