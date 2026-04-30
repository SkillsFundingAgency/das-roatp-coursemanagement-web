using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;

[AuthorizeCourseType(CourseType.Apprenticeship)]
[Route("{ukprn}/standards/add/locations/add")]
public class AddStandardTrainingLocationController : AddAStandardControllerBase
{
    public const string ViewPath = "~/Views/AddAStandard/AddStandardTrainingLocation.cshtml";
    private readonly ILogger<AddStandardTrainingLocationController> _logger;
    private readonly IMediator _mediator;
    private readonly IValidator<CourseLocationAddSubmitModel> _validator;

    public AddStandardTrainingLocationController(IMediator mediator, ISessionService sessionService, ILogger<AddStandardTrainingLocationController> logger, IValidator<CourseLocationAddSubmitModel> validator) : base(sessionService)
    {
        _mediator = mediator;
        _logger = logger;
        _validator = validator;
    }

    [HttpGet(Name = RouteNames.GetAddStandardTrainingLocation)]
    public async Task<IActionResult> SelectAProviderlocation()
    {
        var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
        if (sessionModel == null) return redirectResult;

        var model = await GetModel(sessionModel);

        return View(ViewPath, model);
    }

    [HttpPost(Name = RouteNames.PostAddStandardTrainingLocation)]
    public async Task<IActionResult> SubmitAProviderlocation(CourseLocationAddSubmitModel submitModel)
    {
        var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
        if (sessionModel == null) return redirectResult;
        var model = await GetModel(sessionModel);
        if (model == null) return RedirectToRouteWithUkprn(RouteNames.GetAddStandardSelectStandard);

        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid)
        {
            ModelState.AddValidationErrors(validatedResult.Errors);

            return View(ViewPath, model);
        }

        sessionModel.CourseLocations ??= new List<CourseLocationModel>();

        sessionModel.CourseLocations.Add(new CourseLocationModel
        {
            LocationType = LocationType.Provider,
            ProviderLocationId = Guid.Parse(submitModel.TrainingVenueNavigationId),
            LocationName = model.TrainingVenues.First(x => x.Value == submitModel.TrainingVenueNavigationId).Text,
            DeliveryMethod = new DeliveryMethodModel
            {
                HasBlockReleaseDeliveryOption = submitModel.HasBlockReleaseDeliveryOption,
                HasDayReleaseDeliveryOption = submitModel.HasDayReleaseDeliveryOption
            }
        });

        _sessionService.Set(sessionModel);

        return RedirectToRoute(RouteNames.GetNewStandardViewTrainingLocationOptions, new { ukprn = Ukprn });
    }

    private async Task<CourseLocationAddViewModel> GetModel(StandardSessionModel sessionModel)
    {
        _logger.LogInformation("Getting provider course locations for ukprn {Ukprn} for larsCode {LarsCode}", Ukprn, sessionModel.LarsCode);

        var result = await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));
        var allProviderLocations = result.ProviderLocations;

        var availableProviderLocations = new List<ProviderLocation>();

        foreach (var location in allProviderLocations.Where(x => x.LocationType == LocationType.Provider))
        {
            if (!sessionModel.ProviderLocations.Any(x => x.LocationType == LocationType.Provider && x.LocationName == location.LocationName))
                availableProviderLocations.Add(location);
        }

        var model = new CourseLocationAddViewModel
        {
            TrainingVenues = availableProviderLocations.OrderBy(c => c.LocationName).Select(s => new SelectListItem($"{s.LocationName}", s.NavigationId.ToString())),
            LarsCode = sessionModel.LarsCode
        };

        return model;
    }
}