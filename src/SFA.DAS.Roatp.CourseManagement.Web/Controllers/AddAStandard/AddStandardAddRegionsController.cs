using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class AddStandardAddRegionsController : AddAStandardControllerBase
    {
        public const string ViewPath = "~/Views/AddAStandard/SelectRegions.cshtml";
        private readonly IMediator _mediator;
        private readonly ILogger<AddStandardAddRegionsController> _logger;

        public AddStandardAddRegionsController(IMediator mediator, ILogger<AddStandardAddRegionsController> logger, ISessionService sessionService) : base(sessionService)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Route("{ukprn}/standards/add/regions", Name = RouteNames.GetAddStandardAddRegions)]
        public async Task<IActionResult> SelectRegions()
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;

            if (sessionModel.LocationOption == LocationOption.ProviderLocation || sessionModel.HasNationalDeliveryOption.GetValueOrDefault())
            {
                _logger.LogWarning($"User: {UserId} unexpectedly landed on regions page when location option is {sessionModel.LocationOption} and national delivery option is {sessionModel.HasNationalDeliveryOption}");
                return RedirectToRouteWithUkprn(RouteNames.ViewStandards);
            }

            AddStandardAddRegionsViewModel model = await GetViewModel();
            return View(ViewPath, model);
        }

        [HttpPost]
        [Route("{ukprn}/standards/add/regions", Name = RouteNames.PostAddStandardAddRegions)]
        public async Task<IActionResult> SubmitRegions(RegionsSubmitModel submitModel)
        {
            var (sessionModel, redirectResult) = GetSessionModelWithEscapeRoute(_logger);
            if (sessionModel == null) return redirectResult;

            if (!ModelState.IsValid)
            {
                AddStandardAddRegionsViewModel model = await GetViewModel();
                return View(ViewPath, model);
            }

            var result = await _mediator.Send(new GetAllRegionsAndSubRegionsQuery());
            var selectedRegions = result.Regions.Where(r => submitModel.SelectedSubRegions.Contains(r.Id.ToString())).Select(r => (CourseLocationModel)r);
            sessionModel.CourseLocations.AddRange(selectedRegions);
            _sessionService.Set(sessionModel);

            return RedirectToRouteWithUkprn(RouteNames.GetAddStandardReviewStandard);
        }

        private async Task<AddStandardAddRegionsViewModel> GetViewModel()
        {
            var regions = await _mediator.Send(new GetAllRegionsAndSubRegionsQuery());
            var model = new AddStandardAddRegionsViewModel(regions.Regions.Select(r => (RegionViewModel)r).ToList());
            return model;
        }
    }
}
