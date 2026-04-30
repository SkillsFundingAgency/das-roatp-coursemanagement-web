using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;


namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[AuthorizeCourseType(CourseType.Apprenticeship)]
[Route("{ukprn}/standards/{larsCode}/delete-standard")]
public class ProviderCourseDeleteController : ControllerBase
{
    public const string ViewPath = "~/Views/Standards/ConfirmDeleteStandard.cshtml";
    private readonly IMediator _mediator;
    private readonly ILogger<ProviderCourseDeleteController> _logger;

    public ProviderCourseDeleteController(IMediator mediator, ILogger<ProviderCourseDeleteController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet(Name = RouteNames.GetConfirmDeleteStandard)]
    public async Task<IActionResult> GetProviderCourse(string larsCode)
    {
        _logger.LogInformation("Getting Standard information for ukprn {Ukprn} LarsCode {LarsCode}", Ukprn, larsCode);

        var result = await _mediator.Send(new GetStandardInformationQuery(larsCode));

        if (result == null)
        {
            var message = $"Standard Standard information found for larscode {larsCode}";
            _logger.LogError("Standard Standard information found for larscode {LarsCode}", larsCode);
            throw new InvalidOperationException(message);
        }

        var standardResult = await _mediator.Send(new GetProviderCourseDetailsQuery(Ukprn, larsCode));

        if (standardResult == null) return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);

        var model = (ConfirmDeleteStandardViewModel)result;

        return View(ViewPath, model);
    }

    [HttpPost(Name = RouteNames.PostConfirmDeleteStandard)]
    public async Task<IActionResult> DeleteProviderCourse(ConfirmDeleteStandardViewModel model)
    {
        var command = new DeleteProviderCourseCommand(Ukprn, model.StandardInformation.LarsCode, UserId, UserDisplayName);
        await _mediator.Send(command);
        TempData.Add(TempDataKeys.ShowStandardDeletedBannerTempDataKey, true);

        return RedirectToRouteWithUkprn(RouteNames.ViewStandards);
    }
}
