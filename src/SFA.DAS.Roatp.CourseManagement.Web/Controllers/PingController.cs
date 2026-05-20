using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[Route("ping")]
public class PingController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return new OkObjectResult("Pong");
    }
}