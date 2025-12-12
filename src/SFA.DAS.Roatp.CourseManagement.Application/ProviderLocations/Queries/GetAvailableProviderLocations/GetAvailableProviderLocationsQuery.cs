using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAvailableProviderLocations
{
    public class GetProviderCourseLocationsQuery : IRequest<GetAvailableProviderLocationsQueryResult>
    {
        public int Ukprn { get; }
        public string LarsCode { get; }

        public GetProviderCourseLocationsQuery(int ukprn, string larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}
