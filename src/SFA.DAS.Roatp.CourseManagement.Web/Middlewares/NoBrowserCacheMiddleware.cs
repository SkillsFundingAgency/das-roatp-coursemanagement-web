using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.Roatp.CourseManagement.Web.Middlewares;

[ExcludeFromCodeCoverage]
public class NoBrowserCacheMiddleware
{
    private readonly RequestDelegate _next;

    public NoBrowserCacheMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Set headers to prevent caching
        context.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate, max-age=0";
        context.Response.Headers.Pragma = "no-cache";
        context.Response.Headers.Expires = "0";

        await _next(context);
    }
}