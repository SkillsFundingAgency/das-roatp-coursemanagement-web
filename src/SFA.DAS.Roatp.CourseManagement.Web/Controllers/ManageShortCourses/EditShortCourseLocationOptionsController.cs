using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateOnlineDeliveryOption;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}/{larsCode}/edit-course-locations", Name = RouteNames.EditShortCourseLocationOptions)]
public class EditShortCourseLocationOptionsController(IMediator _mediator, ILogger<EditShortCourseLocationOptionsController> _logger, ISessionService _sessionService, IValidator<SelectShortCourseLocationOptionsSubmitModel> _validator) : ControllerBase
{
    public const string ViewPath = "~/Views/ShortCourses/ShortCourseLocationOptions.cshtml";

    [HttpGet]
    public async Task<IActionResult> EditShortCourseLocationOptions(ApprenticeshipType apprenticeshipType, string larsCode)
    {
        _sessionService.Delete(SessionKeys.SelectedShortCourseLocationOption);

        var providerCourseDetailsResponse = await GetProviderCourseDetails(larsCode);

        if (providerCourseDetailsResponse == null)
        {
            _logger.LogWarning("No data returned for ukprn {Ukprn} and LarsCode {LarsCode} for User: {UserId}. Redirecting to PageNotFound.", Ukprn, larsCode, UserId);

            return View(ViewsPath.PageNotFoundPath);
        }

        var model = GetViewModel(providerCourseDetailsResponse, apprenticeshipType);

        return View(ViewPath, model);
    }

    [HttpPost]
    public async Task<IActionResult> EditShortCourseLocationOptions(SelectShortCourseLocationOptionsSubmitModel submitModel, ApprenticeshipType apprenticeshipType, string larsCode)
    {
        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid)
        {
            var model = GetViewModel(new GetProviderCourseDetailsQueryResult(), apprenticeshipType);

            ModelState.AddValidationErrors(validatedResult.Errors);

            return View(ViewPath, model);
        }

        var providerCourseDetailsResponse = await GetProviderCourseDetails(larsCode);

        if (providerCourseDetailsResponse == null)
        {
            return RedirectToRoute(RouteNames.EditShortCourseLocationOptions, new { ukprn = Ukprn, apprenticeshipType, larsCode });
        }

        var currentLocationOptions = MapCurrentLocationOptions(providerCourseDetailsResponse);

        var removedOptions = currentLocationOptions.Except(submitModel.SelectedLocationOptions).ToList();

        var shouldHaveOnline = submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.Online);

        if (currentLocationOptions.Contains(ShortCourseLocationOption.Online) != shouldHaveOnline)
        {
            var command = new UpdateOnlineDeliveryOptionCommand();
            command.Ukprn = Ukprn;
            command.LarsCode = larsCode;
            command.UserId = UserId;
            command.UserDisplayName = UserDisplayName;
            command.HasOnlineDeliveryOption = shouldHaveOnline;

            await _mediator.Send(command);
        }

        var deleteLocationOptions = new List<DeleteProviderCourseLocationOption>();

        if (removedOptions.Contains(ShortCourseLocationOption.ProviderLocation))
        {
            deleteLocationOptions.Add(DeleteProviderCourseLocationOption.DeleteProviderLocations);
        }
        if (removedOptions.Contains(ShortCourseLocationOption.EmployerLocation))
        {
            deleteLocationOptions.Add(DeleteProviderCourseLocationOption.DeleteEmployerLocations);
        }

        foreach (var deleteLocationOption in deleteLocationOptions)
        {
            var command = new DeleteCourseLocationsCommand(Ukprn, larsCode, UserId, UserDisplayName, deleteLocationOption);

            await _mediator.Send(command);
        }

        if (submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.EmployerLocation) && !currentLocationOptions.Contains(ShortCourseLocationOption.EmployerLocation))
        {
            _sessionService.Set(ShortCourseLocationOption.EmployerLocation.ToString(), SessionKeys.SelectedShortCourseLocationOption);
        }

        if (submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.ProviderLocation) && !currentLocationOptions.Contains(ShortCourseLocationOption.ProviderLocation))
        {

            return RedirectToRoute(RouteNames.EditShortCourseTrainingVenues, new { Ukprn, apprenticeshipType, larsCode });
        }

        if (submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.EmployerLocation) && !currentLocationOptions.Contains(ShortCourseLocationOption.EmployerLocation))
        {
            return RedirectToRoute(RouteNames.EditShortCourseNationalDelivery, new { Ukprn, apprenticeshipType, larsCode });
        }

        return RedirectToRoute(RouteNames.ManageShortCourseDetails, new { Ukprn, apprenticeshipType, larsCode });
    }

    private async Task<GetProviderCourseDetailsQueryResult> GetProviderCourseDetails(string larsCode)
    {
        _logger.LogInformation("Getting provider course details for ukprn {Ukprn} and lasrcode {LarsCode}", Ukprn, larsCode);

        var result = await _mediator.Send(new GetProviderCourseDetailsQuery(Ukprn, larsCode));

        return result;
    }

    private static SelectShortCourseLocationOptionsViewModel GetViewModel(GetProviderCourseDetailsQueryResult providerCourseDetails, ApprenticeshipType apprenticeshipType)
    {
        var currentLocationOptions = MapCurrentLocationOptions(providerCourseDetails);

        List<ShortCourseLocationOptionModel> locationOptions = new List<ShortCourseLocationOptionModel>();

        foreach (var locationOption in Enum.GetValues<ShortCourseLocationOption>())
        {
            locationOptions.Add(new ShortCourseLocationOptionModel { LocationOption = locationOption, IsSelected = currentLocationOptions.Contains(locationOption) });
        }

        return new SelectShortCourseLocationOptionsViewModel()
        {
            LocationOptions = locationOptions,
            ApprenticeshipType = apprenticeshipType,
            SubmitButtonText = ButtonText.Confirm,
            Route = RouteNames.EditShortCourseLocationOptions,
            IsAddJourney = false
        };
    }

    private static List<ShortCourseLocationOption> MapCurrentLocationOptions(GetProviderCourseDetailsQueryResult providerCourseDetails)
    {
        var locationOptions = new List<ShortCourseLocationOption>();

        if (providerCourseDetails.HasProviderLocation)
            locationOptions.Add(ShortCourseLocationOption.ProviderLocation);
        if (providerCourseDetails.HasRegionalLocation || providerCourseDetails.HasNationalLocation)
            locationOptions.Add(ShortCourseLocationOption.EmployerLocation);
        if (providerCourseDetails.HasOnlineDeliveryOption)
            locationOptions.Add(ShortCourseLocationOption.Online);

        return locationOptions;
    }
}
