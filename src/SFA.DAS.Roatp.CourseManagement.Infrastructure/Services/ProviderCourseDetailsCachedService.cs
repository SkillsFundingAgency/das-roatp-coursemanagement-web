using System.Threading.Tasks;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Infrastructure.Services;

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
