using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/courses/{courseType}/new/contact-details", Name = RouteNames.AddShortCourseContactDetails)]
public class AddShortCourseContactDetailsController(ISessionService _sessionService, ILogger<AddShortCourseContactDetailsController> _logger) : ControllerBase
{
    public const string ViewPath = "~/Views/AddAShortCourse/AddShortCourseContactDetailsView.cshtml";

    [HttpGet]
    public IActionResult AddShortCourseContactDetails(CourseType courseType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var model = new AddShortCourseContactDetailsViewModel();

        if (sessionModel.ContactInformation != null)
        {
            model.ContactUsEmail = sessionModel.ContactInformation.ContactUsEmail;
            model.ContactUsPhoneNumber = sessionModel.ContactInformation.ContactUsPhoneNumber;
            model.StandardInfoUrl = sessionModel.ContactInformation.StandardInfoUrl;
            model.CourseType = courseType.Humanize(LetterCasing.LowerCase);
            model.ShowSavedContactDetailsText = sessionModel.IsUsingSavedContactDetails == true;
        }

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult AddShortCourseContactDetails(CourseContactDetailsSubmitModel submitModel, CourseType courseType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!ModelState.IsValid)
        {
            var model = new AddShortCourseContactDetailsViewModel()
            {
                ContactUsEmail = sessionModel.ContactInformation.ContactUsEmail,
                ContactUsPhoneNumber = sessionModel.ContactInformation.ContactUsPhoneNumber,
                StandardInfoUrl = sessionModel.ContactInformation.StandardInfoUrl,
                CourseType = courseType.Humanize(LetterCasing.LowerCase),
                ShowSavedContactDetailsText = sessionModel.IsUsingSavedContactDetails == true
            };

            return View(ViewPath, model);
        }

        sessionModel.ContactInformation.ContactUsEmail = submitModel.ContactUsEmail.Trim();
        sessionModel.ContactInformation.ContactUsPhoneNumber = submitModel.ContactUsPhoneNumber.Trim();
        sessionModel.ContactInformation.StandardInfoUrl = submitModel.StandardInfoUrl.Trim();

        _sessionService.Set(sessionModel);

        _logger.LogInformation("Add {CourseType}: Contact details added for ukprn:{Ukprn} larscode:{Larscode}", courseType, Ukprn, sessionModel.LarsCode);

        return RedirectToRoute(RouteNames.SelectShortCourseLocationOption, new { ukprn = Ukprn, courseType });
    }
}
