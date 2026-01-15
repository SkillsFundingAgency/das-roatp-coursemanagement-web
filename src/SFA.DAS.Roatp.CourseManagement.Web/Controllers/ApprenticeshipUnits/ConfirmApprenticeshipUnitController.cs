using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAnApprenticeshipUnit;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ApprenticeshipUnits;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/manage-apprenticeship-units/add/confirm-apprenticeship-unit", Name = RouteNames.ConfirmApprenticeshipUnit)]
public class ConfirmApprenticeshipUnitController(IMediator _mediator, ISessionService _sessionService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var model = await GetViewModel(sessionModel.LarsCode);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(ConfirmApprenticeshipUnitSubmitModel submitModel)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!ModelState.IsValid)
        {
            var model = await GetViewModel(sessionModel.LarsCode);

            return View(model);
        }

        if (submitModel.IsCorrectShortCourse == false)
        {
            return RedirectToRouteWithUkprn(RouteNames.SelectAnApprenticeshipUnit);
        }

        sessionModel.ShortCourseInformation = await _mediator.Send(new GetStandardInformationQuery(sessionModel.LarsCode));

        _sessionService.Set(sessionModel);

        return RedirectToRouteWithUkprn(RouteNames.ConfirmApprenticeshipUnit);
    }

    private async Task<ConfirmApprenticeshipUnitViewModel> GetViewModel(string larsCode)
    {
        var shortCourseInfo = await _mediator.Send(new GetStandardInformationQuery(larsCode));
        var model = new ConfirmApprenticeshipUnitViewModel()
        {
            ShortCourseInformation = shortCourseInfo
        };
        return model;
    }
}
