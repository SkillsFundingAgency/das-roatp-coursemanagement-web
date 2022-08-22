﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using System;
using System.Threading.Tasks;


namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [DasAuthorize(new[] { "ProviderFeature.CourseManagement" }, Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ProviderCourseLocationRemoveController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProviderCourseLocationRemoveController> _logger;

        public ProviderCourseLocationRemoveController(IMediator mediator, ILogger<ProviderCourseLocationRemoveController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Route("{ukprn}/standards/{larsCode}/providerlocations/{id}/remove-providerlocation", Name = RouteNames.GetRemoveProviderCourseLocation)]
        [HttpGet]
        public async Task<IActionResult> GetProviderCourseLocation(int larsCode, Guid id)
        {
            _logger.LogInformation("Getting Provider Course Location for ukprn {ukprn} ", Ukprn);

            var result = await _mediator.Send(new GetProviderCourseLocationsQuery(Ukprn, larsCode));

            if (result == null)
            {
                var message = $"Provider Course Location not found for ukprn {Ukprn} and larscode {larsCode}";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            var model = new ProviderCourseLocationViewModel
            {
                LarsCode = larsCode
            };

            if (result.ProviderCourseLocations.Find(l => l.Id == id) != null)
            {
                model = (ProviderCourseLocationViewModel)result.ProviderCourseLocations.Find(l => l.Id == id);
            }
            model.BackLink = model.CancelLink = Url.RouteUrl(RouteNames.GetProviderCourseLocations, new { Ukprn, larsCode });
            return View("~/Views/ProviderCourseLocations/RemoveProviderCourseLocation.cshtml", model);
        }

        [Route("{ukprn}/standards/{larsCode}/providerlocations/{id}/remove-providerlocation", Name = RouteNames.PostRemoveProviderCourseLocation)]
        [HttpPost]
        public async Task<IActionResult> RemoveProviderCourseLocation(ProviderCourseLocationViewModel model)
        {
            var command = new DeleteProviderCourseLocationCommand(Ukprn, model.LarsCode,  model.Id, UserId);
            await _mediator.Send(command);

            return RedirectToRoute(RouteNames.GetProviderCourseLocations, new { Ukprn, larsCode = model.LarsCode });
        }
    }
}
