using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/manage-apprenticeship-units/add/confirm-apprenticeship-unit", Name = RouteNames.ConfirmApprenticeshipUnit)]
public class ConfirmShortCourseController(IMediator _mediator, ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddShortCourses/ConfirmShortCourseView.cshtml";

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var model = await GetViewModel(sessionModel.LarsCode);

        sessionModel.ShortCourseInformation = model.ShortCourseInformation;

        _sessionService.Set(sessionModel);

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult Index(ConfirmShortCourseSubmitModel submitModel)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!ModelState.IsValid)
        {
            var model = new ConfirmShortCourseViewModel()
            {
                ShortCourseInformation = sessionModel.ShortCourseInformation
            };

            return View(ViewPath, model);
        }

        if (submitModel.IsCorrectShortCourse == false)
        {
            _sessionService.Delete(nameof(ShortCourseSessionModel));

            return RedirectToRouteWithUkprn(RouteNames.SelectAnApprenticeshipUnit);
        }

        return RedirectToRouteWithUkprn(RouteNames.ConfirmApprenticeshipUnit);
    }

    private async Task<ConfirmShortCourseViewModel> GetViewModel(string larsCode)
    {
        var shortCourseInfo = await _mediator.Send(new GetStandardInformationQuery(larsCode));
        var model = new ConfirmShortCourseViewModel()
        {
            ShortCourseInformation = shortCourseInfo
        };
        return model;
    }
}
