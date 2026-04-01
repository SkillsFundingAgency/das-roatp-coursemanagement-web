using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[AllowAnonymous]
[Route("Error")]
public class ErrorController : Controller
{
    public const string ProviderNotRegisteredViewName = "ProviderNotRegistered";
    public const string PageNotFoundViewName = "PageNotFound";
    public const string ErrorInServiceViewName = "ErrorInService";

    private readonly ILogger<ErrorController> _logger;
    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [Route("{statuscode}")]
    public IActionResult HttpStatusCodeHandler([FromRoute] int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status403Forbidden => View(ProviderNotRegisteredViewName),
            StatusCodes.Status404NotFound => View(PageNotFoundViewName),
            _ => View(ErrorInServiceViewName)
        };
    }

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
