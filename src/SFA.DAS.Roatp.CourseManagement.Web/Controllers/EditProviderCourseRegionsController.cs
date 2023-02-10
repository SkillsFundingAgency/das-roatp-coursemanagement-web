using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateStandardSubRegions;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllStandardRegions;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [DasAuthorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class EditProviderCourseRegionsController : ControllerBase
    {
        public const string ViewPath = "~/Views/Standards/EditProviderCourseRegions.cshtml";
        private readonly IMediator _mediator;
        private readonly ILogger<EditProviderCourseRegionsController> _logger;
        public EditProviderCourseRegionsController(IMediator mediator, ILogger<EditProviderCourseRegionsController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Route("{ukprn}/standards/{larsCode}/regional-locations", Name = RouteNames.GetStandardSubRegions)]
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
            return View(ViewPath, model);
        }

        private async Task<RegionsViewModel> BuildRegionsViewModel(int larsCode)
        {
            var result = await _mediator.Send(new GetAllStandardRegionsQuery(Ukprn, larsCode));

            if (result == null)
            {
                var message = $"Sub Regions not found";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            RegionsViewModel model = new RegionsViewModel();
            model.BackUrl = model.CancelLink = Url.RouteUrl(RouteNames.GetStandardDetails, new { Ukprn, larsCode });

            model.AllRegions = result.Regions.Select(c => (RegionViewModel)c).ToList();
            return model;
        }

        [Route("{ukprn}/standards/{larscode}/regional-locations", Name = RouteNames.PostStandardSubRegions)]
        [HttpPost]
        public async Task<IActionResult> UpdateStandardSubRegions([FromRoute] int larsCode, RegionsSubmitModel submitModel)
        {
            if (!ModelState.IsValid)
            {
                var model = await BuildRegionsViewModel(larsCode);
                model.AllRegions.ForEach(s => s.IsSelected = false);
                return View(ViewPath, model);
            }

            var command = new UpdateStandardSubRegionsCommand
            {
                LarsCode = larsCode,
                Ukprn = Ukprn,
                UserId = UserId,
                UserDisplayName = UserDisplayName,
                SelectedSubRegions = submitModel.SelectedSubRegions.Select(subregion => int.Parse(subregion)).ToList()
            };

            await _mediator.Send(command);

            return RedirectToRoute(RouteNames.GetStandardDetails, new { Ukprn, larsCode });
        }
    }
}
