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
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}")]
public class ConfirmAddProviderLocationController(ISessionService _sessionService, ILogger<ConfirmAddProviderLocationController> _logger, IMediator _mediator) : ControllerBase
{
    public const string ViewPath = "~/Views/ConfirmAddProviderLocation.cshtml";
    public const string LocationNameNotAvailable = "A location with this name already exists";

    [HttpGet("new/add-provider-location/confirm-location", Name = RouteNames.GetConfirmAddTrainingVenue)]
    public async Task<IActionResult> ConfirmLocationAdd(ApprenticeshipType apprenticeshipType)
    {
        bool hasSeenSummaryPage = false;

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            var standardsSessionModel = _sessionService.Get<StandardSessionModel>();

            if (standardsSessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

            var providerLocationsResponse = await GetProviderLocations();

            if (providerLocationsResponse.ProviderLocations.Count > 0)
            {
                _logger.LogWarning("User: {UserId} unexpectedly landed on add training venue page when locations are available for provider.", UserId);

                return RedirectToRouteWithUkprn(RouteNames.GetNewStandardViewTrainingLocationOptions);
            }
        }

        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit)
        {
            var shortCourseSessionModel = _sessionService.Get<ShortCourseSessionModel>();

            if (shortCourseSessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

            if (shortCourseSessionModel.LocationsAvailable)
            {
                _logger.LogWarning("User: {UserId} unexpectedly landed on add training venue page when locations are available for provider.", UserId);

                return RedirectToRoute(RouteNames.SelectShortCourseTrainingVenue, new { ukprn = Ukprn, apprenticeshipType });
            }

            if (shortCourseSessionModel.HasSeenSummaryPage)
            {
                hasSeenSummaryPage = true;
            }
        }

        var addressItem = GetAddressFromTempData(true);

        if (addressItem == null)
        {
            return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);
        }

        var model = GetViewModel(addressItem, apprenticeshipType, true, hasSeenSummaryPage);
        return View(ViewPath, model);
    }

    [HttpPost("new/add-provider-location/confirm-location", Name = RouteNames.PostConfirmAddTrainingVenue)]
    public async Task<IActionResult> ConfirmLocationAdd(ProviderLocationDetailsSubmitModel submitModel, ApprenticeshipType apprenticeshipType)
    {
        bool hasSeenSummaryPage = false;

        var standardsSessionModel = _sessionService.Get<StandardSessionModel>();
        var shortCourseSessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (standardsSessionModel == null && shortCourseSessionModel == null)
        {
            return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);
        }

        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit && shortCourseSessionModel.HasSeenSummaryPage)
        {
            hasSeenSummaryPage = true;
        }

        var addressItem = GetAddressFromTempData(true);

        if (addressItem == null)
        {
            return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);
        }

        await CheckIfNameIsAvailable(submitModel.LocationName);

        if (!ModelState.IsValid)
        {
            var model = GetViewModel(addressItem, apprenticeshipType, true, hasSeenSummaryPage);

            model.LocationName = submitModel.LocationName;
            return View(ViewPath, model);
        }

        var command = GetCommand(submitModel, addressItem);

        TempData.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey);

        await _mediator.Send(command);

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            return RedirectToRouteWithUkprn(RouteNames.GetNewStandardViewTrainingLocationOptions);
        }

        await SetTrainingVenueInSession();

        if (shortCourseSessionModel.LocationOptions.Contains(ShortCourseLocationOption.EmployerLocation) && shortCourseSessionModel.IsEmployerInfoMissing())
        {
            return RedirectToRoute(RouteNames.ConfirmNationalDelivery, new { ukprn = Ukprn, apprenticeshipType });
        }

        if (shortCourseSessionModel.HasNationalDeliveryOption == false && shortCourseSessionModel.IsEmployerRegionsMissing())
        {
            return RedirectToRoute(RouteNames.SelectShortCourseRegions, new { ukprn = Ukprn, apprenticeshipType });
        }

        return RedirectToRoute(RouteNames.ReviewShortCourseDetails, new { ukprn = Ukprn, apprenticeshipType });
    }

    [HttpGet("{larsCode}/add-provider-location/confirm-location", Name = RouteNames.GetConfirmAddTrainingVenueEditShortCourse)]
    public IActionResult ConfirmLocationEdit(ApprenticeshipType apprenticeshipType, [FromRoute] string larsCode)
    {
        var addressItem = GetAddressFromTempData(true);

        if (addressItem == null)
        {
            return RedirectToRoute(RouteNames.EditShortCourseTrainingVenues, new { ukprn = Ukprn, apprenticeshipType, larsCode });
        }

        var model = GetViewModel(addressItem, apprenticeshipType, false, false);
        return View(ViewPath, model);
    }

    [HttpPost("{larsCode}/add-provider-location/confirm-location", Name = RouteNames.PostConfirmAddTrainingVenueEditShortCourse)]
    public async Task<IActionResult> ConfirmLocationEdit(ProviderLocationDetailsSubmitModel submitModel, ApprenticeshipType apprenticeshipType, [FromRoute] string larsCode)
    {
        var addressItem = GetAddressFromTempData(true);

        if (addressItem == null)
        {
            return RedirectToRoute(RouteNames.EditShortCourseTrainingVenues, new { ukprn = Ukprn, apprenticeshipType, larsCode });
        }

        await CheckIfNameIsAvailable(submitModel.LocationName);

        if (!ModelState.IsValid)
        {
            var model = GetViewModel(addressItem, apprenticeshipType, false, false);

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

    [HttpGet("new/add-provider-location/confirm-location/cancel", Name = RouteNames.CancelAddTrainingVenue)]
    public IActionResult CancelAddProviderLocation(int ukprn, ApprenticeshipType apprenticeshipType)
    {
        TempData.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey);

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            return RedirectToRouteWithUkprn(RouteNames.GetAddStandardSelectLocationOption);
        }

        return RedirectToRoute(RouteNames.SelectShortCourseLocationOption, new { ukprn = Ukprn, apprenticeshipType });
    }

    private CreateProviderLocationCommand GetCommand(ProviderLocationDetailsSubmitModel submitModel, AddressItem addressItem)
    {
        var command = new CreateProviderLocationCommand()
        {
            Ukprn = Ukprn,
            UserId = UserId,
            UserDisplayName = UserDisplayName,
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

    private ConfirmAddProviderLocationViewModel GetViewModel(AddressItem addressItem, ApprenticeshipType apprenticeshipType, bool isAddJourney, bool hasSeenSummaryPage)
    {
        string buttonText = GetButtonText(apprenticeshipType, isAddJourney, hasSeenSummaryPage);
        bool showCancelOption = ShowCancelOption(apprenticeshipType, isAddJourney, hasSeenSummaryPage);

        var model = new ConfirmAddProviderLocationViewModel(addressItem)
        {
            ApprenticeshipType = apprenticeshipType,
            SubmitButtonText = buttonText,
            ShowCancelOption = showCancelOption,
            Route = isAddJourney ? RouteNames.PostConfirmAddTrainingVenue : RouteNames.PostConfirmAddTrainingVenueEditShortCourse,
            CancelLink = Url.RouteUrl(RouteNames.CancelAddTrainingVenue, new { ukprn = Ukprn, apprenticeshipType }),
            IsAddJourney = isAddJourney
        };

        model.DisplayHeader = apprenticeshipType == ApprenticeshipType.Apprenticeship ? $"Add a {model.ApprenticeshipTypeLower}" : $"Add an {model.ApprenticeshipTypeLower}";

        return model;
    }

    private static string GetButtonText(ApprenticeshipType apprenticeshipType, bool isAddJourney, bool hasSeenSummaryPage)
    {
        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit && isAddJourney && !hasSeenSummaryPage)
        {
            return ButtonText.Continue;
        }

        return ButtonText.Confirm;
    }

    private static bool ShowCancelOption(ApprenticeshipType apprenticeshipType, bool isAddJourney, bool hasSeenSummaryPage)
    {
        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit && isAddJourney && !hasSeenSummaryPage || apprenticeshipType == ApprenticeshipType.Apprenticeship && isAddJourney)
        {
            return true;
        }

        return false;
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
        if (locations.ProviderLocations.Any(l => l.LocationName.Equals(locationName.Trim(), StringComparison.CurrentCultureIgnoreCase)))
        {
            ModelState.AddModelError("LocationName", LocationNameNotAvailable);
        }
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
            !string.IsNullOrEmpty(sessionValue) &&
            Enum.TryParse<ShortCourseLocationOption>(sessionValue, out var locationOption)
                && locationOption == ShortCourseLocationOption.EmployerLocation;
    }
}
