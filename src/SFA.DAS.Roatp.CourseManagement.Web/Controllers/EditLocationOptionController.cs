using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
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
        public async Task<IActionResult> Index([FromRoute] int larsCode)
        {
            var model = new EditLocationOptionViewModel();
            var locationOption = _sessionService.Get(SessionKeys.SelectedLocationOption);
            if (string.IsNullOrEmpty(locationOption))
            {
                var result = await _mediator.Send(new GetStandardDetailsQuery(Ukprn, larsCode));
                model.LocationOption = result.LocationOption;
            }
            else
            {
                Enum.TryParse<LocationOption>(locationOption, out var result);
                model.LocationOption = result;
            }
            _logger.LogInformation("For Ukprn:{Ukprn} LarsCode:{LarsCode} the location option is set to {locationOption}", Ukprn, larsCode, model.LocationOption);

            model.BackLink = model.CancelLink = GetStandardDetailsUrl(larsCode);

            _sessionService.Delete(SessionKeys.SelectedLocationOption);

            return View(model);
        }

        [HttpPost]
        [Route("{ukprn}/standards/{larsCode}/edit-location-option", Name = RouteNames.PostLocationOption)]
        public async Task<IActionResult> Index([FromRoute] int larsCode, [FromRoute] int ukprn, LocationOptionSubmitModel submitModel)
        {
            if (!ModelState.IsValid)
            {
                var model = new EditLocationOptionViewModel();
                model.BackLink = model.CancelLink = GetStandardDetailsUrl(larsCode);
                return View(model);
            }
            _logger.LogInformation("For Ukprn:{Ukprn} LarsCode:{LarsCode} the location option is being updated to {locationOption}", Ukprn, larsCode, submitModel.LocationOption);

            _sessionService.Set(submitModel.LocationOption.ToString(), SessionKeys.SelectedLocationOption);

            switch (submitModel.LocationOption)
            {
                case LocationOption.ProviderLocation:
                case LocationOption.EmployerLocation:
                    var deleteOption = 
                        submitModel.LocationOption == LocationOption.ProviderLocation ? 
                        DeleteProviderCourseLocationOption.DeleteEmployerLocations :
                        DeleteProviderCourseLocationOption.DeleteProviderLocations;
                    var command = new DeleteCourseLocationsCommand(Ukprn, larsCode, UserId, UserDisplayName ,deleteOption);
                    await _mediator.Send(command);
                    break;
                case LocationOption.Both:
                default:
                    break;
            }
            if(submitModel.LocationOption == LocationOption.ProviderLocation || submitModel.LocationOption == LocationOption.Both)
            {
                return RedirectToRoute(RouteNames.GetProviderCourseLocations, new { Ukprn, larsCode });
            }
            return RedirectToRoute(RouteNames.GetNationalDeliveryOption, new { ukprn, larsCode });
        }
    }
}
