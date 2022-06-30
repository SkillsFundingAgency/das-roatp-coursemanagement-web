using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateApprovedByRegulator;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Net;
using System.Threading.Tasks;


namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ConfirmRemoveProviderLocationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ConfirmRemoveProviderLocationController> _logger;

        public ConfirmRemoveProviderLocationController(IMediator mediator, ILogger<ConfirmRemoveProviderLocationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Route("{ukprn}/traininglocations/{providerLocationId}/confirm-remove-providerlocation", Name = RouteNames.GetRemoveTrainingLocation)]
        [HttpGet]
        public async Task<IActionResult> ConfirmRemoveTrainingLocation()
        {
            //_logger.LogInformation("Getting Course details for ukprn {ukprn} ", Ukprn);

            //var result = await _mediator.Send(new GetStandardDetailsQuery(ukprn, larsCode));

            //if (result == null)
            //{
            //    var message = $"Standard details not found for ukprn {ukprn} and larscode {larsCode}";
            //    _logger.LogError(message);
            //    throw new InvalidOperationException(message);
            //}

            //var model = (ConfirmRegulatedStandardViewModel)result;
            //if (Request.GetTypedHeaders().Referer == null)
            //{
            //    model.BackLink = model.CancelLink = "#";
            //}
            //else
            //{
            //    model.BackLink = model.CancelLink = model.RefererLink = Request.GetTypedHeaders().Referer.ToString();
            //}

            return View("~/Views/ProviderLocations/ConfirmRemoveProviderLocation.cshtml");
        }

        [Route("{ukprn}/traininglocations/{providerLocationId}/confirm-remove-providerlocation", Name = RouteNames.PostRemoveTrainingLocation)]
        [HttpPost]
        public async Task<IActionResult> RemoveTrainingLocation(ConfirmRegulatedStandardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/ProviderLocations/ConfirmRemoveProviderLocation.cshtml", model);
            }

            if(!model.IsRegulatedStandard)
            {
                return Redirect($"Error/{HttpStatusCode.NotFound}");
            }

            var command = (UpdateApprovedByRegulatorCommand)model;
            command.Ukprn = Ukprn;
            command.LarsCode = model.LarsCode;
            command.UserId = UserId;

            await _mediator.Send(command);

            return Redirect(model.RefererLink);
        }
    }
}
