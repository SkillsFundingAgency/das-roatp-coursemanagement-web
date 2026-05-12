using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateStandardSubRegions;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllStandardRegions;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[AuthorizeCourseType(CourseType.Apprenticeship)]
[Route("{ukprn}/standards/{larsCode}/regional-locations")]
public class EditProviderCourseRegionsController : ControllerBase
{
    public const string ViewPath = "~/Views/Standards/EditProviderCourseRegions.cshtml";
    private readonly IMediator _mediator;
    private readonly ILogger<EditProviderCourseRegionsController> _logger;
    private readonly IValidator<RegionsSubmitModel> _validator;
    public EditProviderCourseRegionsController(IMediator mediator, ILogger<EditProviderCourseRegionsController> logger, IValidator<RegionsSubmitModel> validator)
    {
        _logger = logger;
        _mediator = mediator;
        _validator = validator;
    }

    [HttpGet(Name = RouteNames.GetStandardSubRegions)]
    public async Task<IActionResult> GetAllRegions(string larsCode)
    {
        _logger.LogInformation("Getting All Sub Regions");
        var model = await BuildRegionsViewModel(larsCode);
        if (!model.AllRegions.Any())
        {
            _logger.LogError("Sub Regions not found");
            return Redirect($"Error/{HttpStatusCode.NotFound}");
        }
        return View(ViewPath, model);
    }

    private async Task<RegionsViewModel> BuildRegionsViewModel(string larsCode)
    {
        var result = await _mediator.Send(new GetAllStandardRegionsQuery(Ukprn, larsCode));

        if (result == null)
        {
            var message = $"Sub Regions not found";
            _logger.LogError(message);
            throw new InvalidOperationException(message);
        }

        RegionsViewModel model = new RegionsViewModel();

        model.AllRegions = result.Regions.Select(c => (RegionViewModel)c).ToList();
        return model;
    }

    [HttpPost(Name = RouteNames.PostStandardSubRegions)]
    public async Task<IActionResult> UpdateStandardSubRegions([FromRoute] string larsCode, RegionsSubmitModel submitModel)
    {
        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid) ModelState.AddValidationErrors(validatedResult.Errors);

        if (!ModelState.IsValid)
        {
            var model = await BuildRegionsViewModel(larsCode);
            model.AllRegions.ForEach(s => s.IsSelected = false);
            return View(ViewPath, model);
        }

        var command = new UpdateStandardSubRegionsCommand
        {
            LarsCode = larsCode,
            Ukprn = Ukprn,
            UserId = UserId,
            UserDisplayName = UserDisplayName,
            SelectedSubRegions = submitModel.SelectedSubRegions.Select(subregion => int.Parse(subregion)).ToList()
        };

        await _mediator.Send(command);

        return RedirectToRoute(RouteNames.GetStandardDetails, new { Ukprn, larsCode });
    }
}
