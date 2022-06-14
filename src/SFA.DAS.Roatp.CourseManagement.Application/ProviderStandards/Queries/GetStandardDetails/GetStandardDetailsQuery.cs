using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails
{
    public class GetStandardDetailsQuery : IRequest<GetStandardDetailsQueryResult>
    {
        public int Ukprn { get; }
        public int LarsCode { get; }

        public GetStandardDetailsQuery(int ukprn, int larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}