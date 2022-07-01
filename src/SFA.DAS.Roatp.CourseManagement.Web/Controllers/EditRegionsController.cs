using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.Regions.Commands.UpdateSubRegions;
using SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries.GetAllRegions;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class EditRegionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EditRegionsController> _logger;
        public EditRegionsController(IMediator mediator, ILogger<EditRegionsController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Route("{ukprn}/standards/{larsCode}/regional-locations", Name = RouteNames.GetSubRegions)]
        [HttpGet]
        public async Task<IActionResult> GetAllRegions(int larsCode)
        {
            _logger.LogInformation("Getting All Sub Regions");
            var model = await BuildRegionsViewModel(larsCode);
            if(!model.AllRegions.Any())
            {
                _logger.LogError("Sub Regions not found");
                return Redirect($"Error/{HttpStatusCode.NotFound}");
            }
            return View("~/Views/Standards/EditRegions.cshtml", model);
        }

        private async Task<RegionsViewModel> BuildRegionsViewModel(int larsCode)
        {
            var result = await _mediator.Send(new GetAllRegionsQuery(Ukprn, larsCode));

            if (result == null)
            {
                var message = $"Sub Regions not found";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            RegionsViewModel model = new RegionsViewModel();
            model.BackUrl = model.CancelLink = Url.RouteUrl(RouteNames.ViewStandardDetails, new { Ukprn, larsCode });

            model.AllRegions = result.Regions.Select(c => (RegionViewModel)c).ToList();
            return model;
        }

        [Route("{ukprn}/standards/{larscode}/regional-locations", Name = RouteNames.PostSubRegions)]
        [HttpPost]
        public async Task<IActionResult> UpdateSubRegions(RegionsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await BuildRegionsViewModel(model.LarsCode);
                model.AllRegions.ForEach(s => s.IsSelected = false);
                return View("~/Views/Standards/EditRegions.cshtml", model);
            }

            var command = (UpdateSubRegionsCommand)model;
            command.Ukprn = Ukprn;
            command.UserId = UserId;
            command.SelectedSubRegions = model.SelectedSubRegions.Select(subregion => int.Parse(subregion)).ToList();

            await _mediator.Send(command);

            return RedirectToRoute(RouteNames.ViewStandardDetails, new { Ukprn, model.LarsCode });
        }
    }
}
