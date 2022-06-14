using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries
{
    public class GetAllProviderLocationsQueryResult
    {
        public List<Domain.ApiModels.ProviderLocation> ProviderLocations { get; set; }
    }
}
