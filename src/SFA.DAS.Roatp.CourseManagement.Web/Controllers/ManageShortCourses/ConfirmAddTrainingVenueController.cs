using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.CreateProviderLocation;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/courses/{apprenticeshipType}")]
public class ConfirmAddTrainingVenueController(ISessionService _sessionService, ILogger<ConfirmAddTrainingVenueController> _logger, IMediator _mediator) : ControllerBase
{
    public const string ViewPath = "~/Views/ManageShortCourses/ConfirmAddTrainingVenueView.cshtml";
    public const string LocationNameNotAvailable = "A location with this name already exists";

    [HttpGet("new/add-training-venue/confirm-venue", Name = RouteNames.GetConfirmAddTrainingVenue)]
    public IActionResult ConfirmVenue(ApprenticeshipType apprenticeshipType, [FromRoute] string larsCode)
    {
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
        }

        var addressItem = GetAddressFromTempData(true);

        if (addressItem == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var model = GetViewModel(addressItem, apprenticeshipType);
        return View(ViewPath, model);
    }

    [Route("new/add-training-venue/confirm-venue", Name = RouteNames.PostConfirmAddTrainingVenue)]
    [HttpPost]
    public async Task<IActionResult> ConfirmVenue(ConfirmAddTrainingVenueSubmitModel submitModel, ApprenticeshipType apprenticeshipType, [FromRoute] string larsCode)
    {
        var isAddJourney = IsAddJourney(larsCode);

        if (isAddJourney)
        {
            var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

            if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);
        }

        var addressItem = GetAddressFromTempData(true);
        if (addressItem == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (ModelState.IsValid) await CheckIfNameIsAvailable(submitModel.LocationName);

        if (!ModelState.IsValid)
        {
            var model = GetViewModel(addressItem, apprenticeshipType);

            model.LocationName = submitModel.LocationName;
            return View(ViewPath, model);
        }

        var command = GetCommand(submitModel, addressItem);

        TempData.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey);

        await _mediator.Send(command);

        if (isAddJourney)
        {
            await SetTrainingVenueInSession();

            return RedirectToRoute(RouteNames.GetConfirmAddTrainingVenue, new { ukprn = Ukprn, apprenticeshipType });
        }

        return RedirectToRoute(RouteNames.GetConfirmAddTrainingVenue, new { ukprn = Ukprn, apprenticeshipType });
    }

    [HttpGet("new/add-training-venue/confirm-venue/cancel", Name = RouteNames.CancelAddTrainingVenue)]
    public IActionResult CancelAddTrainingVenue(int ukprn, ApprenticeshipType apprenticeshipType)
    {
        TempData.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey);

        return RedirectToRoute(RouteNames.SelectShortCourseLocationOption, new { ukprn = Ukprn, apprenticeshipType });
    }

    private CreateProviderLocationCommand GetCommand(ConfirmAddTrainingVenueSubmitModel submitModel, AddressItem addressItem)
    {
        var command = new CreateProviderLocationCommand()
        {
            Ukprn = base.Ukprn,
            UserId = base.UserId,
            UserDisplayName = base.UserDisplayName,
            LocationName = submitModel.LocationName,
            AddressLine1 = addressItem.AddressLine1,
            AddressLine2 = addressItem.AddressLine2 ?? string.Empty,
            Town = addressItem.Town ?? string.Empty,
            Postcode = addressItem.Postcode,
            County = addressItem.County ?? string.Empty,
            Latitude = addressItem.Latitude,
            Longitude = addressItem.Longitude
        };
        return command;
    }

    private ConfirmAddTrainingVenueViewModel GetViewModel(AddressItem addressItem, ApprenticeshipType apprenticeshipType)
    {
        var model = new ConfirmAddTrainingVenueViewModel(addressItem)
        {
            ApprenticeshipType = apprenticeshipType,
            CancelLink = Url.RouteUrl(RouteNames.CancelAddTrainingVenue, new { ukprn = Ukprn, apprenticeshipType })
        };
        return model;
    }

    private AddressItem GetAddressFromTempData(bool keepTempData)
    {
        TempData.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out var address);
        if (address == null)
        {
            _logger.LogWarning("Selected address not found in the Temp Data, navigating user back to the select training venues page");
            return null;
        }
        if (keepTempData) TempData.Keep(TempDataKeys.SelectedTrainingVenueAddressTempDataKey);
        return JsonSerializer.Deserialize<AddressItem>(address.ToString()!);
    }

    private async Task CheckIfNameIsAvailable(string locationName)
    {
        var locations = await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));
        if (locations.ProviderLocations.Any(l => l.LocationName.Equals(locationName.Trim(), System.StringComparison.CurrentCultureIgnoreCase)))
        {
            ModelState.AddModelError("LocationName", LocationNameNotAvailable);
        }
    }

    private static bool IsAddJourney(string larsCode)
    {
        return string.IsNullOrWhiteSpace(larsCode);
    }

    private async Task SetTrainingVenueInSession()
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        _logger.LogInformation("Getting provider course locations for ukprn {Ukprn}", Ukprn);

        var result = await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));

        sessionModel.TrainingVenues = result.ProviderLocations.Select(p => (TrainingVenueModel)p).Where(p => p.LocationType == LocationType.Provider).OrderBy(l => l.LocationName).ToList();

        foreach (var trainingVenue in sessionModel.TrainingVenues)
        {
            trainingVenue.IsSelected = true;
        }

        sessionModel.LocationsAvailable = true;

        _sessionService.Set(sessionModel);
    }
}
