using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

public class ProviderNotRegisteredController : Controller
{
    [HttpGet]
    [Route("{ukprn}/provider-not-registered", Name = RouteNames.ProviderNotRegistered)]
    public IActionResult Index()
    {
        return View("~/Views/ShutterPages/ProviderNotRegistered.cshtml");
    }
}
