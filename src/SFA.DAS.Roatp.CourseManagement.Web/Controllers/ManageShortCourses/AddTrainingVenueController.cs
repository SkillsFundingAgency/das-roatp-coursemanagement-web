using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}")]
public class AddTrainingVenueController(ISessionService _sessionService, ILogger<AddTrainingVenueController> _logger, IMediator _mediator) : ControllerBase
{
    public const string ViewPath = "~/Views/ShortCourses/AddTrainingVenue.cshtml";

    [HttpGet("new/add-training-venue/lookup-address", Name = RouteNames.GetAddTrainingVenue)]
    public IActionResult LookupAddressAdd(ApprenticeshipType apprenticeshipType)
    {
        var submitButtonText = ButtonText.Continue;
        var postRoute = RouteNames.PostAddTrainingVenue;

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

        TempData.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey);
        var model = new AddTrainingVenueViewModel() { ApprenticeshipType = apprenticeshipType, SubmitButtonText = submitButtonText, Route = postRoute, IsAddJourney = true };
        return View(ViewPath, model);
    }

    [HttpPost("new/add-training-venue/lookup-address", Name = RouteNames.PostAddTrainingVenue)]
    public IActionResult LookupAddressAdd([FromForm] AddTrainingVenueSubmitModel submitModel, ApprenticeshipType apprenticeshipType)
    {
        var submitButtonText = ButtonText.Continue;
        var postRoute = RouteNames.PostAddTrainingVenue;

        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (sessionModel.HasSeenSummaryPage)
        {
            submitButtonText = ButtonText.Confirm;
        }

        var model = new AddTrainingVenueViewModel() { ApprenticeshipType = apprenticeshipType, SubmitButtonText = submitButtonText, Route = postRoute, IsAddJourney = true };

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

    [HttpGet("{larsCode}/add-training-venue/lookup-address", Name = RouteNames.GetAddTrainingVenueEditShortCourse)]
    public async Task<IActionResult> LookupAddressEdit(ApprenticeshipType apprenticeshipType, [FromRoute] string larsCode)
    {
        var submitButtonText = ButtonText.Continue;
        var postRoute = RouteNames.PostAddTrainingVenueEditShortCourse;

        var providerCourseDetailsResponse = await GetProviderCourseDetails(larsCode);

        if (providerCourseDetailsResponse == null)
        {
            _logger.LogWarning("No data returned for ukprn {Ukprn} and LarsCode {LarsCode} for User: {UserId}. Redirecting to PageNotFound.", Ukprn, larsCode, UserId);

            return View(ViewsPath.PageNotFoundPath);
        }

        var providerLocationsResponse = await GetProviderLocations();

        if (providerLocationsResponse.Count > 0)
        {
            return RedirectToRoute(RouteNames.EditShortCourseTrainingVenues, new { ukprn = Ukprn, apprenticeshipType, larsCode });
        }

        TempData.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey);
        var model = new AddTrainingVenueViewModel() { ApprenticeshipType = apprenticeshipType, SubmitButtonText = submitButtonText, Route = postRoute, IsAddJourney = false };
        return View(ViewPath, model);
    }

    [HttpPost("{larsCode}/add-training-venue/lookup-address", Name = RouteNames.PostAddTrainingVenueEditShortCourse)]
    public IActionResult LookupAddressEdit([FromForm] AddTrainingVenueSubmitModel submitModel, ApprenticeshipType apprenticeshipType, [FromRoute] string larsCode)
    {
        var submitButtonText = ButtonText.Continue;
        var postRoute = RouteNames.PostAddTrainingVenueEditShortCourse;

        var model = new AddTrainingVenueViewModel() { ApprenticeshipType = apprenticeshipType, SubmitButtonText = submitButtonText, Route = postRoute, IsAddJourney = false };

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

        return RedirectToRoute(RouteNames.GetConfirmAddTrainingVenueEditShortCourse, new { ukprn = Ukprn, apprenticeshipType, larsCode });
    }

    private async Task<List<ProviderLocation>> GetProviderLocations()
    {
        _logger.LogInformation("Getting provider locations for ukprn {Ukprn}", Ukprn);

        var result = await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));

        var providerLocations = result.ProviderLocations.ToList();

        return providerLocations;
    }

    private async Task<GetProviderCourseDetailsQueryResult> GetProviderCourseDetails(string larsCode)
    {
        _logger.LogInformation("Getting provider course details for ukprn {Ukprn} and lasrcode {LarsCode}", Ukprn, larsCode);

        var result = await _mediator.Send(new GetProviderCourseDetailsQuery(Ukprn, larsCode));

        return result;
    }
}
