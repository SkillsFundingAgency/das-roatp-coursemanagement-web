using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries
{
    public class GetStandardQuery : IRequest<GetStandardQueryResult>
    {
        public int Ukprn { get; }

        public GetStandardQuery(int ukprn)
        {
            Ukprn = ukprn;
        }
    }
}
