using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.Regions.Commands.UpdateSubRegions;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class RegionsViewModel
    {
        [FromRoute]
        public int LarsCode { get; set; }
        public List<RegionViewModel> AllRegions { get; set; }
        public Dictionary<string, List<RegionViewModel>> Regions
        {
            get;set;
        }
        //{
        //    return AllRegions
        //         .GroupBy(x => x.RegionName)
        //         .OrderBy(x => x.Key);
        //}
        public string[] SelectedSubRegions { get; set; } 
        public string BackUrl { get; set; }
        public string CancelLink { get; set; }

        public static implicit operator UpdateSubRegionsCommand(RegionsViewModel model) =>
          new UpdateSubRegionsCommand
          {
              LarsCode = model.LarsCode,

          };
    }
}
