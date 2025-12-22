using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
public class SelectTrainingTypeController : ControllerBase
{
    [Route("{ukprn}/choose-training-type", Name = RouteNames.SelectTrainingType)]
    [HttpGet]
    public IActionResult Index()
    {
        var viewModel = new SelectTrainingTypeViewModel()
        {
            ApprenticeshipsUrl = Url.RouteUrl(RouteNames.ViewStandards, new { ukprn = Ukprn, }),
            ApprenticeshipUnitsUrl = Url.RouteUrl(RouteNames.SelectTrainingType, new { ukprn = Ukprn })
        };

        return View(viewModel);
    }
}
