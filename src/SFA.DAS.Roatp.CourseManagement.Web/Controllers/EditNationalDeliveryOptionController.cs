using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddNationalLocation;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class EditNationalDeliveryOptionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISessionService _sessionService;
        private readonly ILogger<EditNationalDeliveryOptionController> _logger;
        public EditNationalDeliveryOptionController(IMediator mediator, ISessionService sessionService, ILogger<EditNationalDeliveryOptionController> logger)
        {
            _mediator = mediator;
            _sessionService = sessionService;
            _logger = logger;
        }

        [HttpGet]
        [Route("{ukprn}/standards/{larsCode}/edit-national-delivery-option", Name = RouteNames.GetNationalDeliveryOption)]
        public IActionResult Index([FromRoute] string larsCode)
        {
            if (!IsCorrectLocationOptionSetInSession())
            {
                _logger.LogWarning("Location option is not set in session, navigating back to the question ukprn:{ukprn} larscode: {larscode}", Ukprn, larsCode);
                return RedirectToRoute(RouteNames.GetLocationOption, new { Ukprn, larsCode });
            }

            return View(new EditNationalDeliveryOptionViewModel());
        }

        [HttpPost]
        [Route("{ukprn}/standards/{LarsCode}/edit-national-delivery-option", Name = RouteNames.PostNationalDeliveryOption)]
        public async Task<IActionResult> Index([FromRoute] string larsCode, ConfirmNationalProviderSubmitModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("National delivery option was not selected ukprn:{ukprn} larscode:{larscode}", Ukprn, larsCode);
                return View(new EditNationalDeliveryOptionViewModel());
            }

            if (model.HasNationalDeliveryOption.GetValueOrDefault())
            {
                _logger.LogInformation("National delivery option selected, adding national location to ukprn:{ukprn} larscode:{larscode}", Ukprn, larsCode);
                await _mediator.Send(new DeleteCourseLocationsCommand(Ukprn, larsCode, UserId, UserDisplayName, DeleteProviderCourseLocationOption.DeleteEmployerLocations));
                await _mediator.Send(new AddNationalLocationToStandardCommand(Ukprn, larsCode, UserId, UserDisplayName));
                return RedirectToRoute(RouteNames.GetStandardDetails, new { Ukprn, larsCode });
            }

            _logger.LogInformation("National delivery option not selected, navigating to region page ukprn:{ukprn} larscode:{larscode}", Ukprn, larsCode);

            return RedirectToRoute(RouteNames.GetStandardSubRegions, new { Ukprn, larsCode });
        }

        private bool IsCorrectLocationOptionSetInSession()
        {
            var sessionValue = _sessionService.Get(SessionKeys.SelectedLocationOption);
            return
                (!string.IsNullOrEmpty(sessionValue) &&
                (Enum.TryParse<LocationOption>(sessionValue, out var locationOption)
                    && (locationOption == LocationOption.EmployerLocation || locationOption == LocationOption.Both)));
        }
    }
}
