using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class EditLocationOptionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EditLocationOptionController> _logger;
        private readonly ISessionService _sessionService;

        public EditLocationOptionController(IMediator mediator, ILogger<EditLocationOptionController> logger, ISessionService sessionService)
        {
            _mediator = mediator;
            _logger = logger;
            _sessionService = sessionService;
        }

        [HttpGet]
        [Route("{ukprn}/standards/{larsCode}/edit-location-option", Name = RouteNames.GetLocationOption)]
        [ClearSession(SessionKeys.SelectedLocationOption)]
        public async Task<IActionResult> Index([FromRoute] int larsCode)
        {
            var result = await _mediator.Send(new GetStandardDetailsQuery(Ukprn, larsCode));
            var model = new EditLocationOptionViewModel();
            model.LocationOption = result.LocationOption;
            _logger.LogInformation("For Ukprn:{Ukprn} LarsCode:{LarsCode} the location option is set to {locationOption}", Ukprn, larsCode, model.LocationOption);
            model.BackLink = model.CancelLink = GetStandardDetailsUrl(larsCode);

            _sessionService.Delete(SessionKeys.SelectedLocationOption);

            return View(model);
        }

        [HttpPost]
        [Route("{ukprn}/standards/{larsCode}/edit-location-option", Name = RouteNames.PostLocationOption)]
        public async Task<IActionResult> Index([FromRoute] int larsCode, EditLocationOptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.BackLink = model.CancelLink = GetStandardDetailsUrl(larsCode);
                return View(model);
            }
            _logger.LogInformation("For Ukprn:{Ukprn} LarsCode:{LarsCode} the location option is being updated to {locationOption}", Ukprn, larsCode, model.LocationOption);

            switch (model.LocationOption)
            {
                case LocationOption.ProviderLocation:
                case LocationOption.EmployerLocation:
                    var deleteOption = 
                        model.LocationOption == LocationOption.ProviderLocation ? 
                        DeleteProviderCourseLocationOption.DeleteEmployerLocations :
                        DeleteProviderCourseLocationOption.DeleteProviderLocations;
                    var command = new DeleteCourseLocationsCommand(Ukprn, larsCode, UserId, deleteOption);
                    await _mediator.Send(command);
                    break;
                default:
                    break;
            }

            _sessionService.Set(SessionKeys.SelectedLocationOption, model.LocationOption.ToString());
            return View(model);
        }
    }
}
