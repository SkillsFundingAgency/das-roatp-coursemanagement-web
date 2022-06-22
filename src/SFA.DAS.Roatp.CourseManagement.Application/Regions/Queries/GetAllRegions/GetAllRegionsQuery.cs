using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries.GetAllRegions
{
    public class GetAllRegionsQuery : IRequest<GetAllRegionsQueryResult>
    {
        public int Ukprn { get; }
        public int LarsCode { get; }

        public GetAllRegionsQuery(int ukprn, int larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}
