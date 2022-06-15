using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations
{
    public class GetAllProviderLocationsQuery : IRequest<GetAllProviderLocationsQueryResult>
    {
        public int Ukprn { get; }

        public GetAllProviderLocationsQuery(int ukprn)
        {
            Ukprn = ukprn;
        }
    }
}
