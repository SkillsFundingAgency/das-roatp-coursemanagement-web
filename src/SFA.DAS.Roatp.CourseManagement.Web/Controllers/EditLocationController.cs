using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Locations;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    public class EditLocationController : Controller
    {
        private readonly ILogger<EditLocationController> _logger;

        public EditLocationController(ILogger<EditLocationController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new EditLocationViewModel { Name = "Coventry" });
        }
    }
}
