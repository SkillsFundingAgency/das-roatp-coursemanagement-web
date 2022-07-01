using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries.GetAllStandardRegions
{
    public class GetAllStandardRegionsQuery : IRequest<GetAllStandardRegionsQueryResult>
    {
        public int Ukprn { get; }
        public int LarsCode { get; }

        public GetAllStandardRegionsQuery(int ukprn, int larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}
