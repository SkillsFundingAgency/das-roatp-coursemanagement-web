using System.Collections.Generic;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAvailableProviderLocations
{
    public class GetAvailableProviderLocationsQueryResult
    {
        public List<ProviderLocation> AvailableProviderLocations { get; set; } = new List<ProviderLocation>();
    }
}
