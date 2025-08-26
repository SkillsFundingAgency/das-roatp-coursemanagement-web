using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using SFA.DAS.Roatp.CourseManagement.Web.Middlewares;


namespace SFA.DAS.Roatp.CourseManagement.Web.AppStart;

[ExcludeFromCodeCoverage]
public static class NoCacheMiddlewareExtensions
{
    public static IApplicationBuilder UseNoBrowserCache(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<NoBrowserCacheMiddleware>();
    }
}

