using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using System.Security.Claims;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected int Ukprn => int.Parse(User.FindFirstValue(ProviderClaims.ProviderUkprn));
        protected string UserId => User.FindFirstValue(ProviderClaims.UserId);
        protected string GetStandardDetailsUrl(int larsCode) => Url.RouteUrl(RouteNames.ViewStandardDetails, new { Ukprn, larsCode });
    }
}
