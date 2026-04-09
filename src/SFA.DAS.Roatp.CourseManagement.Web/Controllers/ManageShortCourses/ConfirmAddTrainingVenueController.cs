using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.CreateProviderLocation;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourseLocation;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}")]
public class ConfirmAddTrainingVenueController(ISessionService _sessionService, ILogger<ConfirmAddTrainingVenueController> _logger, IMediator _mediator) : ControllerBase
{
    public const string ViewPath = "~/Views/ShortCourses/ConfirmAddTrainingVenue.cshtml";
    public const string LocationNameNotAvailable = "A location with this name already exists";

    [HttpGet("new/add-training-venue/confirm-venue", Name = RouteNames.GetConfirmAddTrainingVenue)]
    [HttpGet("{larsCode}/add-training-venue/confirm-venue", Name = RouteNames.GetConfirmAddTrainingVenueEditShortCourse)]
    public IActionResult ConfirmVenue(ApprenticeshipType apprenticeshipType, [FromRoute] string larsCode)
    {
        var submitButtonText = ButtonText.Confirm;
        bool showCancelOption = false;

        var isAddJourney = IsAddJourney(larsCode);

        var postRoute = isAddJourney
            ? RouteNames.PostConfirmAddTrainingVenue
            : RouteNames.PostConfirmAddTrainingVenueEditShortCourse;

        if (isAddJourney)
        {
            var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

            if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

            if (sessionModel.LocationsAvailable)
            {
                _logger.LogWarning("User: {UserId} unexpectedly landed on add training venue page when locations are available for provider.", UserId);

                return RedirectToRoute(RouteNames.SelectShortCourseTrainingVenue, new { ukprn = Ukprn, apprenticeshipType });
            }

            if (!sessionModel.HasSeenSummaryPage)
            {
                submitButtonText = ButtonText.Continue;
                showCancelOption = true;
            }
        }

        var addressItem = GetAddressFromTempData(true);

        if (addressItem == null)
        {
            return isAddJourney
                ? RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails)
                : RedirectToRoute(RouteNames.EditShortCourseTrainingVenues, new { ukprn = Ukprn, apprenticeshipType, larsCode });
        }

        var model = GetViewModel(addressItem, apprenticeshipType, submitButtonText, showCancelOption, postRoute, isAddJourney);
        return View(ViewPath, model);
    }

    [HttpPost("new/add-training-venue/confirm-venue", Name = RouteNames.PostConfirmAddTrainingVenue)]
    public async Task<IActionResult> ConfirmVenueAdd(ConfirmAddTrainingVenueSubmitModel submitModel, ApprenticeshipType apprenticeshipType)
    {
        var submitButtonText = ButtonText.Confirm;
        bool showCancelOption = false;
        var postRoute = RouteNames.PostConfirmAddTrainingVenue;

        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!sessionModel.HasSeenSummaryPage)
        {
            submitButtonText = ButtonText.Continue;
            showCancelOption = true;
        }

        var addressItem = GetAddressFromTempData(true);

        if (addressItem == null)
        {
            return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);
        }

        await CheckIfNameIsAvailable(submitModel.LocationName);

        if (!ModelState.IsValid)
        {
            var model = GetViewModel(addressItem, apprenticeshipType, submitButtonText, showCancelOption, postRoute, true);

            model.LocationName = submitModel.LocationName;
            return View(ViewPath, model);
        }

        var command = GetCommand(submitModel, addressItem);

        TempData.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey);

        await _mediator.Send(command);

        await SetTrainingVenueInSession();

        if (sessionModel.LocationOptions.Contains(ShortCourseLocationOption.EmployerLocation) && sessionModel.IsEmployerInfoMissing())
        {
            return RedirectToRoute(RouteNames.ConfirmNationalDelivery, new { ukprn = Ukprn, apprenticeshipType });
        }

        if (sessionModel.HasNationalDeliveryOption == false && sessionModel.IsEmployerRegionsMissing())
        {
            return RedirectToRoute(RouteNames.SelectShortCourseRegions, new { ukprn = Ukprn, apprenticeshipType });
        }

        return RedirectToRoute(RouteNames.ReviewShortCourseDetails, new { ukprn = Ukprn, apprenticeshipType });
    }

    [HttpPost("{larsCode}/add-training-venue/confirm-venue", Name = RouteNames.PostConfirmAddTrainingVenueEditShortCourse)]
    public async Task<IActionResult> ConfirmVenueEdit(ConfirmAddTrainingVenueSubmitModel submitModel, ApprenticeshipType apprenticeshipType, [FromRoute] string larsCode)
    {
        var submitButtonText = ButtonText.Confirm;
        bool showCancelOption = false;
        var postRoute = RouteNames.PostConfirmAddTrainingVenueEditShortCourse;

        var addressItem = GetAddressFromTempData(true);

        if (addressItem == null)
        {
            return RedirectToRoute(RouteNames.EditShortCourseTrainingVenues, new { ukprn = Ukprn, apprenticeshipType, larsCode });
        }

        await CheckIfNameIsAvailable(submitModel.LocationName);

        if (!ModelState.IsValid)
        {
            var model = GetViewModel(addressItem, apprenticeshipType, submitButtonText, showCancelOption, postRoute, false);

            model.LocationName = submitModel.LocationName;
            return View(ViewPath, model);
        }

        var command = GetCommand(submitModel, addressItem);

        TempData.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey);

        await _mediator.Send(command);

        var providerLocations = await GetProviderLocations();

        var addedProviderLocation = providerLocations?.ProviderLocations?.FirstOrDefault();

        if (addedProviderLocation == null)
        {
            return RedirectToRoute(RouteNames.EditShortCourseTrainingVenues, new { ukprn = Ukprn, apprenticeshipType, larsCode });
        }

        var addProvideCourseCommand = new AddProviderCourseLocationCommand()
        {
            Ukprn = Ukprn,
            UserId = UserId,
            UserDisplayName = UserDisplayName,
            LarsCode = larsCode,
            LocationNavigationId = addedProviderLocation.NavigationId,
            HasDayReleaseDeliveryOption = false,
            HasBlockReleaseDeliveryOption = false
        };

        await _mediator.Send(addProvideCourseCommand);

        if (IsEmployerLocationOptionSetInSession())
        {
            return RedirectToRoute(RouteNames.EditShortCourseNationalDelivery, new { Ukprn, apprenticeshipType, larsCode });
        }

        return RedirectToRoute(RouteNames.ManageShortCourseDetails, new { Ukprn, apprenticeshipType, larsCode });
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

    private ConfirmAddTrainingVenueViewModel GetViewModel(AddressItem addressItem, ApprenticeshipType apprenticeshipType, string buttonText, bool showCancelOption, string route, bool isAddJourney)
    {
        var model = new ConfirmAddTrainingVenueViewModel(addressItem)
        {
            ApprenticeshipType = apprenticeshipType,
            SubmitButtonText = buttonText,
            ShowCancelOption = showCancelOption,
            Route = route,
            CancelLink = Url.RouteUrl(RouteNames.CancelAddTrainingVenue, new { ukprn = Ukprn, apprenticeshipType }),
            IsAddJourney = isAddJourney
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
        var locations = await GetProviderLocations();
        if (locations.ProviderLocations.Any(l => l.LocationName.Equals(locationName.Trim(), System.StringComparison.CurrentCultureIgnoreCase)))
        {
            ModelState.AddModelError("LocationName", LocationNameNotAvailable);
        }
    }

    private static bool IsAddJourney(string larsCode)
    {
        return string.IsNullOrWhiteSpace(larsCode);
    }

    private async Task<GetAllProviderLocationsQueryResult> GetProviderLocations()
    {
        return await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));
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

    private bool IsEmployerLocationOptionSetInSession()
    {
        var sessionValue = _sessionService.Get(SessionKeys.SelectedShortCourseLocationOption);
        return
            (!string.IsNullOrEmpty(sessionValue) &&
            (Enum.TryParse<ShortCourseLocationOption>(sessionValue, out var locationOption)
                && (locationOption == ShortCourseLocationOption.EmployerLocation)));
    }
}
