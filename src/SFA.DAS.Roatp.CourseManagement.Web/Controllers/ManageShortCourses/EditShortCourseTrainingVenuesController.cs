using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourseLocation;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}/{larsCode}/edit-training-venues", Name = RouteNames.EditShortCourseTrainingVenues)]
public class EditShortCourseTrainingVenuesController(IMediator _mediator, ILogger<EditShortCourseTrainingVenuesController> _logger, ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/ShortCourses/ShortCourseTrainingVenues.cshtml";

    [HttpGet]
    public async Task<IActionResult> EditShortCourseTrainingVenues(ApprenticeshipType apprenticeshipType, string larsCode)
    {
        var providerCourseDetailsResponse = await GetProviderCourseDetails(larsCode);

        if (providerCourseDetailsResponse == null)
        {
            _logger.LogWarning("No data returned for ukprn {Ukprn} and LarsCode {LarsCode} for User: {UserId}. Redirecting to PageNotFound.", Ukprn, larsCode, UserId);

            return View(ViewsPath.PageNotFoundPath);
        }

        var providerLocationsResponse = await GetProviderLocations();

        if (providerLocationsResponse.Count == 0)
        {
            return RedirectToRoute(RouteNames.GetAddProviderLocationEditCourse, new { ukprn = Ukprn, apprenticeshipType, larsCode });
        }

        var model = GetViewModel(providerLocationsResponse, providerCourseDetailsResponse, apprenticeshipType);

        return View(ViewPath, model);
    }

    [HttpPost]
    public async Task<IActionResult> EditShortCourseTrainingVenues(ShortCourseTrainingVenuesSubmitModel submitModel, ApprenticeshipType apprenticeshipType, string larsCode)
    {
        if (!ModelState.IsValid)
        {
            var providerLocationsResponse = await GetProviderLocations();

            if (providerLocationsResponse.Count == 0)
            {
                return RedirectToRoute(RouteNames.EditShortCourseTrainingVenues, new { ukprn = Ukprn, apprenticeshipType, larsCode });
            }

            var model = GetViewModel(providerLocationsResponse, new GetProviderCourseDetailsQueryResult(), apprenticeshipType);

            return View(ViewPath, model);
        }

        var providerCourseDetailsResponse = await GetProviderCourseDetails(larsCode);

        var providerLocations = await GetProviderLocations();

        var selectedProviderLocations = providerLocations.Where(x => submitModel.SelectedProviderLocationIds.Contains(x.NavigationId)).ToList();

        var currentCourseLocations = providerCourseDetailsResponse.ProviderCourseLocations.Where(x => x.LocationType == LocationType.Provider).Select(x => x.LocationName).ToList();

        var addCourseLocationIds = selectedProviderLocations.Where(x => !currentCourseLocations.Contains(x.LocationName)).Select(x => x.NavigationId).ToList();

        var removeCourseLocationIds = providerCourseDetailsResponse.ProviderCourseLocations.Where(x => x.LocationType == LocationType.Provider && !selectedProviderLocations.Any(y => y.LocationName == x.LocationName)).Select(x => x.Id).ToList();

        foreach (var addCourseLocationId in addCourseLocationIds)
        {
            var command = new AddProviderCourseLocationCommand()
            {
                Ukprn = Ukprn,
                UserId = UserId,
                UserDisplayName = UserDisplayName,
                LarsCode = larsCode,
                LocationNavigationId = addCourseLocationId,
                HasDayReleaseDeliveryOption = false,
                HasBlockReleaseDeliveryOption = false
            };

            await _mediator.Send(command);
        }

        foreach (var removeCourseLocationId in removeCourseLocationIds)
        {
            var command = new DeleteProviderCourseLocationCommand(Ukprn, larsCode, removeCourseLocationId, UserId, UserDisplayName);

            await _mediator.Send(command);
        }

        if (IsEmployerLocationOptionSetInSession())
        {
            return RedirectToRoute(RouteNames.EditShortCourseNationalDelivery, new { Ukprn, apprenticeshipType, larsCode });
        }

        return RedirectToRoute(RouteNames.ManageShortCourseDetails, new { Ukprn, apprenticeshipType, larsCode });
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

    private static ShortCourseTrainingVenuesViewModel GetViewModel(List<ProviderLocation> providerLocations, GetProviderCourseDetailsQueryResult providerCourseDetails, ApprenticeshipType apprenticeshipType)
    {
        List<TrainingVenueModel> trainingVenues = providerLocations.Select(p => (TrainingVenueModel)p).Where(p => p.LocationType == LocationType.Provider).OrderBy(l => l.LocationName).ToList();

        var currentTrainingVenues = providerCourseDetails.ProviderCourseLocations.Where(x => x.LocationType == LocationType.Provider).Select(x => x.LocationName).ToList();

        if (currentTrainingVenues.Count != 0)
        {
            foreach (var trainingVenue in trainingVenues)
            {
                trainingVenue.IsSelected = currentTrainingVenues.Any(x => x == trainingVenue.LocationName);
            }
        }

        var model = new ShortCourseTrainingVenuesViewModel()
        {
            TrainingVenues = trainingVenues,
            ApprenticeshipType = apprenticeshipType,
            SubmitButtonText = ButtonText.Confirm,
            Route = RouteNames.EditShortCourseTrainingVenues,
            IsAddJourney = false
        };

        return model;
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
