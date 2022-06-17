﻿using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class RegionsViewModel
    {
        public List<RegionViewModel> AllRegions { get; set; }
        public IEnumerable<IGrouping<string, RegionViewModel>> Regions()
           {
           return AllRegions
                .GroupBy(x => x.RegionName)
                .OrderBy(x=>x.Key);
           }
        public string BackUrl { get; set; }
        public string CancelLink { get; set; }
    }
}
