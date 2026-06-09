using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;

namespace SFA.DAS.Roatp.CourseManagement.Web.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CheckCourseTypeAttribute(CourseType _courseType) : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CheckCourseTypeAttribute>>();
        var providerCourseDetailsService = context.HttpContext.RequestServices.GetRequiredService<IProviderCourseDetailsCachedService>();
        var ukprnClaim = context.HttpContext.User.FindFirst(ProviderClaims.ProviderUkprn);

        var ukprn = int.Parse(ukprnClaim.Value);

        var larsCode = context.RouteData.Values["larsCode"]?.ToString();

        if (string.IsNullOrWhiteSpace(larsCode))
        {
            context.Result = new ViewResult
            {
                ViewName = ViewsPath.PageNotFoundPath
            };
            return;
        }

        var providerCourseDetailsResponse = await providerCourseDetailsService.GetCachedProviderCourseDetails(ukprn, larsCode);

        if (providerCourseDetailsResponse == null)
        {
            logger.LogWarning("No data returned for Ukprn {Ukprn} and LarsCode {LarsCode}. Redirecting to PageNotFound.", ukprn, larsCode);

            context.Result = new ViewResult
            {
                ViewName = ViewsPath.PageNotFoundPath
            };
            return;
        }

        if (providerCourseDetailsResponse.CourseType != _courseType)
        {
            logger.LogWarning("LarsCode {LarsCode} is not a valid {CourseType}.", larsCode, _courseType);

            context.Result = new ViewResult
            {
                ViewName = ViewsPath.PageNotFoundPath
            };
            return;
        }

        await next();
    }
}