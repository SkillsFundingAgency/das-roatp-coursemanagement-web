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

        sessionModel.ShortCourseInformation = model.ShortCourseInformation;

        _sessionService.Set(sessionModel);

        return View(model);
    }

    [HttpPost]
    public IActionResult Index(ConfirmApprenticeshipUnitSubmitModel submitModel)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!ModelState.IsValid)
        {
            var model = new ConfirmApprenticeshipUnitViewModel()
            {
                ShortCourseInformation = sessionModel.ShortCourseInformation
            };

            return View(model);
        }

        if (submitModel.IsCorrectShortCourse == false)
        {
            _sessionService.Delete(nameof(ShortCourseSessionModel));

            return RedirectToRouteWithUkprn(RouteNames.SelectAnApprenticeshipUnit);
        }

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
