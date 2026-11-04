using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
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
    public async Task<IActionResult> Index([FromRoute] string larsCode)
    {
        var model = new EditLocationOptionViewModel();
        var sessionModel = _sessionService.Get<EditLocationOptionsSessionModel>();
        if (sessionModel == null)
        {
            sessionModel = new EditLocationOptionsSessionModel();
            var result = await _mediator.Send(new GetProviderCourseDetailsQuery(Ukprn, larsCode));
            sessionModel.Existing.LocationOption = result.LocationOption;
            sessionModel.Existing.HasNationalOption = result.HasNationalLocation;
            _sessionService.Set(sessionModel); //TODO do not set session on get
            model.LocationOption = result.LocationOption;
        }
        else
        {
            model.LocationOption = sessionModel.Updated.LocationOption.Value;
        }
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
            var model = new EditLocationOptionViewModel();

            return View(model);
        }
        _logger.LogInformation("For Ukprn:{Ukprn} LarsCode:{LarsCode} the location option is being updated to {LocationOption}", Ukprn, larsCode, submitModel.LocationOption);

        var sessionModel = _sessionService.Get<EditLocationOptionsSessionModel>();
        if (sessionModel == null) return NotFound();

        sessionModel.Updated.LocationOption = submitModel.LocationOption;
        _sessionService.Set(sessionModel);

        //switch (submitModel.LocationOption)
        //{
        //    case LocationOption.ProviderLocation:
        //    case LocationOption.EmployerLocation:
        //        var deleteOption =
        //            submitModel.LocationOption == LocationOption.ProviderLocation ?
        //            DeleteProviderCourseLocationOption.DeleteEmployerLocations :
        //            DeleteProviderCourseLocationOption.DeleteProviderLocations;
        //        var command = new DeleteCourseLocationsCommand(Ukprn, larsCode, UserId, UserDisplayName, deleteOption);
        //        await _mediator.Send(command);
        //        break;
        //    case LocationOption.Both:
        //    default:
        //        break;
        //}

        if (submitModel.LocationOption == LocationOption.ProviderLocation || submitModel.LocationOption == LocationOption.Both)
        {
            // TODO
            return RedirectToRoute(RouteNames.GetProviderCourseLocations, new { Ukprn, larsCode });
        }
        return RedirectToRoute(RouteNames.GetNationalDeliveryOption, new { ukprn, larsCode });
    }
}
