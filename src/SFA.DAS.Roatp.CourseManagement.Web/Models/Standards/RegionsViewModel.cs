using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class RegionsViewModel : RegionsSubmitModel, IBackLink
    {
        [FromRoute]
        public string LarsCode { get; set; }
        public List<RegionViewModel> AllRegions { get; set; }

        public IEnumerable<IGrouping<string, RegionViewModel>> GetGroupedSubRegions()
        {
            return AllRegions.GroupBy(x => x.RegionName).OrderBy(x => x.Key);
        }
    }
}
