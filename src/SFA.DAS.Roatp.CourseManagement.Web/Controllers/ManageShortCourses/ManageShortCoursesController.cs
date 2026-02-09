using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}", Name = RouteNames.ManageShortCourses)]
public class ManageShortCoursesController : ControllerBase
{
    public const string ViewPath = "~/Views/ManageShortCourses/ManageShortCoursesView.cshtml";

    [HttpGet]
    public IActionResult Index(ApprenticeshipType apprenticeshipType)
    {
        var selectShortCourseUrl = Url.RouteUrl(RouteNames.SelectShortCourse, new { ukprn = Ukprn, apprenticeshipType });

        var viewModel = new ManageShortCoursesViewModel()
        {
            ApprenticeshipType = apprenticeshipType,
            AddAShortCourseLink = selectShortCourseUrl,
        };

        return View(ViewPath, viewModel);
    }
}
