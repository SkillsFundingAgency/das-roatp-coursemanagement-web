using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddNationalLocation;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System;
using System.Threading.Tasks;

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
        public IActionResult Index([FromRoute] int larsCode)
        {
            if(!IsCorrectLocationOptionSetInSession(larsCode))
            {
                return RedirectToRoute(RouteNames.GetLocationOption, new { Ukprn, larsCode });
            }

            var model = new EditNationalDeliveryOptionViewModel();
            model.BackLink = Url.RouteUrl(RouteNames.GetLocationOption, new { Ukprn, larsCode });
            model.CancelLink = GetStandardDetailsUrl(larsCode);
            return View(model);
        }

        [HttpPost]
        [Route("{ukprn}/standards/{LarsCode}/edit-national-delivery-option", Name = RouteNames.PostNationalDeliveryOption)]
        public async Task<IActionResult> Index(EditNationalDeliveryOptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.HasNationalDeliveryOption.GetValueOrDefault())
            {
                await _mediator.Send(new AddNationalLocationToStandardCommand(Ukprn, model.LarsCode, UserId));
                return RedirectToRoute(RouteNames.GetStandardDetails, new { Ukprn, model.LarsCode });
            }

            return RedirectToRoute(RouteNames.GetStandardDetails, new { Ukprn, model.LarsCode }); //TODO redirect to regions after CSP-114
        }

        private bool IsCorrectLocationOptionSetInSession(int larsCode)
        {
            var sessionValue = _sessionService.Get(SessionKeys.SelectedLocationOption, larsCode.ToString());
            return
                (!string.IsNullOrEmpty(sessionValue) &&
                (Enum.TryParse<LocationOption>(sessionValue, out var locationOption)
                    && (locationOption == LocationOption.EmployerLocation || locationOption == LocationOption.Both)));
        }
    }
}
