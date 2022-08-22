using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries
{
    public class GetAllRegionsAndSubRegionsQueryResult
    {
        public List<RegionModel> Regions { get; set; }
    }
}
