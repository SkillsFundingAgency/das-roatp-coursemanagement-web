using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn}/select-course-type", Name = RouteNames.SelectCourseType)]
public class SelectCourseTypeController(IProviderCourseTypeService _providerCourseTypeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var providerCourseTypeResponse = await _providerCourseTypeService.GetProviderCourseType(Ukprn);

        if (providerCourseTypeResponse.Any(x => x.CourseType == CourseType.Apprenticeship) &&
            providerCourseTypeResponse.Any(x => x.CourseType == CourseType.ApprenticeshipUnit))
        {
            var viewModel = new SelectCourseTypeViewModel()
            {
                ApprenticeshipsUrl = Url.RouteUrl(RouteNames.ViewStandards, new { ukprn = Ukprn, }),
                ApprenticeshipUnitsUrl = Url.RouteUrl(RouteNames.ManageApprenticeshipUnits, new { ukprn = Ukprn })
            };

            return View(viewModel);
        }

        if (providerCourseTypeResponse.Any(x => x.CourseType == CourseType.Apprenticeship))
        {
            return RedirectToRouteWithUkprn(RouteNames.ViewStandards);
        }

        if (providerCourseTypeResponse.Any(x => x.CourseType == CourseType.ApprenticeshipUnit))
        {
            return RedirectToRouteWithUkprn(RouteNames.ManageApprenticeshipUnits);
        }

        return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);
    }
}