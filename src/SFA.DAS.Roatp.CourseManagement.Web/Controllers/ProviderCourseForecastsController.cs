using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Forecasts;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[Route("{ukprn}")]
public class ProviderCourseForecastsController(IMediator _mediator) : ControllerBase
{
    public const string AllCoursesViewPath = "~/Views/Forecasts/AllCourses.cshtml";
    public const string CourseForecastsViewPath = "~/Views/Forecasts/CourseForecasts.cshtml";

    [HttpGet]
    [Route("forecasts/courses", Name = RouteNames.ForecastCourses)]
    public async Task<IActionResult> GetProviderCoursesForForecasts(CancellationToken cancellationToken)
    {
        var standards = await GetShortCourses(cancellationToken);
        ForecastCoursesViewModel model = new()
        {
            ApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit,
            CourseLinks = new(standards.Select(s => new CourseLink(s.DisplayName, Url.RouteUrl(RouteNames.CourseForecasts, new { Ukprn, s.LarsCode }))).OrderBy(c => c.Name))
        };
        return View(AllCoursesViewPath, model);
    }

    private async Task<List<Standard>> GetShortCourses(CancellationToken cancellationToken)
    {
        GetAllProviderStandardsQueryResult result = await _mediator.Send(new GetAllProviderStandardsQuery(Ukprn, CourseType.ShortCourse), cancellationToken);
        return result.Standards;
    }

    [HttpGet]
    [Route("forecasts/courses/{larsCode}", Name = RouteNames.CourseForecasts)]
    public async Task<IActionResult> GetCourseForecasts(string larsCode, CancellationToken cancellationToken)
    {
        GetProviderCourseForecastsQueryResult result = await _mediator.Send(new GetProviderCourseForecastsQuery(Ukprn, larsCode), cancellationToken);

        CourseForecastsViewModel model = new()
        {
            ApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit,
            Ukprn = Ukprn,
            LarsCode = larsCode,
            LastUpdatedDate = result.Forecasts.Max(f => f.UpdatedDate)?.ToString("dd MMMM yyyy"),
            Forecasts = [.. result.Forecasts.Select(f => (ForecastModel)f)]
        };

        return View(CourseForecastsViewPath, model);
    }

    [HttpPost]
    [Route("forecasts/courses/{larsCode}", Name = RouteNames.CourseForecasts)]
    public async Task<IActionResult> GetCourseForecasts(string larsCode, CourseForecastsSubmitModel submitModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return await GetCourseForecasts(larsCode, cancellationToken);
        }
        return RedirectToRoute(RouteNames.ForecastCourses, new { Ukprn });
    }
}
