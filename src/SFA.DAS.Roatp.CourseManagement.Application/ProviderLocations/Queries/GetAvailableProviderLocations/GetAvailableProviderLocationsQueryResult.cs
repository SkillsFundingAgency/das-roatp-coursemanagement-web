using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAvailableProviderLocations
{
    public class GetAvailableProviderLocationsQueryResult
    {
        public List<ProviderLocation> AvailableProviderLocations { get; set; } = new List<ProviderLocation>();
    }
}
