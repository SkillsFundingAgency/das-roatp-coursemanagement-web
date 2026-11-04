using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAvailableProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourseLocation;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[AuthorizeCourseType(CourseType.Apprenticeship)]
[CheckCourseType(CourseType.Apprenticeship)]
[Route("{ukprn}/standards/{larsCode}/providerlocations/add-new")]
public class ProviderCourseLocationAddController : ControllerBase
{
    public const string ViewPath = "~/Views/ProviderCourseLocations/AddTrainingCourseLocation.cshtml";
    private readonly ILogger<ProviderCourseLocationAddController> _logger;
    private readonly IMediator _mediator;
    private readonly IValidator<ProviderCourseLocationAddSubmitModel> _validator;
    private readonly ISessionService _sessionService;

    public ProviderCourseLocationAddController(ILogger<ProviderCourseLocationAddController> logger, IMediator mediator, IValidator<ProviderCourseLocationAddSubmitModel> validator, ISessionService sessionService)
    {
        _logger = logger;
        _mediator = mediator;
        _validator = validator;
        _sessionService = sessionService;
    }

    [HttpGet(Name = RouteNames.GetAddProviderCourseLocation)]
    public async Task<IActionResult> SelectAProviderlocation([FromRoute] string larsCode)
    {
        var model = await GetModel(larsCode);
        return View(ViewPath, model);
    }

    [HttpPost(Name = RouteNames.PostAddProviderCourseLocation)]
    public async Task<IActionResult> SubmitAProviderlocation([FromRoute] string larsCode, ProviderCourseLocationAddSubmitModel submitModel)
    {
        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid) ModelState.AddValidationErrors(validatedResult.Errors);

        if (!ModelState.IsValid)
        {
            var model = await GetModel(larsCode);

            return View(ViewPath, model);
        }

        var command = new AddProviderCourseLocationCommand()
        {
            Ukprn = base.Ukprn,
            UserId = base.UserId,
            UserDisplayName = base.UserDisplayName,
            LarsCode = larsCode,
            LocationNavigationId = Guid.Parse(submitModel.TrainingVenueNavigationId),
            HasDayReleaseDeliveryOption = submitModel.HasDayReleaseDeliveryOption,
            HasBlockReleaseDeliveryOption = submitModel.HasBlockReleaseDeliveryOption
        };
        await _mediator.Send(command);

        var sessionModel = _sessionService.Get<EditLocationOptionsSessionModel>();
        if (sessionModel != null && sessionModel.UpdatedLocationOption == LocationOption.ProviderLocation)
        {
            GetProviderCourseDetailsQueryResult existing = await _mediator.Send(new GetProviderCourseDetailsQuery(Ukprn, larsCode));
            if (existing.HasEmployerLocations)
            {
                var deleteEmployerLocationCommand = new DeleteCourseLocationsCommand(Ukprn, larsCode, UserId, UserDisplayName, DeleteProviderCourseLocationOption.DeleteEmployerLocations);
                await _mediator.Send(deleteEmployerLocationCommand);
            }
            _sessionService.Delete(sessionModel);
        }

        return RedirectToRoute(RouteNames.GetProviderCourseLocations, new { ukprn = Ukprn, larsCode });
    }

    private async Task<ProviderCourseLocationAddViewModel> GetModel(string larsCode)
    {
        _logger.LogInformation("Getting available provider course locations for ukprn {Ukprn}  larsCode {LarsCode}", Ukprn, larsCode);
        var result = await _mediator.Send(new GetAvailableProviderLocationsQuery(Ukprn, larsCode));

        var model = new ProviderCourseLocationAddViewModel
        {
            TrainingVenues = result.AvailableProviderLocations.OrderBy(c => c.LocationName).Select(s => new SelectListItem($"{s.LocationName}", s.NavigationId.ToString()))
        };

        return model;
    }
}
