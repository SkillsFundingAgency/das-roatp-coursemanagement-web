using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;

[AuthorizeCourseType(CourseType.Apprenticeship)]
[Route("{ukprn}/standards/add/regions")]
public class AddStandardAddRegionsController : AddAStandardControllerBase
{
    public const string ViewPath = "~/Views/AddAStandard/SelectRegions.cshtml";
    private readonly IMediator _mediator;
    private readonly ILogger<AddStandardAddRegionsController> _logger;
    private readonly IValidator<RegionsSubmitModel> _validator;

    public AddStandardAddRegionsController(IMediator mediator, ILogger<AddStandardAddRegionsController> logger, ISessionService sessionService, IValidator<RegionsSubmitModel> validator) : base(sessionService)
    {
        _mediator = mediator;
        _logger = logger;
        _validator = validator;
    }

    [HttpGet(Name = RouteNames.GetAddStandardAddRegions)]
    public async Task<IActionResult> SelectRegions()
    {
        var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
        if (sessionModel == null) return redirectResult;

        if (sessionModel.LocationOption == LocationOption.ProviderLocation || sessionModel.HasNationalDeliveryOption.GetValueOrDefault())
        {
            _logger.LogWarning("User: {UserId} unexpectedly landed on regions page when location option is {LocationOption} and national delivery option is {HasNationalDeliveryOption}", UserId, sessionModel.LocationOption, sessionModel.HasNationalDeliveryOption);
            return RedirectToRouteWithUkprn(RouteNames.ViewStandards);
        }

        AddStandardAddRegionsViewModel model = await GetViewModel();
        return View(ViewPath, model);
    }

    [HttpPost(Name = RouteNames.PostAddStandardAddRegions)]
    public async Task<IActionResult> SubmitRegions(RegionsSubmitModel submitModel)
    {
        var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
        if (sessionModel == null) return redirectResult;

        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid)
        {
            AddStandardAddRegionsViewModel model = await GetViewModel();
            ModelState.AddValidationErrors(validatedResult.Errors);
            return View(ViewPath, model);
        }

        var result = await _mediator.Send(new GetAllRegionsAndSubRegionsQuery());
        var selectedRegions = result.Regions.Where(r => submitModel.SelectedSubRegions.Contains(r.Id.ToString())).Select(r => (CourseLocationModel)r);
        sessionModel.CourseLocations.AddRange(selectedRegions);
        _sessionService.Set(sessionModel);

        return RedirectToRouteWithUkprn(RouteNames.GetAddStandardReviewStandard);
    }

    private async Task<AddStandardAddRegionsViewModel> GetViewModel()
    {
        var regions = await _mediator.Send(new GetAllRegionsAndSubRegionsQuery());
        var model = new AddStandardAddRegionsViewModel(regions.Regions.Select(r => (RegionViewModel)r).ToList());
        return model;
    }
}
