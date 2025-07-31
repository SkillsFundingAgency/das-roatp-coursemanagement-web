using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [AllowAnonymous]
    [HideNavigationBar]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("Error/{statuscode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 403:
                case 404:
                    return View("PageNotFound");
                default:
                    return View("ErrorInService");
            }
        }

        [Route("Error")]
        public IActionResult ErrorInService()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (User.Identity.IsAuthenticated)
            {
                _logger.LogError(feature.Error, "Unexpected error occurred during request to path: {Path} by user: {User}", feature.Path, User.FindFirstValue(ProviderClaims.UserId));
            }
            else
            {
                _logger.LogError(feature.Error, "Unexpected error occurred during request to {Path}", feature.Path);
            }
            return View();
        }
    }
}
