using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails
{
    public class GetProviderCourseLocationsQuery : IRequest<GetProviderCourseLocationsQueryResult>
    {
        public int Ukprn { get; }
        public int LarsCode { get; }

        public GetProviderCourseLocationsQuery(int ukprn, int larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}