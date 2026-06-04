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
[CheckCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{learningType}/{larsCode}", Name = RouteNames.ManageShortCourseDetails)]
public class ManageShortCourseDetailsController(IMediator _mediator, ILogger<ManageShortCourseDetailsController> _logger) : ControllerBase
{
    public const string ViewPath = "~/Views/ShortCourses/ManageShortCourses/ManageShortCourseDetails.cshtml";

    [HttpGet]
    [ClearSession(SessionKeys.SelectedShortCourseLocationOption)]
    public async Task<IActionResult> ManageShortCourseDetails(LearningType learningType, string larsCode)
    {
        _logger.LogInformation("Getting Course details for ukprn {Ukprn} LarsCode {LarsCode}", Ukprn, larsCode);

        var providerCourseDetailsResponse = await _mediator.Send(new GetProviderCourseDetailsQuery(Ukprn, larsCode));

        if (providerCourseDetailsResponse == null)
        {
            _logger.LogWarning("No data returned for ukprn {Ukprn} and LarsCode {LarsCode} for User: {UserId}. Redirecting to PageNotFound.", Ukprn, larsCode, UserId);

            return View(ViewsPath.PageNotFoundPath);
        }

        var model = (ManageShortCourseDetailsViewModel)providerCourseDetailsResponse;

        model.LearningType = learningType;
        model.ContactInformation.LearningType = learningType;
        model.LocationInformation.LearningType = learningType;
        model.ContactInformation.ContactDetailsChangeLink = Url.RouteUrl(RouteNames.EditShortCourseContactDetails, new { Ukprn, learningType, larsCode });
        model.LocationInformation.TrainingRegionsChangeLink = Url.RouteUrl(RouteNames.EditShortCourseRegions, new { Ukprn, learningType, larsCode });
        model.LocationInformation.TrainingVenuesChangeLink = Url.RouteUrl(RouteNames.EditShortCourseTrainingVenues, new { Ukprn, learningType, larsCode });
        model.LocationInformation.NationalProviderChangeLink = Url.RouteUrl(RouteNames.EditShortCourseNationalDelivery, new { Ukprn, learningType, larsCode });
        model.LocationInformation.LocationOptionsChangeLink = Url.RouteUrl(RouteNames.EditShortCourseLocationOptions, new { Ukprn, learningType, larsCode });
        model.BackToManageShortCoursesLink = Url.RouteUrl(RouteNames.ManageShortCourses, new { ukprn = Ukprn, learningType });
        model.DeleteShortCourseLink = Url.RouteUrl(RouteNames.DeleteShortCourse, new { Ukprn, learningType, larsCode });
        model.Banner.LearningType = model.LearningTypeLower;

        return View(ViewPath, model);
    }
}
