using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateStandardSubRegions;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllStandardRegions;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}/{larsCode}/edit-regions", Name = RouteNames.EditShortCourseRegions)]
public class EditShortCourseRegionsController(IMediator _mediator, ILogger<EditShortCourseRegionsController> _logger) : ControllerBase
{
    public const string ViewPath = "~/Views/ShortCourses/ShortCourseRegionsView.cshtml";

    [HttpGet]
    public async Task<IActionResult> EditShortCourseRegions(ApprenticeshipType apprenticeshipType, string larsCode)
    {
        var viewModel = await GetViewModel(apprenticeshipType, larsCode);

        if (viewModel == null)
        {
            _logger.LogWarning("No regions returned for LarsCode {LarsCode}. Redirecting to PageNotFound.", larsCode);

            return View(ViewsPath.PageNotFoundPath);
        }

        return View(ViewPath, viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditShortCourseRegions(RegionsSubmitModel submitModel, ApprenticeshipType apprenticeshipType, string larsCode)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = await GetViewModel(apprenticeshipType, larsCode);

            if (viewModel == null)
            {
                _logger.LogWarning("No regions returned for LarsCode {LarsCode}. Redirecting to PageNotFound.", larsCode);

                return View(ViewsPath.PageNotFoundPath);
            }

            viewModel.AllRegions.ForEach(s => s.IsSelected = false);
            return View(ViewPath, viewModel);
        }

        var apiResponse = await _mediator.Send(new GetAllStandardRegionsQuery(Ukprn, larsCode));

        if (apiResponse == null)
        {
            return RedirectToRoute(RouteNames.EditShortCourseRegions, new { Ukprn, apprenticeshipType, larsCode });
        }

        var currentSubRegions = apiResponse.Regions.Where(x => x.IsSelected).Select(x => x.Id).OrderBy(x => x);

        var selectedSubRegions = submitModel.SelectedSubRegions.Select(x => int.Parse(x)).OrderBy(x => x).ToList();

        if (!currentSubRegions.SequenceEqual(selectedSubRegions))
        {
            var command = new UpdateStandardSubRegionsCommand
            {
                LarsCode = larsCode,
                Ukprn = Ukprn,
                UserId = UserId,
                UserDisplayName = UserDisplayName,
                SelectedSubRegions = submitModel.SelectedSubRegions.Select(subregion => int.Parse(subregion)).ToList()
            };

            await _mediator.Send(command);
        }

        return RedirectToRoute(RouteNames.ManageShortCourseDetails, new { Ukprn, apprenticeshipType, larsCode });
    }

    private async Task<SelectShortCourseRegionsViewModel> GetViewModel(ApprenticeshipType apprenticeshipType, string larsCode)
    {
        var result = await _mediator.Send(new GetAllStandardRegionsQuery(Ukprn, larsCode));

        if (result == null)
        {
            return null;
        }

        var model = new SelectShortCourseRegionsViewModel(result.Regions.Select(r => (ShortCourseRegionViewModel)r).ToList());
        model.ShortCourseBaseModel.ApprenticeshipType = apprenticeshipType;
        model.SubmitButtonText = ButtonText.Confirm;
        model.Route = RouteNames.EditShortCourseRegions;
        model.IsAddJourney = false;

        return model;
    }
}
