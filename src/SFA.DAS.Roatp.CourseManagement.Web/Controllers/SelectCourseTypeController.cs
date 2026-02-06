using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
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

        if (providerCourseTypeResponse.Count == 0)
        {
            return RedirectToRouteWithUkprn(RouteNames.ReviewYourDetails);
        }

        if (providerCourseTypeResponse.Count == 1)
        {
            if (providerCourseTypeResponse.Any(x => x.CourseType == CourseType.Apprenticeship))
            {
                return RedirectToRouteWithUkprn(RouteNames.ViewStandards);
            }

            if (providerCourseTypeResponse.Any(x => x.CourseType == CourseType.ShortCourse))
            {
                return RedirectToRoute(RouteNames.ManageShortCourses, new { ukprn = Ukprn, apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit });
            }
        }

        var viewModel = new SelectCourseTypeViewModel()
        {
            ApprenticeshipsUrl = Url.RouteUrl(RouteNames.ViewStandards, new { ukprn = Ukprn, }),
            ApprenticeshipUnitsUrl = Url.RouteUrl(RouteNames.ManageShortCourses, new { ukprn = Ukprn, apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit })
        };

        return View(viewModel);
    }
}