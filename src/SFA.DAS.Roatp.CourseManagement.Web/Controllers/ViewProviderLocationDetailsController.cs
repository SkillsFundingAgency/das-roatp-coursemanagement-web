using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[Route("/providers/{ukprn}/locations/{Id}")]
public class ViewProviderLocationDetailsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ViewProviderLocationDetailsController> _logger;

    public ViewProviderLocationDetailsController(IMediator mediator, ILogger<ViewProviderLocationDetailsController> logger)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet(Name = RouteNames.GetProviderLocationDetails)]
    public async Task<IActionResult> GetProviderLocationDetails([FromRoute] Guid Id)
    {
        _logger.LogInformation("Getting Provider Location for {Ukprn}, {Id}", Ukprn, Id);

        var result = await _mediator.Send(new GetProviderLocationDetailsQuery(Ukprn, Id));

        if (result == null || result.ProviderLocation == null)
        {
            _logger.LogInformation("Provider Location Details not found for {Ukprn} and {Id}", Ukprn, Id);
            return View(ViewsPath.PageNotFoundPath);
        }

        var model = (ProviderLocationViewModel)result.ProviderLocation;

        model.DeleteLocationUrl = Url.RouteUrl(RouteNames.GetConfirmDeleteLocation, new { ukprn = Ukprn, id = Id });

        model.UpdateContactDetailsUrl = Url.RouteUrl(RouteNames.GetUpdateProviderLocationDetails, new { ukprn = Ukprn, Id });
        model.ManageYourStandardsUrl = GetUrlWithUkprn(RouteNames.SelectCourseType);
        model.TrainingVenuesUrl = Url.RouteUrl(RouteNames.GetProviderLocations, new { ukprn = Ukprn });

        model.HasCourses = result.ProviderLocation.Standards.Count > 0;
        model.ShowStandards = result.ProviderLocation.Standards.Any(s => s.LearningType == LearningType.Apprenticeship);
        model.ShowApprenticeshipUnits = result.ProviderLocation.Standards.Any(s => s.LearningType == LearningType.ApprenticeshipUnit);

        model.StandardLinks = new ProviderLocationCourseLinksViewModel(model.Standards
        .Where(s => s.LearningType == LearningType.Apprenticeship)
        .Select(s => new ProviderLocationCourseLink(s.CourseDisplayName, Url.RouteUrl(RouteNames.GetStandardDetails, new { Ukprn, s.LarsCode })))
        .OrderBy(c => c.CourseName));

        model.ApprenticeshipUnitLinks = new ProviderLocationCourseLinksViewModel(model.Standards
        .Where(s => s.LearningType == LearningType.ApprenticeshipUnit)
        .Select(s => new ProviderLocationCourseLink(s.CourseDisplayName, Url.RouteUrl(RouteNames.ManageShortCourseDetails, new { Ukprn, LearningType = LearningType.ApprenticeshipUnit, s.LarsCode })))
        .OrderBy(c => c.CourseName));

        return View("~/Views/EditProviderLocation/ViewProviderLocationsDetails.cshtml", model);
    }
}
