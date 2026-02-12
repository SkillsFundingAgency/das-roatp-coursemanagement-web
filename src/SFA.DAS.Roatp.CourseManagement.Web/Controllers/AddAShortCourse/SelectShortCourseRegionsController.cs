using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}/new/select-regions", Name = RouteNames.SelectShortCourseRegions)]
public class SelectShortCourseRegionsController(ILogger<SelectShortCourseRegionsController> _logger, ISessionService _sessionService, IRegionsService _regionsService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddAShortCourse/SelectShortCourseRegionsView.cshtml";

    [HttpGet]
    public async Task<IActionResult> SelectShortCourseRegions(ApprenticeshipType apprenticeshipType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!sessionModel.LocationOptions.Contains(ShortCourseLocationOption.EmployerLocation))
        {
            _logger.LogWarning("User: {UserId} unexpectedly landed on select regions page when location options does not contain employer.", UserId);

            return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);
        }

        var regionsResponse = await _regionsService.GetRegions();

        SelectShortCourseRegionsViewModel model = GetViewModel(regionsResponse, apprenticeshipType);

        foreach (var subregionsGroupedByRegion in model.SubregionsGroupedByRegions)
        {
            foreach (var region in subregionsGroupedByRegion)
            {
                region.IsSelected = sessionModel.TrainingRegions.Any(r => r.SubregionId == region.Id);
            }
        }

        return View(ViewPath, model);
    }

    [HttpPost]
    public async Task<IActionResult> SelectShortCourseRegions(RegionsSubmitModel submitModel, ApprenticeshipType apprenticeshipType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var regionsResponse = await _regionsService.GetRegions();

        if (!ModelState.IsValid)
        {
            SelectShortCourseRegionsViewModel model = GetViewModel(regionsResponse, apprenticeshipType);
            return View(ViewPath, model);
        }

        sessionModel.TrainingRegions = regionsResponse.Where(r => submitModel.SelectedSubRegions.Contains(r.Id.ToString())).Select(r => (TrainingRegionModel)r).ToList();

        _sessionService.Set(sessionModel);

        return RedirectToRoute(RouteNames.SelectShortCourseRegions, new { ukprn = Ukprn, apprenticeshipType });
    }

    private static SelectShortCourseRegionsViewModel GetViewModel(List<RegionModel> regions, ApprenticeshipType apprenticeshipType)
    {
        var model = new SelectShortCourseRegionsViewModel(regions.Select(r => (ShortCourseRegionViewModel)r).ToList());
        model.ShortCourseBaseModel.ApprenticeshipType = apprenticeshipType;
        return model;
    }
}
