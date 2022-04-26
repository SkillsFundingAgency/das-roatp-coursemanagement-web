using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.ApiClients;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Locations;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [ExcludeFromCodeCoverage] // just a place holder at the moment, will be removed or re-purposed 
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class StandardsController : Controller
    {
        private readonly IRoatpCourseManagementOuterApiClient _roatpCourseManagementOuterApiClient;
        private readonly ILogger<StandardsController> _logger;

        public StandardsController(ILogger<StandardsController> logger, IRoatpCourseManagementOuterApiClient roatpCourseManagementOuterApiClient)
        {
            _logger = logger;
            _roatpCourseManagementOuterApiClient = roatpCourseManagementOuterApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var ukprn = HttpContext.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value;

            _logger.LogInformation("Logged into course management with ukprn {ukprn}", ukprn);

            ukprn = "10063272";
           var result = await _roatpCourseManagementOuterApiClient.GetAllStandards(int.Parse(ukprn));

            var list = new ViewStandardsListViewModel
            {
                Standards = new System.Collections.Generic.List<ViewStandardsViewModel>()
            };

            //var standard1 = new ViewStandardsViewModel
            //{
            //    CourseName = "test1",
            //    IsImported = true
            //};
            //var standard2 = new ViewStandardsViewModel
            //{
            //    CourseName = "test2",
            //    IsImported = false
            //};
            //list.Standards.Add(standard1);
            //list.Standards.Add(standard2);
            list.Standards = result.Standards;

            return View(list);
        }
    }
}
