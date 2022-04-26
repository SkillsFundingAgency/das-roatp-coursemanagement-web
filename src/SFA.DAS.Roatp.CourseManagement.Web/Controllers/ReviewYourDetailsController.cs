using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.Provider.Shared.UI.Models;
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

        [Route("roatp/review-your-details")]
        [HttpGet]
        public IActionResult ReviewYourDetails()
        {
            /// Viewmodel should be built from a mediatr response eventually
            return View("ReviewYourDetails", new ReviewYourDetailsViewModel() { DashboardUrl = _pasSharedConfiguration.DashboardUrl });
        }
    }
}
