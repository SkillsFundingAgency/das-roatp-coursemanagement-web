using MediatR;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseType.Queries.GetProviderCourseType;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Services;

public class ProviderCourseTypeService(IMediator _mediator, ISessionService _sessionService) : IProviderCourseTypeService
{
    public async Task<List<CourseTypeModel>> GetProviderCourseType(int ukprn)
    {
        var savedProviderCourseType = _sessionService.Get<ProviderCourseTypeSessionModel>();
        if (savedProviderCourseType != null) return savedProviderCourseType.CourseTypes;

        var providerCourseType = await GetProviderCourseTypes(ukprn);
        _sessionService.Set(new ProviderCourseTypeSessionModel { CourseTypes = providerCourseType });

        return providerCourseType;
    }

    private async Task<List<CourseTypeModel>> GetProviderCourseTypes(int ukprn)
    {
        var providerCourseTypesResponse = await _mediator.Send(new GetProviderCourseTypeQuery(ukprn));
        return providerCourseTypesResponse;
    }
}
