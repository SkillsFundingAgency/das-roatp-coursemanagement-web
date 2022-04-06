using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    public class PingController : Controller
    {
        [HttpGet]
        [Route("ping")]
        public IActionResult Index()
        {
            return Ok("Pong");
        }
    }
}
