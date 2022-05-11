using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ReviewYourDetailsController : Controller
    {
        private readonly ProviderSharedUIConfiguration _pasSharedConfiguration;
        public ReviewYourDetailsController(IOptions<ProviderSharedUIConfiguration> config)
        {
            _pasSharedConfiguration = config.Value;
        }

        [Route("{ukprn}/review-your-details", Name = RouteNames.ReviewYourDetails)]
        [HttpGet]
        public IActionResult ReviewYourDetails()
        {
            var ukprn = HttpContext.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value;

            var standardsUrl = Url.RouteUrl(RouteNames.ViewStandards, new
            {
                ukprn = ukprn,
            }, Request.Scheme, Request.Host.Value);

            var providerLocationsUrl = Url.RouteUrl(RouteNames.ViewProviderLocations, new
            {
                ukprn = ukprn,
            }, Request.Scheme, Request.Host.Value);

            /// Viewmodel should be built from a mediatr response eventually
            return View("ReviewYourDetails", new ReviewYourDetailsViewModel()
            {
                BackUrl = _pasSharedConfiguration.DashboardUrl,
                StandardsUrl = standardsUrl,
                ProviderLocationsUrl = providerLocationsUrl
            });;
        }
    }
}
