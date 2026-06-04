using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{learningType}", Name = RouteNames.ManageShortCourses)]
public class ManageShortCoursesController(IMediator _mediator) : ControllerBase
{
    public const string ViewPath = "~/Views/ShortCourses/ManageShortCourses/ManageShortCourses.cshtml";

    [HttpGet]
    [ClearSession(nameof(ShortCourseSessionModel))]
    public async Task<IActionResult> Index(LearningType learningType)
    {
        var selectShortCourseUrl = Url.RouteUrl(RouteNames.SelectShortCourse, new { ukprn = Ukprn, learningType });

        var viewModel = new ManageShortCoursesViewModel()
        {
            LearningType = learningType,
            AddAShortCourseLink = selectShortCourseUrl,
        };

        TempData.TryGetValue(TempDataKeys.ShowShortCourseDeletedBannerTempDataKey, out var showShortCourseDeleteBanner);

        if (showShortCourseDeleteBanner != null)
        {
            viewModel.ShowDeleteShortCourseNotificationBanner = true;
            viewModel.Banner.LearningType = viewModel.LearningTypeHumanize;
            TempData.Remove(TempDataKeys.ShowShortCourseDeletedBannerTempDataKey);
        }

        var result = await _mediator.Send(new GetAllProviderStandardsQuery(Ukprn, CourseType.ShortCourse));

        viewModel.ShortCourses = result.Standards.Select(c => (ShortCourseViewModel)c).OrderBy(c => c.CourseDisplayName).ToList();

        foreach (var shortCourse in viewModel.ShortCourses)
        {
            shortCourse.StandardUrl = Url.RouteUrl(RouteNames.ManageShortCourseDetails, new { Ukprn, LearningType = learningType, larsCode = shortCourse.LarsCode });
        }

        viewModel.ShowShortCourseHeading = viewModel.ShortCourses.Count > 0;

        return View(ViewPath, viewModel);
    }
}
