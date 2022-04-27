using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.ApiClients;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class StandardsController : Controller
    {
        private readonly IRoatpCourseManagementOuterApiClient _roatpCourseManagementOuterApiClient;
        private readonly ILogger<StandardsController> _logger;

        public StandardsController(IRoatpCourseManagementOuterApiClient roatpCourseManagementOuterApiClient, ILogger<StandardsController> logger)
        {
            _logger = logger;
            _roatpCourseManagementOuterApiClient = roatpCourseManagementOuterApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetStandards()
        {
            var ukprn = HttpContext.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value;

            _logger.LogInformation("Logged into course management with ukprn {ukprn}", ukprn);

           var result = await _roatpCourseManagementOuterApiClient.GetAllStandards(int.Parse(ukprn));

            var model = new ViewStandardsListViewModel
            {
                Standards = new System.Collections.Generic.List<ViewStandardsViewModel>()
            };
            if(result != null)
            {
                model.Standards = result.Standards;
            }

            return View("~/Views/Standards/ViewStandards.cshtml", model);
        }
    }
}
