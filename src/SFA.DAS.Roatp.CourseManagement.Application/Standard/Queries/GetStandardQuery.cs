using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries
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
