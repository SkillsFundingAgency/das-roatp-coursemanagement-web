using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/courses/{courseType}/new/use-provider-contact", Name = RouteNames.ConfirmSavedContactDetailsForShortCourse)]
public class ConfirmSavedContactDetailsController(ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddAShortCourse/ConfirmSavedContactDetailsView.cshtml";

    [HttpGet]
    public IActionResult ConfirmSavedContactDetails(CourseType courseType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null || sessionModel.LatestProviderContactModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (sessionModel.LatestProviderContactModel.EmailAddress == null && sessionModel.LatestProviderContactModel.PhoneNumber == null) return RedirectToRoute(RouteNames.AddShortCourseContactDetails, new { ukprn = Ukprn, courseType });

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

        sessionModel.ContactInformation = submitModel.IsUsingSavedContactDetails == true ? new ContactInformationSessionModel()
        {
            ContactUsEmail = sessionModel.LatestProviderContactModel.EmailAddress,
            ContactUsPhoneNumber = sessionModel.LatestProviderContactModel.PhoneNumber,
        } : new ContactInformationSessionModel();

        _sessionService.Set(sessionModel);

        return RedirectToRoute(RouteNames.AddShortCourseContactDetails, new { ukprn = Ukprn, courseType });
    }

    private static ConfirmSavedContactDetailsViewModel GetViewModel(ShortCourseSessionModel sessionModel, int ukprn, CourseType courseType)
    {
        return new ConfirmSavedContactDetailsViewModel
        {
            Ukprn = ukprn,
            EmailAddress = sessionModel.LatestProviderContactModel.EmailAddress,
            PhoneNumber = sessionModel.LatestProviderContactModel.PhoneNumber,
            ShowEmail = !string.IsNullOrWhiteSpace(sessionModel.LatestProviderContactModel.EmailAddress),
            ShowPhone = !string.IsNullOrWhiteSpace(sessionModel.LatestProviderContactModel.PhoneNumber),
            IsUsingSavedContactDetails = sessionModel.IsUsingSavedContactDetails,
            CourseType = courseType
        };
    }
}
