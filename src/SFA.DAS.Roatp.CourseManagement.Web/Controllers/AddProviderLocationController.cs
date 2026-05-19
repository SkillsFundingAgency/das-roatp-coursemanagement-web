using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[AuthorizeCourseType(CourseType.ShortCourse, CourseType.Apprenticeship)]
[Route("{ukprn}/courses/{learningType}")]
public class AddProviderLocationController(ISessionService _sessionService, ILogger<AddProviderLocationController> _logger, IMediator _mediator, IValidator<AddressSearchSubmitModel> _validator) : ControllerBase
{
    public const string ViewPath = "~/Views/AddProviderLocation.cshtml";

    [HttpGet("new/add-provider-location/lookup-address", Name = RouteNames.GetAddProviderLocation)]
    public async Task<IActionResult> GetAddress(LearningType learningType)
    {
        bool hasSeenSummaryPage = false;

        if (learningType == LearningType.Apprenticeship)
        {
            var standardsSessionModel = _sessionService.Get<StandardSessionModel>();

            if (standardsSessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

            var providerLocationsResponse = await GetProviderLocations();

            if (providerLocationsResponse.Count > 0)
            {
                _logger.LogWarning("User: {UserId} unexpectedly landed on add training venue page when locations are available for provider.", UserId);

                return RedirectToRouteWithUkprn(RouteNames.GetNewStandardViewTrainingLocationOptions);
            }
        }

        if (learningType == LearningType.ApprenticeshipUnit)
        {
            var shortCourseSessionModel = _sessionService.Get<ShortCourseSessionModel>();

            if (shortCourseSessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

            if (shortCourseSessionModel.LocationsAvailable)
            {
                _logger.LogWarning("User: {UserId} unexpectedly landed on add training venue page when locations are available for provider.", UserId);

                return RedirectToRoute(RouteNames.SelectShortCourseTrainingVenue, new { ukprn = Ukprn, learningType });
            }

            hasSeenSummaryPage = shortCourseSessionModel.HasSeenSummaryPage;
        }

        TempData.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey);

        var model = GetViewModel(learningType, true, hasSeenSummaryPage);

        return View(ViewPath, model);
    }

    [HttpPost("new/add-provider-location/lookup-address", Name = RouteNames.PostAddProviderLocation)]
    public IActionResult GetAddress([FromForm] AddressSearchSubmitModel submitModel, LearningType learningType)
    {
        bool hasSeenSummaryPage = false;

        if (learningType == LearningType.Apprenticeship)
        {
            var standardsSessionModel = _sessionService.Get<StandardSessionModel>();

            if (standardsSessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);
        }

        if (learningType == LearningType.ApprenticeshipUnit)
        {
            var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

            if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

            hasSeenSummaryPage = sessionModel.HasSeenSummaryPage;
        }

        var model = GetViewModel(learningType, true, hasSeenSummaryPage);

        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid) ModelState.AddValidationErrors(validatedResult.Errors);

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

        TempData.Add(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, JsonSerializer.Serialize(selectedAddress));

        return RedirectToRoute(RouteNames.GetConfirmAddProviderLocation, new { ukprn = Ukprn, learningType });
    }

    [HttpGet("{larsCode}/add-provider-location/lookup-address", Name = RouteNames.GetAddProviderLocationEditCourse)]
    public async Task<IActionResult> GetAddress(LearningType learningType, [FromRoute] string larsCode)
    {
        var providerCourseDetailsResponse = await GetProviderCourseDetails(larsCode);

        if (providerCourseDetailsResponse == null)
        {
            _logger.LogWarning("No course details found for ukprn {Ukprn} and LarsCode {LarsCode} for User: {UserId}. Redirecting to PageNotFound.", Ukprn, larsCode, UserId);

            return View(ViewsPath.PageNotFoundPath);
        }

        var providerLocationsResponse = await GetProviderLocations();

        if (providerLocationsResponse.Count > 0)
        {
            switch (learningType)
            {
                case LearningType.Apprenticeship:
                    return RedirectToRoute(RouteNames.GetProviderCourseLocations, new { Ukprn, larsCode });

                case LearningType.ApprenticeshipUnit:
                    return RedirectToRoute(RouteNames.EditShortCourseTrainingVenues, new { ukprn = Ukprn, learningType, larsCode });
            }
        }

        TempData.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey);

        var model = GetViewModel(learningType, false, false);

        return View(ViewPath, model);
    }

    [HttpPost("{larsCode}/add-provider-location/lookup-address", Name = RouteNames.PostAddProviderLocationEditCourse)]
    public IActionResult GetAddress([FromForm] AddressSearchSubmitModel submitModel, LearningType learningType, [FromRoute] string larsCode)
    {
        var model = GetViewModel(learningType, false, false);

        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid) ModelState.AddValidationErrors(validatedResult.Errors);

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

        TempData.Add(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, JsonSerializer.Serialize(selectedAddress));

        return RedirectToRoute(RouteNames.GetConfirmAddProviderLocationEditCourse, new { ukprn = Ukprn, learningType, larsCode });
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

    private static AddProviderLocationViewModel GetViewModel(LearningType learningType, bool isAddJourney, bool hasSeenSummaryPage)
    {
        string submitButtonText = GetButtonText(learningType, isAddJourney, hasSeenSummaryPage);

        var viewModel = new AddProviderLocationViewModel()
        {
            LearningType = learningType,
            SubmitButtonText = submitButtonText,
            Route = isAddJourney ? RouteNames.PostAddProviderLocation : RouteNames.PostAddProviderLocationEditCourse,
            IsAddJourney = isAddJourney
        };

        if (isAddJourney)
        {
            viewModel.DisplayHeader = learningType == LearningType.Apprenticeship ? $"Add a {viewModel.LearningTypeLower}" : $"Add an {viewModel.LearningTypeLower}";
        }
        else
        {
            viewModel.DisplayHeader = learningType == LearningType.Apprenticeship ? $"Manage a {viewModel.LearningTypeLower}" : $"Manage an {viewModel.LearningTypeLower}";
        }

        return viewModel;
    }

    private static string GetButtonText(LearningType learningType, bool isAddJourney, bool hasSeenSummaryPage)
    {
        if (learningType == LearningType.ApprenticeshipUnit && isAddJourney && hasSeenSummaryPage)
        {
            return ButtonText.Confirm;
        }

        return ButtonText.Continue;
    }
}
