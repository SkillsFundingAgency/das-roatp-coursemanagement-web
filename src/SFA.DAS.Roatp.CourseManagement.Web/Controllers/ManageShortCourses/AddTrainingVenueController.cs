using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Text.Json;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}")]
public class AddTrainingVenueController(ISessionService _sessionService, ILogger<AddTrainingVenueController> _logger) : ControllerBase
{
    public const string ViewPath = "~/Views/ManageShortCourses/AddTrainingVenueView.cshtml";

    [HttpGet("new/add-training-venue/lookup-address", Name = RouteNames.GetAddTrainingVenue)]
    public IActionResult LookupAddress(ApprenticeshipType apprenticeshipType, [FromRoute] string larsCode)
    {
        var submitButtonText = ButtonText.Continue;

        var isAddJourney = IsAddJourney(larsCode);

        if (isAddJourney)
        {
            var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

            if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

            if (sessionModel.LocationsAvailable)
            {
                _logger.LogWarning("User: {UserId} unexpectedly landed on add training venue page when locations are available for provider.", UserId);

                return RedirectToRoute(RouteNames.SelectShortCourseTrainingVenue, new { ukprn = Ukprn, apprenticeshipType });
            }

            if (sessionModel.HasSeenSummaryPage)
            {
                submitButtonText = ButtonText.Confirm;
            }
        }

        TempData.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey);
        var model = new AddTrainingVenueViewModel() { ApprenticeshipType = apprenticeshipType, SubmitButtonText = submitButtonText };
        return View(ViewPath, model);
    }

    [HttpPost("new/add-training-venue/lookup-address", Name = RouteNames.PostAddTrainingVenue)]
    public IActionResult LookupAddress([FromForm] AddTrainingVenueSubmitModel submitModel, ApprenticeshipType apprenticeshipType, [FromRoute] string larsCode)
    {
        var submitButtonText = ButtonText.Continue;

        var isAddJourney = IsAddJourney(larsCode);

        if (isAddJourney)
        {
            var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

            if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

            if (sessionModel.HasSeenSummaryPage)
            {
                submitButtonText = ButtonText.Confirm;
            }
        }

        var model = new AddTrainingVenueViewModel() { ApprenticeshipType = apprenticeshipType, SubmitButtonText = submitButtonText };

        if (!ModelState.IsValid)
        {
            return View(ViewPath, model);
        }

        AddressItem selectedAddress = new AddressItem
        {
            AddressLine1 = submitModel.AddressLine1,
            AddressLine2 = submitModel.AddressLine2,
            Town = submitModel.Town,
            County = submitModel.County,
            Latitude = submitModel.Latitude,
            Longitude = submitModel.Longitude,
            Postcode = submitModel.Postcode
        };

        TempData.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey);
        TempData.Add(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, JsonSerializer.Serialize(selectedAddress));

        return RedirectToRoute(RouteNames.GetConfirmAddTrainingVenue, new { ukprn = Ukprn, apprenticeshipType });
    }

    private static bool IsAddJourney(string larsCode)
    {
        return string.IsNullOrWhiteSpace(larsCode);
    }
}
