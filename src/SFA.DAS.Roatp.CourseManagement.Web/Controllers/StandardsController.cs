using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class StandardsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<StandardsController> _logger;

        public StandardsController(IMediator mediator, ILogger<StandardsController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Route("{ukprn}/standards",Name = RouteNames.ViewStandards)]
        [HttpGet]
        public async Task<IActionResult> ViewStandards()
        {
            var ukprn = HttpContext.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value;

            _logger.LogInformation("Getting standards for {ukprn}", ukprn);

            var result = await _mediator.Send(new GetStandardQuery(int.Parse(ukprn)));

            var model = new StandardListViewModel();
           
            if(result != null)
            {
                model.Standards = result.Standards.Select(c => (StandardViewModel)c).ToList();
            };

            return View("~/Views/Standards/ViewStandards.cshtml", model);
        }
    }
}
