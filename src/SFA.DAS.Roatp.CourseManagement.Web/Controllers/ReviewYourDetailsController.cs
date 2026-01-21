using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ISessionService _sessionService;
        public ReviewYourDetailsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [Route("{ukprn}/review-your-details", Name = RouteNames.ReviewYourDetails)]
        [HttpGet]
        public IActionResult ReviewYourDetails()
        {
            _sessionService.Delete(nameof(ProviderContactSessionModel));

            var selectCourseTypeUrl = Url.RouteUrl(RouteNames.SelectCourseType, new
            {
                ukprn = Ukprn,
            });

            var providerLocationsUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new
            { ukprn = Ukprn });
            var providerDescriptionUrl = Url.RouteUrl(RouteNames.GetProviderDescription, new { Ukprn });

            var providerContactUrl = Url.RouteUrl(RouteNames.CheckProviderContactDetails, new { Ukprn });

            return View("ReviewYourDetails", new ReviewYourDetailsViewModel()
            {
                SelectCourseTypeUrl = selectCourseTypeUrl,
                ProviderLocationsUrl = providerLocationsUrl,
                ProviderDescriptionUrl = providerDescriptionUrl,
                ProviderContactUrl = providerContactUrl
            });
        }
    }
}
