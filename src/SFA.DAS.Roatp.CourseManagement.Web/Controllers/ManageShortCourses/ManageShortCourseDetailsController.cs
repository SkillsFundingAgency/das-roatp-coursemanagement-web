using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}/{larsCode}", Name = RouteNames.ManageShortCourseDetails)]
public class ManageShortCourseDetailsController(IMediator _mediator, ILogger<ManageShortCourseDetailsController> _logger) : ControllerBase
{
    public const string ViewPath = "~/Views/ShortCourses/ManageShortCourses/ManageShortCourseDetails.cshtml";

    [HttpGet]
    [ClearSession(SessionKeys.SelectedShortCourseLocationOption)]
    public async Task<IActionResult> ManageShortCourseDetails(ApprenticeshipType apprenticeshipType, string larsCode)
    {
        _logger.LogInformation("Getting Course details for ukprn {Ukprn} LarsCode {LarsCode}", Ukprn, larsCode);

        var providerCourseDetailsResponse = await _mediator.Send(new GetProviderCourseDetailsQuery(Ukprn, larsCode));

        if (providerCourseDetailsResponse == null)
        {
            _logger.LogWarning("No data returned for ukprn {Ukprn} and LarsCode {LarsCode} for User: {UserId}. Redirecting to PageNotFound.", Ukprn, larsCode, UserId);

            return View(ViewsPath.PageNotFoundPath);
        }

        var model = (ManageShortCourseDetailsViewModel)providerCourseDetailsResponse;

        model.ApprenticeshipType = apprenticeshipType;
        model.ContactInformation.ApprenticeshipType = apprenticeshipType;
        model.LocationInformation.ApprenticeshipType = apprenticeshipType;
        model.ContactInformation.ContactDetailsChangeLink = Url.RouteUrl(RouteNames.EditShortCourseContactDetails, new { Ukprn, apprenticeshipType, larsCode });
        model.LocationInformation.TrainingRegionsChangeLink = Url.RouteUrl(RouteNames.EditShortCourseRegions, new { Ukprn, apprenticeshipType, larsCode });
        model.LocationInformation.TrainingVenuesChangeLink = Url.RouteUrl(RouteNames.EditShortCourseTrainingVenues, new { Ukprn, apprenticeshipType, larsCode });
        model.LocationInformation.NationalProviderChangeLink = Url.RouteUrl(RouteNames.EditShortCourseNationalDelivery, new { Ukprn, apprenticeshipType, larsCode });
        model.LocationInformation.LocationOptionsChangeLink = Url.RouteUrl(RouteNames.EditShortCourseLocationOptions, new { Ukprn, apprenticeshipType, larsCode });
        model.BackToManageShortCoursesLink = Url.RouteUrl(RouteNames.ManageShortCourses, new { ukprn = Ukprn, apprenticeshipType });
        model.DeleteShortCourseLink = Url.RouteUrl(RouteNames.DeleteShortCourse, new { Ukprn, apprenticeshipType, larsCode });
        model.Banner.ApprenticeshipType = model.ApprenticeshipTypeLower;

        return View(ViewPath, model);
    }
}
