using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using System.Collections.Generic;

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
            /// Viewmodel should be built from a mediatr response eventually
            return View("ReviewYourDetails", new ReviewYourDetailsViewModel()
            {
                DashboardUrl = _pasSharedConfiguration.DashboardUrl,
                RouteDictionary = new Dictionary<string, string>
                {{ "ukprn", HttpContext.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value }}
            });
        }
    }
}
