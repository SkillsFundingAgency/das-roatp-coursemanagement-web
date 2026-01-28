using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/courses/{courseType}/new/confirm-course", Name = RouteNames.ConfirmShortCourse)]
public class ConfirmShortCourseController(IMediator _mediator, ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddAShortCourse/ConfirmShortCourseView.cshtml";

    [HttpGet]
    public async Task<IActionResult> ConfirmShortCourse(CourseType courseType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var model = await GetViewModel(sessionModel.LarsCode, courseType);

        sessionModel.ShortCourseInformation = model.ShortCourseInformation;

        _sessionService.Set(sessionModel);

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult ConfirmShortCourse(ConfirmShortCourseSubmitModel submitModel, CourseType courseType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!ModelState.IsValid)
        {
            var model = new ConfirmShortCourseViewModel()
            {
                ShortCourseInformation = sessionModel.ShortCourseInformation,
                CourseType = courseType
            };

            return View(ViewPath, model);
        }

        if (submitModel.IsCorrectShortCourse == false)
        {
            _sessionService.Delete(nameof(ShortCourseSessionModel));

            return RedirectToRoute(RouteNames.SelectShortCourse, new { ukprn = Ukprn, courseType });
        }

        if (sessionModel.LatestProviderContactModel == null || (sessionModel.LatestProviderContactModel.EmailAddress == null && sessionModel.LatestProviderContactModel.PhoneNumber == null))
        {
            return RedirectToRoute(RouteNames.AddShortCourseContactDetails, new { ukprn = Ukprn, courseType });
        }

        return RedirectToRoute(RouteNames.ConfirmSavedContactDetailsForShortCourse, new { ukprn = Ukprn, courseType });
    }

    private async Task<ConfirmShortCourseViewModel> GetViewModel(string larsCode, CourseType courseType)
    {
        var shortCourseInfo = await _mediator.Send(new GetStandardInformationQuery(larsCode));
        var model = new ConfirmShortCourseViewModel()
        {
            ShortCourseInformation = shortCourseInfo,
            CourseType = courseType
        };
        return model;
    }
}
