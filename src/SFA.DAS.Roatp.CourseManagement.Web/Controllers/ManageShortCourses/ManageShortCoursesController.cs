using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/courses/{courseType}", Name = RouteNames.ManageShortCourses)]
public class ManageShortCoursesController(IProviderCourseTypeService _providerCourseTypeService) : ControllerBase
{
    public const string ViewPath = "~/Views/ManageShortCourses/ManageShortCoursesView.cshtml";
    public const string CourseTypeDescription = "apprenticeship units";

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var providerCourseTypeResponse = await _providerCourseTypeService.GetProviderCourseType(Ukprn);

        if (!providerCourseTypeResponse.Any(x => x.CourseType == CourseType.ApprenticeshipUnit))
        {
            return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);
        }

        var selectShortCourseUrl = Url.RouteUrl(RouteNames.SelectShortCourse, new { ukprn = Ukprn, courseType = CourseType.ApprenticeshipUnit });

        var viewModel = new ManageShortCoursesViewModel()
        {
            CourseTypeDescription = CourseTypeDescription,
            AddAShortCourseLink = selectShortCourseUrl,
        };

        return View(ViewPath, viewModel);
    }
}
