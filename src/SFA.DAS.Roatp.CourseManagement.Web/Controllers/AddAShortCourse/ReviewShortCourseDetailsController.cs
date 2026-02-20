using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}/new/review-details", Name = RouteNames.ReviewShortCourseDetails)]
public class ReviewShortCourseDetailsController(ISessionService _sessionService, IValidator<ReviewShortCourseDetailsViewModel> _validator) : ControllerBase
{
    public const string ViewPath = "~/Views/AddAShortCourse/ReviewShortCourseDetailsView.cshtml";
    public IActionResult ReviewShortCourseDetails(ApprenticeshipType apprenticeshipType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        sessionModel.HasSeenSummaryPage = true;

        _sessionService.Set(sessionModel);

        ReviewShortCourseDetailsViewModel model = sessionModel;

        model.ApprenticeshipType = apprenticeshipType;
        model.CancelLink = Url.RouteUrl(RouteNames.ReviewYourDetails, new { Ukprn });
        model.ContactDetailsChangeLink = Url.RouteUrl(RouteNames.AddShortCourseContactDetails, new { ukprn = Ukprn, apprenticeshipType });
        model.TrainingRegionsChangeLink = Url.RouteUrl(RouteNames.SelectShortCourseRegions, new { ukprn = Ukprn, apprenticeshipType });
        model.TrainingVenuesChangeLink = Url.RouteUrl(RouteNames.SelectShortCourseTrainingVenue, new { ukprn = Ukprn, apprenticeshipType });

        var result = _validator.Validate(model);

        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }

        return View(ViewPath, model);
    }
}
