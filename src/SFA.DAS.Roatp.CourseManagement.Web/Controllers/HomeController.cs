using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Locations;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [ExcludeFromCodeCoverage] // just a place holder at the moment, will be removed or re-purposed 
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var ukprn = HttpContext.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value;

            _logger.LogInformation("Logged into course management with ukprn {ukprn}", ukprn);

            return View(new EditLocationViewModel { Name = "Coventry" });
        }
    }
}
