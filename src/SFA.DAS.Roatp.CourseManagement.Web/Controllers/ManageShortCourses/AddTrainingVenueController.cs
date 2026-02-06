using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/courses/{apprenticeshipType}")]
public class AddTrainingVenueController(ISessionService _sessionService, ILogger<AddTrainingVenueController> _logger) : ControllerBase
{
    public const string ViewPath = "~/Views/ManageShortCourses/AddTrainingVenueView.cshtml";

    [HttpGet("new/add-training-venue/lookup-address", Name = RouteNames.GetAddTrainingVenue)]
    public Task<IActionResult> LookupAddress(ApprenticeshipType apprenticeshipType, [FromRoute] string larsCode)
    {
        var isAddJourney = IsAddJourney(larsCode);

        if (isAddJourney)
        {
            var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

            if (sessionModel == null) return Task.FromResult<IActionResult>(RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails));

            if (sessionModel.LocationsAvailable)
            {
                _logger.LogWarning("User: {UserId} unexpectedly landed on add training venue page when locations are available for provider.", UserId);

                return Task.FromResult<IActionResult>(RedirectToRoute(RouteNames.SelectShortCourseTrainingVenue, new { ukprn = Ukprn, apprenticeshipType }));
            }
        }

        TempData.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey);
        var model = new AddTrainingVenueViewModel() { ApprenticeshipType = apprenticeshipType };
        return Task.FromResult<IActionResult>(View(ViewPath, model));
    }

    [HttpPost("new/add-training-venue/lookup-address", Name = RouteNames.PostAddTrainingVenue)]
    public Task<IActionResult> LookupAddress([FromForm] AddTrainingVenueSubmitModel submitModel, ApprenticeshipType apprenticeshipType, [FromRoute] string larsCode)
    {
        var isAddJourney = IsAddJourney(larsCode);

        if (isAddJourney)
        {
            var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

            if (sessionModel == null) return Task.FromResult<IActionResult>(RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails));
        }

        var model = new AddTrainingVenueViewModel() { ApprenticeshipType = apprenticeshipType };

        if (!ModelState.IsValid)
        {
            return Task.FromResult<IActionResult>(View(ViewPath, model));
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

        return Task.FromResult<IActionResult>(RedirectToRoute(RouteNames.GetConfirmAddTrainingVenue, new { ukprn = Ukprn, apprenticeshipType }));
    }

    private static bool IsAddJourney(string larsCode)
    {
        return string.IsNullOrWhiteSpace(larsCode);
    }
}
