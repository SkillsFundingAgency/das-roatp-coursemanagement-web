using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[AuthorizeCourseType(CourseType.Apprenticeship)]
[CheckCourseType(CourseType.Apprenticeship)]
[Route("{ukprn}/standards/{larsCode}/edit-location-option")]
public class EditLocationOptionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EditLocationOptionController> _logger;
    private readonly ISessionService _sessionService;
    private readonly IValidator<LocationOptionSubmitModel> _validator;

    public EditLocationOptionController(IMediator mediator, ILogger<EditLocationOptionController> logger, ISessionService sessionService, IValidator<LocationOptionSubmitModel> validator)
    {
        _mediator = mediator;
        _logger = logger;
        _sessionService = sessionService;
        _validator = validator;
    }

    [HttpGet(Name = RouteNames.GetLocationOption)]
    [ClearSession(nameof(EditLocationOptionsSessionModel))]
    public async Task<IActionResult> Index([FromRoute] string larsCode)
    {
        var result = await _mediator.Send(new GetProviderCourseDetailsQuery(Ukprn, larsCode));
        EditLocationOptionViewModel model = new() { LocationOption = result.LocationOption };
        _logger.LogInformation("For Ukprn:{Ukprn} LarsCode:{LarsCode} the location option is set to {LocationOption}", Ukprn, larsCode, model.LocationOption);

        return View(model);
    }

    [HttpPost(Name = RouteNames.PostLocationOption)]
    public async Task<IActionResult> Index([FromRoute] string larsCode, [FromRoute] int ukprn, LocationOptionSubmitModel submitModel)
    {
        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid) ModelState.AddValidationErrors(validatedResult.Errors);

        if (!ModelState.IsValid)
        {
            return View(submitModel);
        }
        _logger.LogInformation("For Ukprn:{Ukprn} LarsCode:{LarsCode} the location option is being updated to {LocationOption}", Ukprn, larsCode, submitModel.LocationOption);

        GetProviderCourseDetailsQueryResult existing = await _mediator.Send(new GetProviderCourseDetailsQuery(Ukprn, larsCode));

        EditLocationOptionsSessionModel sessionModel = new()
        {
            ExistingLocationOption = existing.LocationOption,
            HasExistingNationalOption = existing.HasNationalLocation,
            UpdatedLocationOption = submitModel.LocationOption
        };
        _sessionService.Set(sessionModel);

        if (sessionModel.HasLocationOptionChanged && existing.HasProviderLocation && (existing.HasNationalLocation || existing.HasRegionalLocation))
        {
            if (submitModel.LocationOption == LocationOption.EmployerLocation)
            {
                var command = new DeleteCourseLocationsCommand(Ukprn, larsCode, UserId, UserDisplayName, DeleteProviderCourseLocationOption.DeleteProviderLocations);
                await _mediator.Send(command);
            }
            else if (submitModel.LocationOption == LocationOption.ProviderLocation)
            {
                var command = new DeleteCourseLocationsCommand(Ukprn, larsCode, UserId, UserDisplayName, DeleteProviderCourseLocationOption.DeleteEmployerLocations);
                await _mediator.Send(command);
            }
        }

        if (submitModel.LocationOption is LocationOption.ProviderLocation or LocationOption.Both)
        {
            return RedirectToRoute(RouteNames.GetProviderCourseLocations, new { Ukprn, larsCode });
        }

        return RedirectToRoute(RouteNames.GetNationalDeliveryOption, new { ukprn, larsCode });
    }
}
