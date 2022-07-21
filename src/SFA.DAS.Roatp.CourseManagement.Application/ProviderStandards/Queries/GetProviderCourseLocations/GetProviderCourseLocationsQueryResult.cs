using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails
{
    public class GetProviderCourseLocationsQueryResult
    {
        public List<ProviderCourseLocation> ProviderCourseLocations { get; set; } = new List<ProviderCourseLocation>();
    }
}