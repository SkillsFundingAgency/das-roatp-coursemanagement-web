using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
[Route("{ukprn}/manage-apprenticeship-units/add/select-apprenticeship-unit", Name = RouteNames.SelectAnApprenticeshipUnit)]
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

        _sessionService.Set(sessionModel);

        return RedirectToRouteWithUkprn(RouteNames.ConfirmApprenticeshipUnit);
    }

    private async Task<SelectShortCourseViewModel> GetModel()
    {
        var result = await _mediator.Send(new GetAvailableProviderStandardsQuery(Ukprn, CourseType.ApprenticeshipUnit));
        var model = new SelectShortCourseViewModel();
        model.ShortCourses = result.AvailableCourses.OrderBy(c => c.Title).Select(s => new SelectListItem($"{s.Title} (Level {s.Level})", s.LarsCode.ToString()));
        return model;
    }
}