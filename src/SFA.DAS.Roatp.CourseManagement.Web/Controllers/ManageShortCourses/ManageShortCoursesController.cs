using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}", Name = RouteNames.ManageShortCourses)]
public class ManageShortCoursesController(IMediator _mediator) : ControllerBase
{
    public const string ViewPath = "~/Views/ManageShortCourses/ManageShortCoursesView.cshtml";

    [HttpGet]
    [ClearSession(nameof(ShortCourseSessionModel))]
    public async Task<IActionResult> Index(ApprenticeshipType apprenticeshipType)
    {
        var selectShortCourseUrl = Url.RouteUrl(RouteNames.SelectShortCourse, new { ukprn = Ukprn, apprenticeshipType });

        var viewModel = new ManageShortCoursesViewModel()
        {
            ApprenticeshipType = apprenticeshipType,
            AddAShortCourseLink = selectShortCourseUrl,
        };

        var result = await _mediator.Send(new GetAllProviderStandardsQuery(Ukprn, CourseType.ShortCourse));

        viewModel.ShortCourses = result.Standards.Select(c => (StandardViewModel)c).OrderBy(c => c.CourseDisplayName).ToList();

        viewModel.ShowShortCourseHeading = viewModel.ShortCourses.Count > 0;

        return View(ViewPath, viewModel);
    }
}
