using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/courses/{courseType}/new/select-course-venue", Name = RouteNames.SelectShortCourseTrainingVenue)]
public class SelectShortCourseTrainingVenueController(ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddAShortCourse/SelectShortCourseTrainingVenueView.cshtml";

    [HttpGet]
    public IActionResult SelectShortCourseTrainingVenue(CourseType courseType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var model = new SelectShortCourseTrainingVenueViewModel()
        {
            ProviderLocations = sessionModel.ProviderLocations,
            CourseType = courseType,
        };

        return View(ViewPath, model);
    }
}
