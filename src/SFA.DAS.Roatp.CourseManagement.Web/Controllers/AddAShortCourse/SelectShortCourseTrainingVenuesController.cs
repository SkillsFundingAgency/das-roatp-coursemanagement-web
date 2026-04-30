using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}/new/select-training-venues", Name = RouteNames.SelectShortCourseTrainingVenue)]
public class SelectShortCourseTrainingVenuesController(ISessionService _sessionService, IMediator _mediator, ILogger<SelectShortCourseTrainingVenuesController> _logger, IValidator<ShortCourseTrainingVenuesSubmitModel> _validator) : ControllerBase
{
    public const string ViewPath = "~/Views/ShortCourses/ShortCourseTrainingVenues.cshtml";

    [HttpGet]
    public async Task<IActionResult> SelectShortCourseTrainingVenue(ApprenticeshipType apprenticeshipType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!sessionModel.LocationOptions.Contains(ShortCourseLocationOption.ProviderLocation))
        {
            _logger.LogWarning("User: {UserId} unexpectedly landed on select a training venue page when location options does not contain provider.", UserId);

            return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);
        }

        var providerLocationsResponse = await GetProviderLocations();

        sessionModel.LocationsAvailable = providerLocationsResponse.Count != 0;

        sessionModel.ProviderLocations = providerLocationsResponse;

        _sessionService.Set(sessionModel);

        if (!sessionModel.LocationsAvailable)
        {
            return RedirectToRoute(RouteNames.GetAddProviderLocation, new { ukprn = Ukprn, apprenticeshipType });
        }

        ShortCourseTrainingVenuesViewModel model = GetViewModel(sessionModel, apprenticeshipType);

        foreach (var trainingVenue in model.TrainingVenues)
        {
            trainingVenue.IsSelected = sessionModel.TrainingVenues.Any(t => t.ProviderLocationId == trainingVenue.ProviderLocationId);
        }

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult SelectShortCourseTrainingVenue(ShortCourseTrainingVenuesSubmitModel submitModel, ApprenticeshipType apprenticeshipType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid)
        {
            ShortCourseTrainingVenuesViewModel model = GetViewModel(sessionModel, apprenticeshipType);

            ModelState.AddValidationErrors(validatedResult.Errors);

            return View(ViewPath, model);
        }

        sessionModel.TrainingVenues = sessionModel.ProviderLocations.Where(p => submitModel.SelectedProviderLocationIds.Contains(p.NavigationId)).Select(p => (TrainingVenueModel)p).ToList();

        _sessionService.Set(sessionModel);

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

    private async Task<List<ProviderLocation>> GetProviderLocations()
    {
        _logger.LogInformation("Getting provider course locations for ukprn {Ukprn}", Ukprn);

        var result = await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));

        var providerLocations = result.ProviderLocations.ToList();

        return providerLocations;
    }

    private static ShortCourseTrainingVenuesViewModel GetViewModel(ShortCourseSessionModel sessionModel, ApprenticeshipType apprenticeshipType)
    {
        List<TrainingVenueModel> trainingVenues = sessionModel.ProviderLocations.Select(p => (TrainingVenueModel)p).Where(p => p.LocationType == LocationType.Provider).OrderBy(l => l.LocationName).ToList();

        var model = new ShortCourseTrainingVenuesViewModel()
        {
            TrainingVenues = trainingVenues,
            ApprenticeshipType = apprenticeshipType,
            SubmitButtonText = sessionModel.HasSeenSummaryPage ? ButtonText.Confirm : ButtonText.Continue,
            Route = RouteNames.SelectShortCourseTrainingVenue,
            IsAddJourney = true
        };

        return model;
    }
}
