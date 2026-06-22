using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;

[Route("{ukprn}/contact/edit/confirm", Name = RouteNames.AddProviderContactSaved)]
public class ProviderContactCompleteController(ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddProviderContact/ProviderContactAdded.cshtml";
    public const string NoCourseViewPath = "~/Views/AddProviderContact/ProviderContactAddedNoCourse.cshtml";

    [HttpGet]
    public IActionResult ContactDetailsSaved(int ukprn)
    {
        var sessionModel = _sessionService.Get<ProviderContactSessionModel>();
        if (sessionModel == null) return RedirectToRoute(RouteNames.ReviewYourDetails, new { ukprn = Ukprn });

        _sessionService.Delete(nameof(ProviderContactSessionModel));

        var checkedStandards = StandardDescriptionListService.BuildSelectedStandardsList(sessionModel.Standards.Where(x => x.CourseType == CourseType.Apprenticeship).OrderBy(x => x.CourseName).ThenBy(x => x.Level).ToList());
        var checkedApprenticeshipUnits = StandardDescriptionListService.BuildSelectedStandardsList(sessionModel.Standards.Where(x => x.CourseType == CourseType.ShortCourse).OrderBy(x => x.CourseName).ThenBy(x => x.Level).ToList());
        var standardList = new CourseListViewModel(checkedStandards);
        var apprenticeshipUnitList = new CourseListViewModel(checkedApprenticeshipUnits);

        var showBoth = !string.IsNullOrEmpty(sessionModel.PhoneNumber) && !string.IsNullOrEmpty(sessionModel.EmailAddress);
        var showPhoneOnly = !string.IsNullOrEmpty(sessionModel.PhoneNumber) && string.IsNullOrEmpty(sessionModel.EmailAddress);
        var showEmailOnly = string.IsNullOrEmpty(sessionModel.PhoneNumber) && !string.IsNullOrEmpty(sessionModel.EmailAddress);

        var model = new AddProviderContactCompleteViewModel
        {
            EmailAddress = sessionModel.EmailAddress,
            PhoneNumber = sessionModel.PhoneNumber,
            CheckedStandards = standardList,
            CheckedApprenticeshipUnits = apprenticeshipUnitList,
            ManageCoursesUrl = GetUrlWithUkprn(RouteNames.SelectCourseType),
            ShowBoth = showBoth,
            ShowEmailOnly = showEmailOnly,
            ShowPhoneOnly = showPhoneOnly,
            ShowStandards = checkedStandards.Count > 0,
            ShowApprenticeshipUnits = checkedApprenticeshipUnits.Count > 0
        };

        if (sessionModel.UpdateExistingStandards == false)
        {
            return View(NoCourseViewPath, model);
        }

        return View(ViewPath, model);
    }
}
