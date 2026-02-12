using MediatR;
using SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Services;

public class RegionsService(
    ISessionService _sessionService,
    IDistributedCacheService _distributedCacheService,
    IMediator _mediator
) : IRegionsService
{
    public async Task<List<RegionModel>> GetRegions()
    {
        var sessionRegions = _sessionService.Get<List<RegionModel>>();
        if (sessionRegions != null)
        {
            return sessionRegions;
        }

        var cachedRegions = await _distributedCacheService.GetOrSetAsync(
            CacheSetting.Regions.Key,
            async () =>
            {
                var apiResponse = await _mediator.Send(new GetAllRegionsAndSubRegionsQuery());

                return apiResponse.Regions;
            },
            CacheSetting.Regions.CacheDuration
        );

        _sessionService.Set(cachedRegions);
        return cachedRegions;
    }
}