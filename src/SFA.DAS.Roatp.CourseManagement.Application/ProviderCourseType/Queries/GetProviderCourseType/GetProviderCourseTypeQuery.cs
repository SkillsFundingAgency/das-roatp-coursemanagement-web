using MediatR;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseType.Queries.GetProviderCourseType;
public class GetProviderCourseTypeQuery : IRequest<List<CourseTypeModel>>
{
    public int Ukprn { get; }

    public GetProviderCourseTypeQuery(int ukprn)
    {
        Ukprn = ukprn;
    }
}
