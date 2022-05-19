using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries
{
    public class GetStandardDetailsQuery : IRequest<GetStandardDetailsQueryResult>
    {
        public int Ukprn { get; }
        public int LarsCode { get; }
        public int ProviderCourseId { get; }
        public GetStandardDetailsQuery(int ukprn, int larsCode, int providerCourseId)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            ProviderCourseId = providerCourseId;
        }
    }
}