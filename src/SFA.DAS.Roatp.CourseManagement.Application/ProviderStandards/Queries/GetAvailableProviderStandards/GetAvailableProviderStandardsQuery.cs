using MediatR;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Constants;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;

public class GetAvailableProviderStandardsQuery : IRequest<GetAvailableProviderStandardsQueryResult>
{
    public int Ukprn { get; }
    public CourseType? CourseType { get; }
    public GetAvailableProviderStandardsQuery(int ukprn, CourseType? courseType)
    {
        Ukprn = ukprn;
        CourseType = courseType;
    }
}