using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations
{
    public class GetAllProviderLocationsQueryResult
    {
        public List<ProviderLocation> ProviderLocations { get; set; } = new List<ProviderLocation>();
    }
}
