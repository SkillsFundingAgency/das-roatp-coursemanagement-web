using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ReviewYourDetailsController : ControllerBase
    {
        private readonly ProviderSharedUIConfiguration _pasSharedConfiguration;
        private readonly ISessionService _sessionService;
        public ReviewYourDetailsController(IOptions<ProviderSharedUIConfiguration> config, ISessionService sessionService)
        {
            _sessionService = sessionService;
            _pasSharedConfiguration = config.Value;
        }

        [Route("{ukprn}/review-your-details", Name = RouteNames.ReviewYourDetails)]
        [HttpGet]
        public IActionResult ReviewYourDetails()
        {
            _sessionService.Delete(nameof(ProviderContactSessionModel));

            var standardsUrl = Url.RouteUrl(RouteNames.ViewStandards, new
            {
                ukprn = Ukprn,
            });

            var providerLocationsUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new
            { ukprn = Ukprn });
            var providerDescriptionUrl = Url.RouteUrl(RouteNames.GetProviderDescription, new { Ukprn });

            var providerContactUrl = Url.RouteUrl(RouteNames.AddProviderContactDetails, new { Ukprn });

            return View("ReviewYourDetails", new ReviewYourDetailsViewModel()
            {
                BackUrl = _pasSharedConfiguration.DashboardUrl,
                StandardsUrl = standardsUrl,
                ProviderLocationsUrl = providerLocationsUrl,
                ProviderDescriptionUrl = providerDescriptionUrl,
                ProviderContactUrl = providerContactUrl
            });
        }
    }
}
