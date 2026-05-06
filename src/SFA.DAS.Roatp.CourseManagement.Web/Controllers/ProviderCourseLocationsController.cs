using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Services;


namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[AuthorizeCourseType(CourseType.Apprenticeship)]
[Route("{ukprn}/standards/{larsCode}/providerlocations")]
public class ProviderCourseLocationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProviderCourseLocationsController> _logger;
    private readonly ISessionService _sessionService;
    private readonly IValidator<ProviderCourseLocationListViewModel> _validator;

    public ProviderCourseLocationsController(IMediator mediator, ILogger<ProviderCourseLocationsController> logger, ISessionService sessionService, IValidator<ProviderCourseLocationListViewModel> validator)
    {
        _mediator = mediator;
        _logger = logger;
        _sessionService = sessionService;
        _validator = validator;
    }

    [HttpGet(Name = RouteNames.GetProviderCourseLocations)]
    public async Task<IActionResult> GetProviderCourseLocations([FromRoute] string larsCode)
    {
        _logger.LogInformation("Getting Provider Course Locations for ukprn {Ukprn} ", Ukprn);

        var providerLocations = await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));

        if (!providerLocations.ProviderLocations.Any(l => l.LocationType == LocationType.Provider))
        {
            return RedirectToRoute(RouteNames.GetAddProviderLocationEditCourse, new { ukprn = Ukprn, apprenticeshipType = ApprenticeshipType.Apprenticeship, larsCode });
        }

        ProviderCourseLocationListViewModel model = await BuildViewModel(larsCode);

        return View("~/Views/ProviderCourseLocations/EditTrainingLocations.cshtml", model);
    }

    private async Task<ProviderCourseLocationListViewModel> BuildViewModel(string larsCode)
    {
        var result = await _mediator.Send(new GetProviderCourseLocationsQuery(Ukprn, larsCode));

        if (result == null)
        {
            _logger.LogError("Provider Course Locations not found for ukprn {Ukprn} and larscode {LarsCode}", Ukprn, larsCode);
            result = new GetProviderCourseLocationsQueryResult();
        }

        var model = new ProviderCourseLocationListViewModel
        {
            ProviderCourseLocations = result.ProviderCourseLocations.Select(x => (ProviderCourseLocationViewModel)x).ToList(),
            LarsCode = larsCode
        };
        foreach (var location in model.ProviderCourseLocations)
        {
            location.RemoveUrl = Url.RouteUrl(RouteNames.GetRemoveProviderCourseLocation, new { ukprn = Ukprn, larsCode, id = location.Id });
        }

        model.AddTrainingLocationUrl = Url.RouteUrl(RouteNames.GetAddProviderCourseLocation, new { ukprn = Ukprn, larsCode = model.LarsCode });
        return model;
    }

    [HttpPost(Name = RouteNames.PostProviderCourseLocations)]
    public async Task<IActionResult> ConfirmedProviderCourseLocations(ProviderCourseLocationListViewModel model)
    {
        model = await BuildViewModel(model.LarsCode);
        var validatedResult = _validator.Validate(model);

        if (!validatedResult.IsValid) ModelState.AddValidationErrors(validatedResult.Errors);

        if (!ModelState.IsValid)
        {
            return View("~/Views/ProviderCourseLocations/EditTrainingLocations.cshtml", model);
        }

        var sessionValue = _sessionService.Get(SessionKeys.SelectedLocationOption);
        if ((!string.IsNullOrEmpty(sessionValue) &&
            (Enum.TryParse<LocationOption>(sessionValue, out var locationOption)
                && (locationOption == LocationOption.Both))))
        {
            return RedirectToRoute(RouteNames.GetNationalDeliveryOption, new { Ukprn, model.LarsCode });
        }
        else
        {
            return RedirectToRoute(RouteNames.GetStandardDetails, new { Ukprn, model.LarsCode });
        }
    }
}
