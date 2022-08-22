using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateStandardSubRegions;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class RegionsViewModel : RegionsSubmitModel
    {
        [FromRoute]
        public int LarsCode { get; set; }
        public List<RegionViewModel> AllRegions { get; set; }

        public string BackUrl { get; set; }
        public string CancelLink { get; set; }

        public IEnumerable<IGrouping<string, RegionViewModel>> GetGroupedSubRegions()
        {
            return AllRegions.GroupBy(x => x.RegionName).OrderBy(x => x.Key);
        }
    }
}
