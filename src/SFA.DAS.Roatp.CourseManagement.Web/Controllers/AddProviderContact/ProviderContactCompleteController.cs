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

        var model = new ProviderContactCheckStandardsViewModel
        {
            EmailAddress = sessionModel.EmailAddress,
            PhoneNumber = sessionModel.PhoneNumber,
            CheckedStandards = checkedStandards,
            ReviewYourDetailsUrl = GetUrlWithUkprn(RouteNames.ReviewYourDetails),
            UseBulletedList = checkedStandards.Count > 1
        };

        return View(ViewPath, model);
    }
}
