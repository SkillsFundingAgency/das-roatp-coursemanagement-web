using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class IntroductionController : Controller
    {
        private readonly ProviderSharedUIConfiguration _pasSharedConfiguration;
        public IntroductionController(IOptions<ProviderSharedUIConfiguration> config)
        {
            _pasSharedConfiguration = config.Value;
        }

        //[Route("check-your-record")]
        [HttpGet]
        public IActionResult Index()
        {
            return View("Index", new IntroductionViewModel() { DashboardUrl = _pasSharedConfiguration.DashboardUrl });
        }
    }
}
