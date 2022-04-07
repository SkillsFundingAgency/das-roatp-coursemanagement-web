using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Locations;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [ExcludeFromCodeCoverage] // just a plcase holder at the moment, will be removed or repurposed 
    public class EditLocationController : Controller
    {
        private readonly ILogger<EditLocationController> _logger;

        public EditLocationController(ILogger<EditLocationController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Get edit location in course management.");
            return View(new EditLocationViewModel { Name = "Coventry" });
        }
    }
}
