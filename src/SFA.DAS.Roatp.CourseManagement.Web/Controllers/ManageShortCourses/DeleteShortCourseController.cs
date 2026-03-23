using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}/{larsCode}/delete-course", Name = RouteNames.DeleteShortCourse)]
public class DeleteShortCourseController(IMediator _mediator, ILogger<DeleteShortCourseController> _logger) : ControllerBase
{
    public const string ViewPath = "~/Views/ManageShortCourses/DeleteShortCourseView.cshtml";

    [HttpGet]
    public async Task<IActionResult> DeleteShortCourse(ApprenticeshipType apprenticeshipType, string larsCode)
    {
        var courseDetailsResponse = await _mediator.Send(new GetStandardDetailsQuery(Ukprn, larsCode));

        if (courseDetailsResponse == null)
        {
            _logger.LogWarning("No data returned for ukprn {Ukprn} and LarsCode {LarsCode} for User: {UserId}. Redirecting to PageNotFound.", Ukprn, larsCode, UserId);

            return View(ViewsPath.PageNotFoundPath);
        }

        var courseInformationResponse = await _mediator.Send(new GetStandardInformationQuery(larsCode));

        if (courseInformationResponse == null)
        {
            _logger.LogWarning("Course information not found for LarsCode {LarsCode}. Redirecting to PageNotFound.", larsCode);

            return View(ViewsPath.PageNotFoundPath);
        }

        var model = (DeleteShortCourseViewModel)courseInformationResponse;
        model.ApprenticeshipType = apprenticeshipType;
        model.BackToManageShortCoursesLink = Url.RouteUrl(RouteNames.ManageShortCourses, new { ukprn = Ukprn, apprenticeshipType });

        return View(ViewPath, model);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteShortCourse(ApprenticeshipType apprenticeshipType, DeleteShortCourseSubmitModel model)
    {
        var command = new DeleteProviderCourseCommand(Ukprn, model.LarsCode, UserId, UserDisplayName);

        await _mediator.Send(command);

        TempData.Add(TempDataKeys.ShowShortCourseDeletedBannerTempDataKey, true);

        return RedirectToRoute(RouteNames.ManageShortCourses, new { ukprn = Ukprn, apprenticeshipType });
    }
}
