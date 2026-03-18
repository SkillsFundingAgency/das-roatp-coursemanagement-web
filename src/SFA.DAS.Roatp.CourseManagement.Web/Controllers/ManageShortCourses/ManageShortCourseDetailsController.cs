using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
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
    public const string ViewPath = "~/Views/ManageShortCourses/ManageShortCourseDetailsView.cshtml";

    [HttpGet]
    public async Task<IActionResult> ManageShortCourseDetails(ApprenticeshipType apprenticeshipType, string larsCode)
    {
        _logger.LogInformation("Getting Course details for ukprn {Ukprn} LarsCode {LarsCode}", Ukprn, larsCode);

        var standardDetailsResponse = await _mediator.Send(new GetStandardDetailsQuery(Ukprn, larsCode));

        if (standardDetailsResponse == null)
        {
            _logger.LogWarning("No data returned for ukprn {Ukprn} and LarsCode {LarsCode} for User: {UserId}. Redirecting to PageNotFound.", Ukprn, larsCode, UserId);

            return View(ViewsPath.PageNotFoundPath);
        }

        var model = (ManageShortCourseDetailsViewModel)standardDetailsResponse;

        model.ApprenticeshipType = apprenticeshipType;
        model.ContactInformation.ApprenticeshipType = apprenticeshipType;
        model.LocationInformation.ApprenticeshipType = apprenticeshipType;
        model.ContactInformation.ContactDetailsChangeLink = Url.RouteUrl(RouteNames.EditShortCourseContactDetails, new { Ukprn, apprenticeshipType, larsCode });
        model.BackToManageShortCoursesLink = Url.RouteUrl(RouteNames.ManageShortCourses, new { ukprn = Ukprn, apprenticeshipType });

        return View(ViewPath, model);
    }
}
