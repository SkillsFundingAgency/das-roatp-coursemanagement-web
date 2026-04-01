using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails
{
    public class GetProviderCourseDetailsQuery : IRequest<GetProviderCourseDetailsQueryResult>
    {
        public int Ukprn { get; }
        public string LarsCode { get; }

        public GetProviderCourseDetailsQuery(int ukprn, string larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}