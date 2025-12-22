using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAvailableProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourseLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ProviderCourseLocationAddController : ControllerBase
    {
        public const string ViewPath = "~/Views/ProviderCourseLocations/AddTrainingCourseLocation.cshtml";
        private readonly ILogger<ProviderCourseLocationAddController> _logger;
        private readonly IMediator _mediator;

        public ProviderCourseLocationAddController(ILogger<ProviderCourseLocationAddController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Route("{ukprn}/standards/{larsCode}/providerlocations/add-new", Name = RouteNames.GetAddProviderCourseLocation)]
        [HttpGet]
        public async Task<IActionResult> SelectAProviderlocation([FromRoute] string larsCode)
        {
            var model = await GetModel(larsCode);
            return View(ViewPath, model);
        }

        [Route("{ukprn}/standards/{larsCode}/providerlocations/add-new", Name = RouteNames.PostAddProviderCourseLocation)]
        [HttpPost]
        public async Task<IActionResult> SubmitAProviderlocation([FromRoute] string larsCode, ProviderCourseLocationAddSubmitModel submitModel)
        {
            if (!ModelState.IsValid)
            {
                var model = await GetModel(larsCode);
                return View(ViewPath, model);
            }
            var command = new AddProviderCourseLocationCommand()
            {
                Ukprn = base.Ukprn,
                UserId = base.UserId,
                UserDisplayName = base.UserDisplayName,
                LarsCode = larsCode,
                LocationNavigationId = Guid.Parse(submitModel.TrainingVenueNavigationId),
                HasDayReleaseDeliveryOption = submitModel.HasDayReleaseDeliveryOption,
                HasBlockReleaseDeliveryOption = submitModel.HasBlockReleaseDeliveryOption
            };

            await _mediator.Send(command);

            return RedirectToRoute(RouteNames.GetProviderCourseLocations, new { ukprn = Ukprn, larsCode });
        }

        private async Task<ProviderCourseLocationAddViewModel> GetModel(string larsCode)
        {
            _logger.LogInformation("Getting available provider course locations for ukprn {ukprn}  larsCode {larsCode}", Ukprn, larsCode);
            var result = await _mediator.Send(new GetAvailableProviderLocationsQuery(Ukprn, larsCode));

            var model = new ProviderCourseLocationAddViewModel
            {
                TrainingVenues = result.AvailableProviderLocations.OrderBy(c => c.LocationName).Select(s => new SelectListItem($"{s.LocationName}", s.NavigationId.ToString()))
            };

            return model;
        }
    }
}
