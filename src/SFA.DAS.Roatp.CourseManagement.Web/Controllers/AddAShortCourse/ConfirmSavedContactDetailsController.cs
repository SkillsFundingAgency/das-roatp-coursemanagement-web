using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/courses/{courseType}/new/use-provider-contact", Name = RouteNames.ConfirmSavedContactDetailsForShortCourse)]
public class ConfirmSavedContactDetailsController(ISessionService _sessionService, ILogger<ConfirmSavedContactDetailsController> _logger) : ControllerBase
{
    public const string ViewPath = "~/Views/AddAShortCourse/ConfirmSavedContactDetailsView.cshtml";

    [HttpGet]
    public IActionResult ConfirmSavedContactDetails(CourseType courseType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (sessionModel.SavedProviderContactModel == null)
        {
            _logger.LogWarning("User: {UserId} unexpectedly landed on confirm saved contact details page when provider contact details are not available.", UserId);

            return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);
        }

        if (sessionModel.SavedProviderContactModel.EmailAddress == null && sessionModel.SavedProviderContactModel.PhoneNumber == null) return RedirectToRoute(RouteNames.AddShortCourseContactDetails, new { ukprn = Ukprn, courseType });

        var model = GetViewModel(sessionModel, Ukprn, courseType);

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult ConfirmSavedContactDetails(ConfirmSavedContactDetailsSubmitModel submitModel, CourseType courseType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!ModelState.IsValid)
        {
            var viewModel = GetViewModel(sessionModel, Ukprn, courseType);
            return View(ViewPath, viewModel);
        }

        sessionModel.IsUsingSavedContactDetails = submitModel.IsUsingSavedContactDetails;

        sessionModel.ContactInformation = submitModel.IsUsingSavedContactDetails == true ? new ContactInformationModel()
        {
            ContactUsEmail = sessionModel.SavedProviderContactModel.EmailAddress,
            ContactUsPhoneNumber = sessionModel.SavedProviderContactModel.PhoneNumber,
        } : new ContactInformationModel();

        _sessionService.Set(sessionModel);

        return RedirectToRoute(RouteNames.AddShortCourseContactDetails, new { ukprn = Ukprn, courseType });
    }

    private static ConfirmSavedContactDetailsViewModel GetViewModel(ShortCourseSessionModel sessionModel, int ukprn, CourseType courseType)
    {
        return new ConfirmSavedContactDetailsViewModel
        {
            Ukprn = ukprn,
            EmailAddress = sessionModel.SavedProviderContactModel.EmailAddress,
            PhoneNumber = sessionModel.SavedProviderContactModel.PhoneNumber,
            ShowEmail = !string.IsNullOrWhiteSpace(sessionModel.SavedProviderContactModel.EmailAddress),
            ShowPhone = !string.IsNullOrWhiteSpace(sessionModel.SavedProviderContactModel.PhoneNumber),
            IsUsingSavedContactDetails = sessionModel.IsUsingSavedContactDetails,
            CourseType = courseType
        };
    }
}
