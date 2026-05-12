using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateStandardSubRegions;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}/{larsCode}/edit-regions", Name = RouteNames.EditShortCourseRegions)]
public class EditShortCourseRegionsController(IMediator _mediator, ILogger<EditShortCourseRegionsController> _logger, IRegionsService _regionsService, IValidator<RegionsSubmitModel> _validator) : ControllerBase
{
    public const string ViewPath = "~/Views/ShortCourses/ShortCourseRegions.cshtml";

    [HttpGet]
    public async Task<IActionResult> EditShortCourseRegions(ApprenticeshipType apprenticeshipType, string larsCode)
    {
        var providerCourseDetailsResponse = await GetProviderCourseDetails(larsCode);

        if (providerCourseDetailsResponse == null)
        {
            _logger.LogWarning("No data returned for ukprn {Ukprn} and LarsCode {LarsCode} for User: {UserId}. Redirecting to PageNotFound.", Ukprn, larsCode, UserId);

            return View(ViewsPath.PageNotFoundPath);
        }

        var regionsResponse = await _regionsService.GetRegions();

        var viewModel = GetViewModel(regionsResponse, providerCourseDetailsResponse, apprenticeshipType);

        return View(ViewPath, viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditShortCourseRegions(RegionsSubmitModel submitModel, ApprenticeshipType apprenticeshipType, string larsCode)
    {
        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid) ModelState.AddValidationErrors(validatedResult.Errors);

        if (!ModelState.IsValid)
        {
            var regionsResponse = await _regionsService.GetRegions();

            var viewModel = GetViewModel(regionsResponse, new GetProviderCourseDetailsQueryResult(), apprenticeshipType);

            return View(ViewPath, viewModel);
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

        return RedirectToRoute(RouteNames.ManageShortCourseDetails, new { Ukprn, apprenticeshipType, larsCode });
    }

    private async Task<GetProviderCourseDetailsQueryResult> GetProviderCourseDetails(string larsCode)
    {
        _logger.LogInformation("Getting provider course details for ukprn {Ukprn} and lasrcode {LarsCode}", Ukprn, larsCode);

        var result = await _mediator.Send(new GetProviderCourseDetailsQuery(Ukprn, larsCode));

        return result;
    }

    private static SelectShortCourseRegionsViewModel GetViewModel(List<RegionModel> regions, GetProviderCourseDetailsQueryResult providerCourseDetails, ApprenticeshipType apprenticeshipType)
    {
        var model = new SelectShortCourseRegionsViewModel(regions.Select(r => (ShortCourseRegionViewModel)r).ToList());
        model.ApprenticeshipType = apprenticeshipType;
        model.SubmitButtonText = ButtonText.Confirm;
        model.Route = RouteNames.EditShortCourseRegions;
        model.IsAddJourney = false;

        foreach (var subregionsGroupedByRegion in model.SubregionsGroupedByRegions)
        {
            foreach (var region in subregionsGroupedByRegion)
            {
                region.IsSelected = providerCourseDetails.ProviderCourseLocations.Any(r => r.SubregionName == region.SubregionName && r.LocationType == LocationType.Regional);
            }
        }

        return model;
    }
}
