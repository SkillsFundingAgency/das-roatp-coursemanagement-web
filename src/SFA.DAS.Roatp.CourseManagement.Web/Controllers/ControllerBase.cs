using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [ExcludeFromCodeCoverage]
    public abstract class ControllerBase : Controller
    {
        protected int Ukprn => int.Parse(User.FindFirstValue(ProviderClaims.ProviderUkprn));
        protected string UserId => User.FindFirstValue(ProviderClaims.UserId) ?? User.FindFirstValue(ProviderClaims.DfEUserId);
        protected string UserDisplayName => User.FindFirstValue(ProviderClaims.DisplayName);
        protected string GetStandardDetailsUrl(int larsCode) => Url.RouteUrl(RouteNames.GetStandardDetails, new { Ukprn, larsCode });
        protected string GetUrlWithUkprn(string routeName) => Url.RouteUrl(routeName, new { Ukprn });
        protected RedirectToRouteResult RedirectToRouteWithUkprn(string routeName) => RedirectToRoute(routeName, new { Ukprn });
    }
}
