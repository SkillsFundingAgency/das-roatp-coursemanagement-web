using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;

[Route("{ukprn}/provider-contact-details-added", Name = RouteNames.AddProviderContactSaved)]
public class ProviderContactCompleteController(ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddProviderContact/ProviderContactAdded.cshtml";

    [HttpGet]
    public IActionResult ContactDetailsSaved(int ukprn)
    {
        var sessionModel = _sessionService.Get<ProviderContactSessionModel>();
        if (sessionModel == null) return RedirectToRoute(RouteNames.ReviewYourDetails, new { ukprn = Ukprn });

        _sessionService.Delete(nameof(ProviderContactSessionModel));

        var checkedStandards = StandardDescriptionListService.BuildSelectedStandardsList(sessionModel.Standards.Where(x => x.CourseType == CourseType.Apprenticeship).ToList());
        var checkedApprenticeshipUnits = StandardDescriptionListService.BuildSelectedStandardsList(sessionModel.Standards.Where(x => x.CourseType == CourseType.ShortCourse).ToList());

        var showBoth = !string.IsNullOrEmpty(sessionModel.PhoneNumber) && !string.IsNullOrEmpty(sessionModel.EmailAddress);
        var showPhoneOnly = !string.IsNullOrEmpty(sessionModel.PhoneNumber) && string.IsNullOrEmpty(sessionModel.EmailAddress);
        var showEmailOnly = string.IsNullOrEmpty(sessionModel.PhoneNumber) && !string.IsNullOrEmpty(sessionModel.EmailAddress);
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit.ToString();

        var model = new AddProviderContactCompleteViewModel
        {
            EmailAddress = sessionModel.EmailAddress,
            PhoneNumber = sessionModel.PhoneNumber,
            CheckedStandards = checkedStandards,
            CheckedApprenticeshipUnits = checkedApprenticeshipUnits,
            ReviewYourDetailsUrl = GetUrlWithUkprn(RouteNames.ReviewYourDetails),
            ManageShortCoursesUrl = Url.RouteUrl(RouteNames.ManageShortCourses, new { ukprn = Ukprn, apprenticeshipType }),
            ShowBoth = showBoth,
            ShowEmailOnly = showEmailOnly,
            ShowPhoneOnly = showPhoneOnly,
            ShowStandards = checkedStandards.Count > 0,
            ShowApprenticeshipUnits = checkedApprenticeshipUnits.Count > 0
        };

        return View(ViewPath, model);
    }
}
