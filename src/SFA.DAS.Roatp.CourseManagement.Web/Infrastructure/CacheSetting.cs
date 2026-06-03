using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;

[ExcludeFromCodeCoverage]
public static class CacheSetting
{
    public static CacheInfo Regions => new(CacheKeys.Regions, TimeSpan.FromHours(24));
}
