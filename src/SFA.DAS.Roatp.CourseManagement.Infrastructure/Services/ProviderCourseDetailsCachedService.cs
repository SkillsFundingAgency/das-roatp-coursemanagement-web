using System.Threading.Tasks;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Infrastructure.Services;

// <summary>
// This service is registered with Scoped lifetime to make the api call only once per request. The results are stored in private variable to enable request based caching.
// </summary>
public class ProviderCourseDetailsCachedService(IApiClient _apiClient) : IProviderCourseDetailsCachedService
{
    private StandardDetails _cachedProviderCourseDetails;
    public async Task<StandardDetails> GetCachedProviderCourseDetails(int ukprn, string larsCode)
    {
        if (_cachedProviderCourseDetails != null)
        {
            return _cachedProviderCourseDetails;
        }

        _cachedProviderCourseDetails = await _apiClient.Get<StandardDetails>($"providers/{ukprn}/courses/{larsCode}");
        return _cachedProviderCourseDetails;
    }
}
