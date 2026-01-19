using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.DfESignIn.Auth.Extensions;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/courses/{courseType}/new/select-course", Name = RouteNames.SelectShortCourse)]
public class SelectShortCourseController(IMediator _mediator, ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddShortCourses/SelectShortCourseView.cshtml";

    [HttpGet]
    [ClearSession(nameof(ShortCourseSessionModel))]
    public async Task<IActionResult> Index()
    {
        var viewModel = await GetModel();
        return View(ViewPath, viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Index(SelectShortCourseSubmitModel submitModel)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = await GetModel();
            return View(ViewPath, viewModel);

        }

        var sessionModel = new ShortCourseSessionModel();

        sessionModel.LarsCode = submitModel.SelectedLarsCode;

        sessionModel.CourseTypeDescription = CourseType.ApprenticeshipUnit.GetDescription().ToLower();

        _sessionService.Set(sessionModel);

        return RedirectToRoute(RouteNames.ConfirmShortCourse, new { ukprn = Ukprn, courseType = CourseType.ApprenticeshipUnit });
    }

    private async Task<SelectShortCourseViewModel> GetModel()
    {
        var result = await _mediator.Send(new GetAvailableProviderStandardsQuery(Ukprn, CourseType.ApprenticeshipUnit));
        var courseTypeDescription = CourseType.ApprenticeshipUnit.GetDescription().ToLower();
        var model = new SelectShortCourseViewModel();
        model.ShortCourses = result.AvailableCourses.OrderBy(c => c.Title).Select(s => new SelectListItem($"{s.Title} (Level {s.Level})", s.LarsCode.ToString()));
        model.CourseTypeDescription = courseTypeDescription;
        return model;
    }
}