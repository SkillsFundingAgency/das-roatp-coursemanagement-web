using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ApprenticeshipUnits;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/manage-apprenticeship-units", Name = RouteNames.ManageApprenticeshipUnits)]
public class ManageApprenticeshipUnitsController(IProviderCourseTypeService _providerCourseTypeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var providerCourseTypeResponse = await _providerCourseTypeService.GetProviderCourseType(Ukprn);

        if (!providerCourseTypeResponse.Any(x => x.CourseType == CourseType.ApprenticeshipUnit))
        {
            return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);
        }

        var selectAnApprenticeshipUnitUrl = Url.RouteUrl(RouteNames.SelectAnApprenticeshipUnit, new { Ukprn });

        var viewModel = new ManageApprenticeshipUnitsViewModel()
        {
            AddAnApprenticeshipUnitLink = selectAnApprenticeshipUnitUrl,
        };

        return View(viewModel);
    }
}
