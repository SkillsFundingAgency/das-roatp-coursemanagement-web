using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[Route("{ukprn}/review-your-details", Name = RouteNames.ReviewYourDetails)]
public class ReviewYourDetailsController(ISessionService _sessionService, IMediator _mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ReviewYourDetails(CancellationToken cancellationToken)
    {
        _sessionService.Delete(nameof(ProviderContactSessionModel));

        var urlParams = new { Ukprn };

        bool showForecastOption = await HasShortCourses(cancellationToken);

        var model = new ReviewYourDetailsViewModel()
        {
            SelectCourseTypeUrl = Url.RouteUrl(RouteNames.SelectCourseType, urlParams),
            ProviderLocationsUrl = Url.RouteUrl(RouteNames.GetProviderLocations, urlParams),
            ProviderDescriptionUrl = Url.RouteUrl(RouteNames.GetProviderDescription, urlParams),
            ProviderContactUrl = Url.RouteUrl(RouteNames.CheckProviderContactDetails, urlParams),
            ForecastUrl = "#",
            ShowForecastOption = showForecastOption
        };

        return View("ReviewYourDetails", model);
    }

    private async Task<bool> HasShortCourses(CancellationToken cancellationToken)
    {
        GetAllProviderStandardsQueryResult result = await _mediator.Send(new GetAllProviderStandardsQuery(Ukprn, CourseType.ShortCourse), cancellationToken);
        return result.Standards.Any();
    }
}
