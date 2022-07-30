using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards
{
    public class GetAvailableProviderStandardsQueryResult
    {
        public List<StandardLookupModel> AvailableCourses { get; set; } = new List<StandardLookupModel>();
    }
}
