using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/courses/{apprenticeshipType}/new/confirm-course", Name = RouteNames.ConfirmShortCourse)]
public class ConfirmShortCourseController(IMediator _mediator, ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddAShortCourse/ConfirmShortCourseView.cshtml";

    [HttpGet]
    public async Task<IActionResult> ConfirmShortCourse(ApprenticeshipType apprenticeshipType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var model = await GetViewModel(sessionModel.LarsCode, apprenticeshipType);

        sessionModel.ShortCourseInformation = model.ShortCourseInformation;

        _sessionService.Set(sessionModel);

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult ConfirmShortCourse(ConfirmShortCourseSubmitModel submitModel, ApprenticeshipType apprenticeshipType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!ModelState.IsValid)
        {
            var model = new ConfirmShortCourseViewModel()
            {
                ShortCourseInformation = sessionModel.ShortCourseInformation,
                ApprenticeshipType = apprenticeshipType
            };

            return View(ViewPath, model);
        }

        if (submitModel.IsCorrectShortCourse == false)
        {
            _sessionService.Delete(nameof(ShortCourseSessionModel));

            return RedirectToRoute(RouteNames.SelectShortCourse, new { ukprn = Ukprn, apprenticeshipType });
        }

        if (sessionModel.SavedProviderContactModel == null || (sessionModel.SavedProviderContactModel.EmailAddress == null && sessionModel.SavedProviderContactModel.PhoneNumber == null))
        {
            return RedirectToRoute(RouteNames.AddShortCourseContactDetails, new { ukprn = Ukprn, apprenticeshipType });
        }

        return RedirectToRoute(RouteNames.ConfirmSavedContactDetailsForShortCourse, new { ukprn = Ukprn, apprenticeshipType });
    }

    private async Task<ConfirmShortCourseViewModel> GetViewModel(string larsCode, ApprenticeshipType apprenticeshipType)
    {
        var shortCourseInfo = await _mediator.Send(new GetStandardInformationQuery(larsCode));
        var model = new ConfirmShortCourseViewModel()
        {
            ShortCourseInformation = shortCourseInfo,
            ApprenticeshipType = apprenticeshipType
        };
        return model;
    }
}
