using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
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

        var checkedStandards = StandardDescriptionListService.BuildSelectedStandardsList(sessionModel.Standards);

        var showBoth = !string.IsNullOrEmpty(sessionModel.PhoneNumber) && !string.IsNullOrEmpty(sessionModel.EmailAddress);
        var showPhoneOnly = !string.IsNullOrEmpty(sessionModel.PhoneNumber) && string.IsNullOrEmpty(sessionModel.EmailAddress);
        var showEmailOnly = string.IsNullOrEmpty(sessionModel.PhoneNumber) && !string.IsNullOrEmpty(sessionModel.EmailAddress);

        var model = new AddProviderContactCompleteViewModel
        {
            EmailAddress = sessionModel.EmailAddress,
            PhoneNumber = sessionModel.PhoneNumber,
            CheckedStandards = checkedStandards,
            ReviewYourDetailsUrl = GetUrlWithUkprn(RouteNames.ReviewYourDetails),
            UseBulletedList = checkedStandards.Count > 1,
            ShowBoth = showBoth,
            ShowEmailOnly = showEmailOnly,
            ShowPhoneOnly = showPhoneOnly,
            ShowStandards = checkedStandards.Count > 0
        };

        return View(ViewPath, model);
    }
}
