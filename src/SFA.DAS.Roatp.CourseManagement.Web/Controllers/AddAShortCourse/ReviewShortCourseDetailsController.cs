using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}/new")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class ReviewShortCourseDetailsController(ISessionService _sessionService, IValidator<ReviewShortCourseDetailsViewModel> _validator, IMediator _mediator, ILogger<ReviewShortCourseDetailsController> _logger) : ControllerBase
{
    public const string ViewPath = "~/Views/AddAShortCourse/ReviewShortCourseDetailsView.cshtml";
    public const string ConfirmationPageViewPath = "~/Views/AddAShortCourse/SaveShortCourseConfirmationView.cshtml";

    [HttpGet("review-details", Name = RouteNames.ReviewShortCourseDetails)]
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
        model.NationalProviderChangeLink = Url.RouteUrl(RouteNames.ConfirmNationalDelivery, new { ukprn = Ukprn, apprenticeshipType });
        model.LocationOptionsChangeLink = Url.RouteUrl(RouteNames.SelectShortCourseLocationOption, new { ukprn = Ukprn, apprenticeshipType });

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

    [HttpPost("review-details", Name = RouteNames.ReviewShortCourseDetails)]
    public async Task<IActionResult> ReviewShortCourseDetailsPost(ApprenticeshipType apprenticeshipType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        ReviewShortCourseDetailsViewModel model = sessionModel;

        var result = _validator.Validate(model);

        if (!result.IsValid)
        {
            return RedirectToRoute(RouteNames.ReviewShortCourseDetails, new { ukprn = Ukprn, apprenticeshipType });
        }

        AddProviderCourseCommand command = sessionModel;
        command.UserId = UserId;
        command.UserDisplayName = UserDisplayName;
        command.Ukprn = Ukprn;

        await _mediator.Send(command);

        var bannerData = new SaveShortCourseConfirmationViewModel()
        {
            CourseName = sessionModel.ShortCourseInformation.CourseName,
            ApprenticeshipType = apprenticeshipType,
        };

        TempData.Add(TempDataKeys.SaveShortCourseBannerTempDataKey, JsonSerializer.Serialize(bannerData));

        return RedirectToRoute(RouteNames.SaveShortCourseConfirmation, new { ukprn = Ukprn, apprenticeshipType });
    }

    [HttpGet("training-confirmation", Name = RouteNames.SaveShortCourseConfirmation)]
    [ClearSession(nameof(ShortCourseSessionModel))]

    public IActionResult SaveShortCourseConfirmation()
    {
        var model = GetModel();

        if (model == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        TempData.Remove(TempDataKeys.SaveShortCourseBannerTempDataKey);

        return View(ConfirmationPageViewPath, model);
    }

    private SaveShortCourseConfirmationViewModel GetModel()
    {
        TempData.TryGetValue(TempDataKeys.SaveShortCourseBannerTempDataKey, out var bannerData);
        if (bannerData == null)
        {
            _logger.LogWarning("Banner temp data not found, navigating user back to review your details page");
            return null;
        }
        var model = JsonSerializer.Deserialize<SaveShortCourseConfirmationViewModel>(bannerData.ToString()!);
        model.DashboardLink = Url.RouteUrl(RouteNames.ReviewYourDetails, new { Ukprn });
        model.ManageTrainingTypeLink = Url.RouteUrl(RouteNames.ManageShortCourses, new { ukprn = Ukprn, model.ApprenticeshipType });

        return model;
    }
}