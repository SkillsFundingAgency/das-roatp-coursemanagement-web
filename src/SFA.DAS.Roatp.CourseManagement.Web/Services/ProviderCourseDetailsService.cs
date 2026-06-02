using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;

namespace SFA.DAS.Roatp.CourseManagement.Web.Services;

public class ProviderCourseDetailsService(IDistributedCacheService _distributedCacheService, IMediator _mediator) : IProviderCourseDetailsService
{
    public async Task<GetProviderCourseDetailsQueryResult> GetProviderCourseDetails(int ukprn, string larsCode)
    {
        var cacheKey = CacheKeys.GetProviderCourseDetailsKey(ukprn, larsCode);

        return await _distributedCacheService.GetOrSetAsync(
            cacheKey,
            async () =>
            {
                var apiResponse = await _mediator.Send(new GetProviderCourseDetailsQuery(ukprn, larsCode));

                return apiResponse;
            },
            CacheSetting.ProviderCourseDetails.CacheDuration
        );
    }
}
