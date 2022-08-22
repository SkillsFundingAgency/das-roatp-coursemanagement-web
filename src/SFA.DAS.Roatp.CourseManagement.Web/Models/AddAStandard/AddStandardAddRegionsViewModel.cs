using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard
{
    public class AddStandardAddRegionsViewModel : RegionsSubmitModel
    {
        private readonly List<RegionViewModel> AllRegions;
        public AddStandardAddRegionsViewModel(List<RegionViewModel> allRegions)
        {
            AllRegions = allRegions;
        }

        public IEnumerable<IGrouping<string, RegionViewModel>> SubregionsGroupedByRegions => AllRegions.GroupBy(x => x.RegionName).OrderBy(x => x.Key);


        public string CancelLink { get; set; }
    }
}
