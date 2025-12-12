using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAvailableProviderLocations
{
    public class GetAvailableProviderLocationsQuery : IRequest<GetAvailableProviderLocationsQueryResult>
    {
        public int Ukprn { get; }
        public string LarsCode { get; }

        public GetAvailableProviderLocationsQuery(int ukprn, string larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}
