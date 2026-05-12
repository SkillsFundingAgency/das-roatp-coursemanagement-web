using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddNationalLocation;
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
[Route("{ukprn}/standards/{larsCode}/edit-national-delivery-option")]
public class EditNationalDeliveryOptionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISessionService _sessionService;
    private readonly ILogger<EditNationalDeliveryOptionController> _logger;
    private readonly IValidator<ConfirmNationalProviderSubmitModel> _validator;
    public EditNationalDeliveryOptionController(IMediator mediator, ISessionService sessionService, ILogger<EditNationalDeliveryOptionController> logger, IValidator<ConfirmNationalProviderSubmitModel> validator)
    {
        _mediator = mediator;
        _sessionService = sessionService;
        _logger = logger;
        _validator = validator;
    }

    [HttpGet(Name = RouteNames.GetNationalDeliveryOption)]
    public IActionResult Index([FromRoute] string larsCode)
    {
        if (!IsCorrectLocationOptionSetInSession())
        {
            _logger.LogWarning("Location option is not set in session, navigating back to the question ukprn:{Ukprn} larscode: {LarsCode}", Ukprn, larsCode);
            return RedirectToRoute(RouteNames.GetLocationOption, new { Ukprn, larsCode });
        }

        return View(new EditNationalDeliveryOptionViewModel());
    }

    [HttpPost(Name = RouteNames.PostNationalDeliveryOption)]
    public async Task<IActionResult> Index([FromRoute] string larsCode, ConfirmNationalProviderSubmitModel model)
    {
        var validatedResult = _validator.Validate(model);

        if (!validatedResult.IsValid) ModelState.AddValidationErrors(validatedResult.Errors);

        if (!ModelState.IsValid)
        {
            _logger.LogInformation("National delivery option was not selected ukprn:{Ukprn} larscode:{LarsCode}", Ukprn, larsCode);

            return View(new EditNationalDeliveryOptionViewModel());
        }

        if (model.HasNationalDeliveryOption.GetValueOrDefault())
        {
            _logger.LogInformation("National delivery option selected, adding national location to ukprn:{Ukprn} larscode:{LarsCode}", Ukprn, larsCode);
            await _mediator.Send(new DeleteCourseLocationsCommand(Ukprn, larsCode, UserId, UserDisplayName, DeleteProviderCourseLocationOption.DeleteEmployerLocations));
            await _mediator.Send(new AddNationalLocationToStandardCommand(Ukprn, larsCode, UserId, UserDisplayName));
            return RedirectToRoute(RouteNames.GetStandardDetails, new { Ukprn, larsCode });
        }

        _logger.LogInformation("National delivery option not selected, navigating to region page ukprn:{Ukprn} larscode:{LarsCode}", Ukprn, larsCode);

        return RedirectToRoute(RouteNames.GetStandardSubRegions, new { Ukprn, larsCode });
    }

    private bool IsCorrectLocationOptionSetInSession()
    {
        var sessionValue = _sessionService.Get(SessionKeys.SelectedLocationOption);
        return
            (!string.IsNullOrEmpty(sessionValue) &&
            (Enum.TryParse<LocationOption>(sessionValue, out var locationOption)
                && (locationOption == LocationOption.EmployerLocation || locationOption == LocationOption.Both)));
    }
}
