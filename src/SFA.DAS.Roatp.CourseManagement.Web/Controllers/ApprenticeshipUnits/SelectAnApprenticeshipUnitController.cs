using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAnApprenticeshipUnit;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Constants;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ApprenticeshipUnits;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/manage-apprenticeship-units/add/select-apprenticeship-unit", Name = RouteNames.SelectAnApprenticeshipUnit)]
public class SelectAnApprenticeshipUnitController(IMediator _mediator) : ControllerBase
{
    public async Task<IActionResult> Index()
    {
        var viewModel = await GetModel();
        return View(viewModel);
    }

    private async Task<SelectAnApprenticeshipUnitViewModel> GetModel()
    {
        var result = await _mediator.Send(new GetAvailableProviderStandardsQuery(Ukprn, CourseType.ApprenticeshipUnit));
        var model = new SelectAnApprenticeshipUnitViewModel();
        model.ApprenticeshipUnit = result.AvailableCourses.OrderBy(c => c.Title).Select(s => new SelectListItem($"{s.Title} (Level {s.Level})", s.LarsCode.ToString()));
        return model;
    }
}

