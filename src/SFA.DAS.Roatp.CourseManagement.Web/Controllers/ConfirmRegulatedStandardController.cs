using System;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateApprovedByRegulator;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;


namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[Route("{ukprn}/standards/{larsCode}/confirm-regulated-standard")]
public class ConfirmRegulatedStandardController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ConfirmRegulatedStandardController> _logger;
    private readonly IValidator<ConfirmRegulatedStandardViewModel> _validator;

    public ConfirmRegulatedStandardController(IMediator mediator, ILogger<ConfirmRegulatedStandardController> logger, IValidator<ConfirmRegulatedStandardViewModel> validator)
    {
        _mediator = mediator;
        _logger = logger;
        _validator = validator;
    }

    [HttpGet(Name = RouteNames.GetConfirmRegulatedStandard)]
    public async Task<IActionResult> ConfirmRegulatedStandard([FromRoute] string larsCode)
    {
        var ukprn = Ukprn;
        _logger.LogInformation("Getting Course details for ukprn {Ukprn} LarsCode {LarsCode}", ukprn, larsCode);

        var result = await _mediator.Send(new GetProviderCourseDetailsQuery(ukprn, larsCode));

        if (result == null)
        {
            var message = $"Standard details not found for ukprn {ukprn} and larscode {larsCode}";
            _logger.LogError("Standard details not found for ukprn {Ukprn} and larscode {LarsCode}", ukprn, larsCode);
            throw new InvalidOperationException(message);
        }

        var model = (ConfirmRegulatedStandardViewModel)result;
        model.RefererLink = Request.GetTypedHeaders().Referer == null ? "#" : Request.GetTypedHeaders().Referer!.ToString();

        return View("~/Views/Standards/ConfirmRegulatedStandard.cshtml", model);
    }

    [HttpPost(Name = RouteNames.PostConfirmRegulatedStandard)]
    public async Task<IActionResult> UpdateApprovedByRegulator(ConfirmRegulatedStandardViewModel model)
    {
        var validatedResult = _validator.Validate(model);

        if (!validatedResult.IsValid)
        {
            ModelState.AddValidationErrors(validatedResult.Errors);

            return View("~/Views/Standards/ConfirmRegulatedStandard.cshtml", model);
        }

        if (!model.IsRegulatedStandard)
        {
            return Redirect($"Error/{HttpStatusCode.NotFound}");
        }

        var command = (UpdateApprovedByRegulatorCommand)model;
        command.Ukprn = Ukprn;
        command.LarsCode = model.LarsCode;
        command.UserId = UserId;
        command.UserDisplayName = UserDisplayName;

        await _mediator.Send(command);

        if (!model.IsApprovedByRegulator.Value)
        {
            return View("~/Views/ShutterPages/RegulatedStandardSeekApproval.cshtml", model);
        }
        return Redirect(model.RefererLink);
    }
}
