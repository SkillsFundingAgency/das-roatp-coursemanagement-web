using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;

[ExcludeFromCodeCoverage]
public static class CacheKeys
{
    public const string Regions = nameof(Regions);
    public const string ProviderCourseDetails = nameof(ProviderCourseDetails);
    public static string GetProviderCourseDetailsKey(int ukprn, string larsCode) =>
        $"{ProviderCourseDetails}:{ukprn}:{larsCode}";
}
