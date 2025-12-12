using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails
{
    public class GetStandardDetailsQuery : IRequest<GetStandardDetailsQueryResult>
    {
        public int Ukprn { get; }
        public string LarsCode { get; }

        public GetStandardDetailsQuery(int ukprn, string larsCode)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
        }
    }
}