using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddNationalLocation;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}/{larsCode}/edit-national-delivery", Name = RouteNames.EditShortCourseNationalDelivery)]
public class EditShortCourseNationalDeliveryController(IMediator _mediator, ILogger<EditShortCourseNationalDeliveryController> _logger, ISessionService _sessionService, IValidator<ConfirmNationalDeliverySubmitModel> _validator) : ControllerBase
{
    public const string ViewPath = "~/Views/ShortCourses/ConfirmNationalDelivery.cshtml";

    [HttpGet]
    public async Task<IActionResult> EditShortCourseNationalDelivery(ApprenticeshipType apprenticeshipType, string larsCode)
    {
        var providerCourseDetailsResponse = await GetProviderCourseDetails(larsCode);

        if (providerCourseDetailsResponse == null)
        {
            _logger.LogWarning("No data returned for ukprn {Ukprn} and LarsCode {LarsCode} for User: {UserId}. Redirecting to PageNotFound.", Ukprn, larsCode, UserId);

            return View(ViewsPath.PageNotFoundPath);
        }

        var viewModel = GetViewModel(providerCourseDetailsResponse, apprenticeshipType);

        return View(ViewPath, viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditShortCourseNationalDelivery(ConfirmNationalDeliverySubmitModel submitModel, ApprenticeshipType apprenticeshipType, string larsCode)
    {
        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid)
        {
            var viewModel = GetViewModel(new GetProviderCourseDetailsQueryResult(), apprenticeshipType);

            ModelState.AddValidationErrors(validatedResult.Errors);

            return View(ViewPath, viewModel);
        }

        var providerCourseDetailsResponse = await GetProviderCourseDetails(larsCode);

        if (providerCourseDetailsResponse == null)
        {
            return RedirectToRoute(RouteNames.EditShortCourseNationalDelivery, new { Ukprn, apprenticeshipType, larsCode });
        }

        var hasSelectionChanged = providerCourseDetailsResponse.ProviderCourseLocations.Any(x => x.LocationType == LocationType.National) != submitModel.HasNationalDeliveryOption;

        if (hasSelectionChanged && submitModel.HasNationalDeliveryOption == true)
        {
            await _mediator.Send(new DeleteCourseLocationsCommand(Ukprn, larsCode, UserId, UserDisplayName, DeleteProviderCourseLocationOption.DeleteEmployerLocations));
            await _mediator.Send(new AddNationalLocationToStandardCommand(Ukprn, larsCode, UserId, UserDisplayName));
        }

        if (submitModel.HasNationalDeliveryOption == false && !providerCourseDetailsResponse.ProviderCourseLocations.Any(x => x.LocationType == LocationType.Regional))
        {
            return RedirectToRoute(RouteNames.EditShortCourseRegions, new { Ukprn, apprenticeshipType, larsCode });
        }

        return RedirectToRoute(RouteNames.ManageShortCourseDetails, new { Ukprn, apprenticeshipType, larsCode });
    }

    private async Task<GetProviderCourseDetailsQueryResult> GetProviderCourseDetails(string larsCode)
    {
        _logger.LogInformation("Getting provider course details for ukprn {Ukprn} and lasrcode {LarsCode}", Ukprn, larsCode);

        var result = await _mediator.Send(new GetProviderCourseDetailsQuery(Ukprn, larsCode));

        return result;
    }

    private ConfirmNationalDeliveryViewModel GetViewModel(GetProviderCourseDetailsQueryResult providerCourseDetails, ApprenticeshipType apprenticeshipType)
    {
        bool? hasNationalDeliveryOption = HasNationalDeliveryOption(providerCourseDetails);

        var model = new ConfirmNationalDeliveryViewModel();

        model.ApprenticeshipType = apprenticeshipType;
        model.HasNationalDeliveryOption = hasNationalDeliveryOption;
        model.SubmitButtonText = ButtonText.Confirm;
        model.Route = RouteNames.EditShortCourseNationalDelivery;
        model.IsAddJourney = false;

        return model;
    }

    private bool? HasNationalDeliveryOption(GetProviderCourseDetailsQueryResult providerCourseDetails)
    {
        var sessionValue = _sessionService.Get(SessionKeys.SelectedShortCourseLocationOption);
        if (!string.IsNullOrEmpty(sessionValue) &&
            (Enum.TryParse<ShortCourseLocationOption>(sessionValue, out var locationOption)
                && (locationOption == ShortCourseLocationOption.EmployerLocation)))
        {
            return null;
        }

        if (providerCourseDetails.ProviderCourseLocations.Any(x => x.LocationType == LocationType.National))
        {
            return true;
        }

        return false;
    }
}
