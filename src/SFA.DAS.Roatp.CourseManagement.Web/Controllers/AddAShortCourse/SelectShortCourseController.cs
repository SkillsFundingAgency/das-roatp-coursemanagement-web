using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/courses/{courseType}/new/select-course", Name = RouteNames.SelectShortCourse)]
public class SelectShortCourseController(IMediator _mediator, ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddAShortCourse/SelectShortCourseView.cshtml";

    [HttpGet]
    [ClearSession(nameof(ShortCourseSessionModel))]
    public async Task<IActionResult> Index(CourseType courseType)
    {
        var viewModel = await GetModel(courseType);
        return View(ViewPath, viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Index(SelectShortCourseSubmitModel submitModel, CourseType courseType)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = await GetModel(courseType);
            return View(ViewPath, viewModel);

        }

        var sessionModel = new ShortCourseSessionModel();

        sessionModel.LarsCode = submitModel.SelectedLarsCode;

        _sessionService.Set(sessionModel);

        return RedirectToRoute(RouteNames.ConfirmShortCourse, new { ukprn = Ukprn, courseType });
    }

    private async Task<SelectShortCourseViewModel> GetModel(CourseType courseType)
    {
        var result = await _mediator.Send(new GetAvailableProviderStandardsQuery(Ukprn, courseType));
        var model = new SelectShortCourseViewModel();
        model.ShortCourses = result.AvailableCourses.OrderBy(c => c.Title).Select(s => new SelectListItem($"{s.Title} (Level {s.Level})", s.LarsCode.ToString()));
        model.CourseType = courseType;
        return model;
    }
}