using MediatR;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards
{
    public class GetAllProviderStandardsQuery : IRequest<GetAllProviderStandardsQueryResult>
    {
        public int Ukprn { get; }
        public CourseType? CourseType { get; }

        public GetAllProviderStandardsQuery(int ukprn, CourseType? courseType)
        {
            Ukprn = ukprn;
            CourseType = courseType;
        }
    }
}
