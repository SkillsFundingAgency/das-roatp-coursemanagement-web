using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}/new/confirm-national", Name = RouteNames.ConfirmNationalProviderDelivery)]
public class ConfirmNationalProviderDeliveryController(ISessionService _sessionService, ILogger<ConfirmNationalProviderDeliveryController> _logger) : ControllerBase
{
    public const string ViewPath = "~/Views/AddAShortCourse/ConfirmNationalProviderDeliveryView.cshtml";

    [HttpGet]
    public IActionResult ConfirmNationalProviderDelivery(ApprenticeshipType apprenticeshipType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!sessionModel.LocationOptions.Contains(ShortCourseLocationOption.EmployerLocation))
        {
            _logger.LogWarning("User: {UserId} unexpectedly landed on national delivery confirmation when location options does not contain employer.", UserId);

            return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);
        }

        var model = new ConfirmNationalProviderDeliveryViewModel()
        {
            ApprenticeshipType = apprenticeshipType,
            HasNationalDeliveryOption = sessionModel.HasNationalDeliveryOption
        };

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult ConfirmNationalProviderDelivery(ConfirmNationalProviderDeliverySubmitModel submitModel, ApprenticeshipType apprenticeshipType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!ModelState.IsValid)
        {
            return View(ViewPath, new ConfirmNationalProviderDeliveryViewModel() { ApprenticeshipType = apprenticeshipType });
        }

        sessionModel.HasNationalDeliveryOption = submitModel.HasNationalDeliveryOption;

        _sessionService.Set(sessionModel);

        if (submitModel.HasNationalDeliveryOption == false)
        {
            return RedirectToRoute(RouteNames.ConfirmNationalProviderDelivery, new { ukprn = Ukprn, apprenticeshipType });
        }

        return RedirectToRoute(RouteNames.ConfirmNationalProviderDelivery, new { ukprn = Ukprn, apprenticeshipType });
    }
}
