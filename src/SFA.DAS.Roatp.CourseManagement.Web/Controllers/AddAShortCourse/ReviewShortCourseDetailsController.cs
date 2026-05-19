using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{learningType}/new")]
public class ReviewShortCourseDetailsController(ISessionService _sessionService, IValidator<ReviewShortCourseDetailsViewModel> _validator, IMediator _mediator, ILogger<ReviewShortCourseDetailsController> _logger) : ControllerBase
{
    public const string ViewPath = "~/Views/ShortCourses/AddAShortCourse/ReviewShortCourseDetails.cshtml";
    public const string ConfirmationPageViewPath = "~/Views/ShortCourses/AddAShortCourse/SaveShortCourseConfirmation.cshtml";

    [HttpGet("review-details", Name = RouteNames.ReviewShortCourseDetails)]
    public IActionResult ReviewShortCourseDetails(LearningType learningType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        sessionModel.HasSeenSummaryPage = true;

        _sessionService.Set(sessionModel);

        ReviewShortCourseDetailsViewModel model = sessionModel;

        model.LearningType = learningType;
        model.ContactInformation.LearningType = learningType;
        model.LocationInformation.LearningType = learningType;
        model.CancelLink = Url.RouteUrl(RouteNames.ReviewYourDetails, new { Ukprn });
        model.ContactInformation.ContactDetailsChangeLink = Url.RouteUrl(RouteNames.AddShortCourseContactDetails, new { ukprn = Ukprn, learningType });
        model.LocationInformation.TrainingRegionsChangeLink = Url.RouteUrl(RouteNames.SelectShortCourseRegions, new { ukprn = Ukprn, learningType });
        model.LocationInformation.TrainingVenuesChangeLink = Url.RouteUrl(RouteNames.SelectShortCourseTrainingVenue, new { ukprn = Ukprn, learningType });
        model.LocationInformation.NationalProviderChangeLink = Url.RouteUrl(RouteNames.ConfirmNationalDelivery, new { ukprn = Ukprn, learningType });
        model.LocationInformation.LocationOptionsChangeLink = Url.RouteUrl(RouteNames.SelectShortCourseLocationOption, new { ukprn = Ukprn, learningType });

        var result = _validator.Validate(model);

        if (!result.IsValid)
        {
            ModelState.AddValidationErrors(result.Errors);
        }

        return View(ViewPath, model);
    }

    [HttpPost("review-details", Name = RouteNames.ReviewShortCourseDetails)]
    public async Task<IActionResult> ReviewShortCourseDetailsPost(LearningType learningType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        ReviewShortCourseDetailsViewModel model = sessionModel;

        var result = _validator.Validate(model);

        if (!result.IsValid)
        {
            return RedirectToRoute(RouteNames.ReviewShortCourseDetails, new { ukprn = Ukprn, learningType });
        }

        AddProviderCourseCommand command = sessionModel;
        command.UserId = UserId;
        command.UserDisplayName = UserDisplayName;
        command.Ukprn = Ukprn;

        await _mediator.Send(command);

        var bannerData = new SaveShortCourseConfirmationViewModel()
        {
            CourseName = sessionModel.ShortCourseInformation.CourseName,
            LearningType = learningType,
        };

        TempData.Add(TempDataKeys.SaveShortCourseBannerTempDataKey, JsonSerializer.Serialize(bannerData));

        return RedirectToRoute(RouteNames.SaveShortCourseConfirmation, new { ukprn = Ukprn, learningType });
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
        model.ManageTrainingTypeLink = Url.RouteUrl(RouteNames.ManageShortCourses, new { ukprn = Ukprn, model.LearningType });

        return model;
    }
}