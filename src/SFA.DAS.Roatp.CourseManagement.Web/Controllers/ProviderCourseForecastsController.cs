using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
    public const string ViewPath = "~/Views/Forecasts/AllCourses.cshtml";

    [HttpGet]
    [Route("forecasts/courses", Name = RouteNames.ForecastCourses)]
    public async Task<IActionResult> GetProviderCoursesForForecasts(CancellationToken cancellationToken)
    {
        var standards = await GetShortCourses(cancellationToken);
        ForecastCoursesViewModel model = new()
        {
            ApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit,
            CourseLinks = new(standards.Select(s => new CourseLink(s.DisplayName, "#")).OrderBy(c => c.Name))
        };
        return View(ViewPath, model);
    }

    private async Task<List<Standard>> GetShortCourses(CancellationToken cancellationToken)
    {
        GetAllProviderStandardsQueryResult result = await _mediator.Send(new GetAllProviderStandardsQuery(Ukprn, CourseType.ShortCourse), cancellationToken);
        return result.Standards;
    }
}
