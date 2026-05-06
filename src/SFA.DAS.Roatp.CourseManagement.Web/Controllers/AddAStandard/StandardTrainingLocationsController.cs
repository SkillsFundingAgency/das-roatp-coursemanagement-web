using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;

[AuthorizeCourseType(CourseType.Apprenticeship)]
[Route("{ukprn}/standards/add/locations")]
public class StandardTrainingLocationsController : AddAStandardControllerBase
{
    public const string ViewPath = "~/Views/AddAStandard/StandardTrainingLocations.cshtml";
    private readonly ILogger<StandardTrainingLocationsController> _logger;
    private readonly IMediator _mediator;
    private readonly IValidator<TrainingLocationListViewModel> _validator;


    public StandardTrainingLocationsController(
        ISessionService sessionService,
        ILogger<StandardTrainingLocationsController> logger,
        IMediator mediator,
        IValidator<TrainingLocationListViewModel> validator) : base(sessionService)
    {
        _logger = logger;
        _mediator = mediator;
        _validator = validator;
    }

    [HttpGet(Name = RouteNames.GetNewStandardViewTrainingLocationOptions)]
    public async Task<IActionResult> ViewTrainingLocations()
    {
        var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
        if (sessionModel == null) return redirectResult;

        //this is protective code as well as allows post method to avoid this check
        if (sessionModel.LocationOption == LocationOption.EmployerLocation)
        {
            _logger.LogWarning("User: {UserId} unexpectedly landed on provider location page when location option is set to employers.", UserId);
            return RedirectToRouteWithUkprn(RouteNames.ViewStandards);
        }

        var providerLocations = await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));

        if (!providerLocations.ProviderLocations.Any(l => l.LocationType == LocationType.Provider))
        {
            return RedirectToRoute(RouteNames.GetAddProviderLocation, new { ukprn = Ukprn, apprenticeshipType = ApprenticeshipType.Apprenticeship });
        }

        var model = GetModel();
        model.ProviderCourseLocations = MapProviderLocationsToProviderCourseLocations(sessionModel.ProviderLocations);

        return View(ViewPath, model);
    }

    [HttpPost(Name = RouteNames.PostNewStandardConfirmTrainingLocationOptions)]
    public IActionResult SubmitTrainingLocations(TrainingLocationListViewModel model)
    {
        var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
        if (sessionModel == null) return redirectResult;

        model.ProviderCourseLocations = MapProviderLocationsToProviderCourseLocations(sessionModel.ProviderLocations);

        var validatedResult = _validator.Validate(model);

        if (!validatedResult.IsValid) ModelState.AddValidationErrors(validatedResult.Errors);

        if (!ModelState.IsValid)
        {
            return View(ViewPath, model);
        }

        if (sessionModel.LocationOption == LocationOption.ProviderLocation)
        {
            return RedirectToRouteWithUkprn(RouteNames.GetAddStandardReviewStandard);
        }
        //if location option is set to both
        return RedirectToRouteWithUkprn(RouteNames.GetAddStandardConfirmNationalProvider);
    }

    private TrainingLocationListViewModel GetModel() => new TrainingLocationListViewModel
    {
        AddTrainingLocationUrl = Url.RouteUrl(RouteNames.GetAddStandardTrainingLocation, new { Ukprn })
    };

    private List<ProviderCourseLocationViewModel> MapProviderLocationsToProviderCourseLocations(IEnumerable<CourseLocationModel> sessionModelProviderLocations)
    {
        var providerCourseLocations = new List<ProviderCourseLocationViewModel>();
        foreach (var location in sessionModelProviderLocations)
        {
            providerCourseLocations.Add(new ProviderCourseLocationViewModel
            {
                DeliveryMethod = location.DeliveryMethod,
                LocationName = location.LocationName,
                LocationType = location.LocationType,
                RemoveUrl = Url.RouteUrl(RouteNames.GetAddStandardRemoveProviderCourseLocation, new { ukprn = Ukprn, providerLocationId = location.ProviderLocationId })
            });
        }

        return providerCourseLocations;
    }
}