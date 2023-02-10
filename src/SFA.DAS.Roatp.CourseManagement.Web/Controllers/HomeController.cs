using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize( Policy = nameof(PolicyNames.HasProviderAccount) )]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var ukprn = HttpContext.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value;
            return new RedirectToRouteResult(RouteNames.ReviewYourDetails, new { ukprn });
        }
    }
}
