using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Commands.UpdateApprovedByRegulator;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System;
using System.Net;
using System.Threading.Tasks;


namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
    public class ConfirmRegulatedStandardController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ConfirmRegulatedStandardController> _logger;

        public ConfirmRegulatedStandardController(IMediator mediator, ILogger<ConfirmRegulatedStandardController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Route("{ukprn}/standards/{larsCode}/confirm-regulated-standard", Name = RouteNames.ConfirmRegulatedStandard)]
        [HttpGet]
        public async Task<IActionResult> ConfirmRegulatedStandard([FromRoute] int larsCode)
        {
            var ukprn = Ukprn;
            _logger.LogInformation("Getting Course details for ukprn {ukprn} LarsCode {larsCode}", ukprn, larsCode);

            var result = await _mediator.Send(new GetStandardDetailsQuery(ukprn, larsCode));

            if (result?.StandardDetails == null)
            {
                var message = $"Standard details not found for ukprn {ukprn} and larscode {larsCode}";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            var model = (ConfirmRegulatedStandardViewModel)result.StandardDetails;
            if (Request.GetTypedHeaders().Referer == null)
            {
                model.BackLink = model.CancelLink = "#";
            }
            else
            {
                model.BackLink = model.CancelLink = model.RefererLink = Request.GetTypedHeaders().Referer.ToString();
            }

            return View("~/Views/Standards/ConfirmRegulatedStandard.cshtml", model);
        }

        [Route("{ukprn}/standards/{larsCode}/confirm-regulated-standard", Name = RouteNames.PostConfirmRegulatedStandard)]
        [HttpPost]
        public async Task<IActionResult> UpdateApprovedByRegulator(ConfirmRegulatedStandardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Standards/ConfirmRegulatedStandard.cshtml", model);
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

            if (!model.IsApprovedByRegulator.Value)
            {
                return View("~/Views/ShutterPages/RegulatedStandardSeekApproval.cshtml", model);
            }
            return Redirect(model.RefererLink);
        }
    }
}
