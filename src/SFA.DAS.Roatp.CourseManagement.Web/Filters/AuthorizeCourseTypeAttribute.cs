using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeCourseTypeAttribute(CourseType _courseType) : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var providerService = context.HttpContext.RequestServices.GetRequiredService<IProviderCourseTypeService>();

        var ukprnClaim = context.HttpContext.User.FindFirst(ProviderClaims.ProviderUkprn);

        var ukprn = int.Parse(ukprnClaim.Value);

        var courseTypeResponse = await providerService.GetProviderCourseType(ukprn);

        if (!courseTypeResponse.Any(c => c.CourseType == _courseType))
        {
            context.Result = new ViewResult { ViewName = "~/Views/Error/PageNotFound.cshtml" };
        }
    }
}
