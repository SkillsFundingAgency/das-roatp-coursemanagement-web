using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class EditLocationOptionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EditLocationOptionController> _logger;
        private int LarsCode;

        public EditLocationOptionController(IMediator mediator, ILogger<EditLocationOptionController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Route("{ukprn}/standards/{larsCode}/edit-location-option", Name = RouteNames.GetLocationOption)]
        [ClearSession(SessionKeys.SelectedLocationOption)]
        public async Task<IActionResult> Index([FromRoute] int larsCode)
        {
            LarsCode = larsCode;
            var result = await _mediator.Send(new GetStandardDetailsQuery(Ukprn, larsCode));
            var model = new EditLocationOptionViewModel();
            model.LocationOption = GetLocationOption(result.StandardDetails.ProviderCourseLocations);
            _logger.LogInformation("For Ukprn:{Ukprn} LarsCode:{LarsCode} the location option is set to {locationOption}", Ukprn, larsCode, model.LocationOption);
            model.BackLink = model.CancelLink = GetStandardDetailsUrl(larsCode);
            return View(model);
        }

        [HttpPost]
        [Route("{ukprn}/standards/{larsCode}/edit-location-option", Name = RouteNames.PostLocationOption)]
        public IActionResult Index([FromRoute] int larsCode, EditLocationOptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.BackLink = model.CancelLink = GetStandardDetailsUrl(larsCode);
                return View(model);
            }
            _logger.LogInformation("For Ukprn:{Ukprn} LarsCode:{LarsCode} the location option is being updated to {locationOption}", Ukprn, larsCode, model.LocationOption);
            HttpContext.Session.SetString(SessionKeys.SelectedLocationOption, model.LocationOption.ToString());
            return View(model);
        }

        private LocationOption? GetLocationOption(List<Domain.ApiModels.ProviderCourseLocation> providerCourseLocations)
        {
            if (!providerCourseLocations.Any()) return null;

            var hasProviderLocation = providerCourseLocations.Any(l => l.LocationType == Domain.ApiModels.LocationType.Provider);
            var hasNationalLocation = providerCourseLocations.Any(l => l.LocationType == Domain.ApiModels.LocationType.National);
            var hasRegionalLocation = providerCourseLocations.Any(l => l.LocationType == Domain.ApiModels.LocationType.Regional);

            _logger.LogInformation($"Locations for Ukprn:{Ukprn} LarsCode:{LarsCode} HasProviderLocation:{hasProviderLocation} HasNationalLocation:{hasNationalLocation} HasRegionalLocation:{hasRegionalLocation}");

            if (hasNationalLocation && hasRegionalLocation)
            {
                _logger.LogWarning("Ukprn:{Ukprn} LarsCode:{LarsCode} has both National and Regional locations", Ukprn, LarsCode);
            }

            if (hasProviderLocation && !hasNationalLocation && !hasRegionalLocation)
                return LocationOption.ProviderLocation;
            else if (!hasProviderLocation && (hasNationalLocation || hasRegionalLocation))
                return LocationOption.EmployerLocation;
            else
                return LocationOption.Both;
        }
    }
}
