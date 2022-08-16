using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAvailableProviderLocations
{
    public class GetAvailableProviderLocationsQuery : IRequest<GetAvailableProviderLocationsQueryResult>
    {
        public int Ukprn { get; }
        public int LarsCode { get; }

        public GetAvailableProviderLocationsQuery(int ukprn, int larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}
