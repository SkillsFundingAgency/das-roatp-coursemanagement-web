using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize( Policy = nameof(PolicyNames.HasProviderAccount) )]
    public class ReviewYourDetailsController : ControllerBase
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
            var standardsUrl = Url.RouteUrl(RouteNames.ViewStandards, new
            {
                ukprn = Ukprn,
            });

            var providerLocationsUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new
            { ukprn = Ukprn});
            var providerDescriptionUrl = Url.RouteUrl(RouteNames.GetProviderDescription, new { Ukprn });

            return View("ReviewYourDetails", new ReviewYourDetailsViewModel()
            {
                BackUrl = _pasSharedConfiguration.DashboardUrl,
                StandardsUrl = standardsUrl,
                ProviderLocationsUrl = providerLocationsUrl,
                ProviderDescriptionUrl = providerDescriptionUrl
            });
        }
    }
}
