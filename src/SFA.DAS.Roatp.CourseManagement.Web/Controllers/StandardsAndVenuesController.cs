using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class StandardsAndVenuesController : ControllerBase
    {
        private readonly ILogger<StandardsAndVenuesController> _logger;
        private readonly ProviderSharedUIConfiguration _pasSharedConfiguration;
        public StandardsAndVenuesController( ILogger<StandardsAndVenuesController> logger, IOptions<ProviderSharedUIConfiguration> config)
        {
            _logger = logger;
            _pasSharedConfiguration = config.Value;
        }

        [Route("{ukprn}/standards-and-venues", Name = RouteNames.StandardsAndVenuesDetails)]
        [HttpGet]
        public  IActionResult ViewStandardsAndVenues(int ukprn)
        {
            _logger.LogInformation("Standards and venues for {ukprn}", Ukprn);

            var model = new StandardsAndVenuesViewModel
            {
                StandardsUrl = Url.RouteUrl(RouteNames.ViewStandards, new { ukprn }),
                VenuesUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new { ukprn }),
                BackUrl = _pasSharedConfiguration.DashboardUrl,
                ProviderDescriptionUrl = Url.RouteUrl(RouteNames.ProviderDescription, new { ukprn })
            };

            return View("~/Views/StandardsAndVenues/Index.cshtml", model);
        }
    }
}
