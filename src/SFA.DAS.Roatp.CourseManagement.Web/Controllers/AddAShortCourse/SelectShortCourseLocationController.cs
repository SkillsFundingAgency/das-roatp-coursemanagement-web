using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/courses/{courseType}/new/select-course-location", Name = RouteNames.SelectShortCourseLocation)]
public class SelectShortCourseLocationController(ISessionService _sessionService) : ControllerBase
{
    public const string ViewPath = "~/Views/AddAShortCourse/SelectShortCourseLocationView.cshtml";

    [HttpGet]
    public IActionResult SelectShortCourseLocation(CourseType courseType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var model = new SelectShortCourseLocationViewModel()
        {
            ShortCourseLocations = sessionModel.ShortCourseLocations,
            CourseType = courseType,
        };

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult SelectShortCourseLocation(SelectShortCourseLocationSubmitModel submitModel, CourseType courseType)
    {
        var sessionModel = _sessionService.Get<ShortCourseSessionModel>();

        if (sessionModel == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        if (!ModelState.IsValid)
        {
            var model = new SelectShortCourseLocationViewModel()
            {
                ShortCourseLocations = submitModel.ShortCourseLocations,
                CourseType = courseType,
            };

            return View(ViewPath, model);
        }

        sessionModel.ShortCourseLocations = submitModel.ShortCourseLocations;

        sessionModel.HasOnlineDeliveryOption = submitModel.ShortCourseLocations.Contains(ShortCourseLocationOption.Online);

        _sessionService.Set(sessionModel);

        return RedirectToRoute(RouteNames.SelectShortCourseLocation, new { ukprn = Ukprn, courseType });
    }
}
