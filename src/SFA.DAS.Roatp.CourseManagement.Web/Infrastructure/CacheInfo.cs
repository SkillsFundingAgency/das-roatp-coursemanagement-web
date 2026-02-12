using System;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;

[ExcludeFromCodeCoverage]
public record CacheInfo(string Key, TimeSpan CacheDuration);
