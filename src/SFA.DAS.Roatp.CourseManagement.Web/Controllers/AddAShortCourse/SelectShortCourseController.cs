using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderContact.Queries;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
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
    public async Task<IActionResult> SelectShortCourse(CourseType courseType)
    {
        var viewModel = await GetModel(courseType);
        return View(ViewPath, viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> SelectShortCourse(SelectShortCourseSubmitModel submitModel, CourseType courseType)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = await GetModel(courseType);
            return View(ViewPath, viewModel);

        }

        var sessionModel = new ShortCourseSessionModel();

        sessionModel.LarsCode = submitModel.SelectedLarsCode;

        var providerContactResponse = await _mediator.Send(new GetLatestProviderContactQuery(Ukprn));

        if (providerContactResponse != null)
        {
            sessionModel.SavedProviderContactModel = new ProviderContactModel
            {
                EmailAddress = providerContactResponse.EmailAddress,
                PhoneNumber = providerContactResponse.PhoneNumber
            };
        }

        _sessionService.Set(sessionModel);

        return RedirectToRoute(RouteNames.ConfirmShortCourse, new { ukprn = Ukprn, courseType });
    }

    private async Task<SelectShortCourseViewModel> GetModel(CourseType courseType)
    {
        var result = await _mediator.Send(new GetAvailableProviderStandardsQuery(Ukprn, courseType));
        var model = new SelectShortCourseViewModel()
        {
            ShortCourses = result.AvailableCourses.OrderBy(c => c.Title).Select(s => new SelectListItem($"{s.Title} (Level {s.Level})", s.LarsCode.ToString())),
            CourseType = courseType
        };

        return model;
    }
}