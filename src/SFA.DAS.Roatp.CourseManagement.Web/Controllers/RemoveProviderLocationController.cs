using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Threading.Tasks;


namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class RemoveProviderLocationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RemoveProviderLocationController> _logger;

        public RemoveProviderLocationController(IMediator mediator, ILogger<RemoveProviderLocationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Route("{ukprn}/standards/{larsCode}/providercourselocations/{providerCourseLocationId}/remove-providercourselocation", Name = RouteNames.GetRemoveProviderCourseLocation)]
        [HttpGet]
        public async Task<IActionResult> RemoveProviderCourseLocation(int larsCode, int providerCourseLocationId)
        {
            _logger.LogInformation("Getting Provider Course Location for ukprn {ukprn} ", Ukprn);

            var result = await _mediator.Send(new GetProviderCourseLocationsQuery(Ukprn, larsCode));

            if (result == null)
            {
                var message = $"Provider Course Location not found for ukprn {Ukprn} and larscode {larsCode}";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            var model = (RemoveProviderLocationViewModel)result.ProviderCourseLocations.Find(l=>l.ProviderCourseLocationId == providerCourseLocationId);
            model.BackLink = model.CancelLink = GetStandardDetailsUrl(model.LarsCode);

            return View("~/Views/Standards/RemoveProviderCourseLocation.cshtml", model);
        }

        [Route("{ukprn}/standards/{larsCode}/providercourselocations/{providerCourseLocationId}/remove-providercourselocation", Name = RouteNames.PostRemoveProviderCourseLocation)]
        [HttpPost]
        public async Task<IActionResult> RemoveProviderCourseLocation(RemoveProviderLocationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.BackLink = model.CancelLink = GetStandardDetailsUrl(model.LarsCode);
                return View("~/Views/Standards/RemoveProviderCourseLocation.cshtml", model);
            }
            var command = new DeleteProviderCourseLocationCommand(Ukprn, model.LarsCode,  model.ProviderCourseLocationId, UserId);
            await _mediator.Send(command);

            return Redirect(model.BackLink);
        }
    }
}
