using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourseLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
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
        public async Task<IActionResult> SelectAProviderlocation([FromRoute] int larsCode)
        {
            var model = await GetModel(larsCode);
            return View(ViewPath, model);
        }

        [Route("{ukprn}/standards/{larsCode}/providerlocations/add-new", Name = RouteNames.PostAddProviderCourseLocation)]
        [HttpPost]
        public async Task<IActionResult> SubmitAProviderlocation([FromRoute] int larsCode, ProviderCourseLocationAddSubmitModel submitModel)
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
                LarsCode = larsCode,
                LocationNavigationId = Guid.Parse(submitModel.TrainingVenueNavigationId),
                HasDayReleaseDeliveryOption = submitModel.HasDayReleaseDeliveryOption,
                HasBlockReleaseDeliveryOption = submitModel.HasBlockReleaseDeliveryOption
            };

            await _mediator.Send(command);

            return RedirectToRoute(RouteNames.GetProviderCourseLocations, new { ukprn = Ukprn, larsCode});
        }

        private async Task<ProviderCourseLocationAddViewModel> GetModel(int larsCode)
        {
            var result = await _mediator.Send(new GetAllProviderLocationsQuery(Ukprn));
            var model = new ProviderCourseLocationAddViewModel();
            model.TrainingVenues = result.ProviderLocations.OrderBy(c => c.LocationName).Select(s => new SelectListItem($"{s.LocationName}", s.NavigationId.ToString()));
            model.BackLink = model.CancelLink = Url.RouteUrl(RouteNames.GetProviderCourseLocations, new { ukprn = Ukprn, larsCode });
            return model;
        }
    }
}
