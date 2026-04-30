using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateContactDetails;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers;

[AuthorizeCourseType(CourseType.Apprenticeship)]
[Route("{ukprn}/standards/{larsCode}/edit-contact-details")]
public class EditCourseContactDetailsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EditCourseContactDetailsController> _logger;
    private readonly IValidator<CourseContactDetailsSubmitModel> _validator;

    public EditCourseContactDetailsController(IMediator mediator, ILogger<EditCourseContactDetailsController> logger, IValidator<CourseContactDetailsSubmitModel> validator)
    {
        _mediator = mediator;
        _logger = logger;
        _validator = validator;
    }

    [HttpGet(Name = RouteNames.GetCourseContactDetails)]
    public async Task<IActionResult> Index([FromRoute] string larsCode)
    {
        EditCourseContactDetailsViewModel model = await GetViewModel(larsCode);

        return View(model);
    }

    [HttpPost(Name = RouteNames.PostCourseContactDetails)]
    public async Task<IActionResult> Index([FromRoute] string larsCode, CourseContactDetailsSubmitModel submitModel)
    {
        var validatedResult = _validator.Validate(submitModel);

        if (!validatedResult.IsValid)
        {
            var viewModel = await GetViewModel(larsCode);

            ModelState.AddValidationErrors(validatedResult.Errors);

            return View(viewModel);
        }

        var command = (UpdateProviderCourseContactDetailsCommand)submitModel;
        command.LarsCode = larsCode;
        command.Ukprn = Ukprn;
        command.UserId = UserId;
        command.UserDisplayName = UserDisplayName;

        await _mediator.Send(command);

        return RedirectToRoute(RouteNames.GetStandardDetails, new { Ukprn, larsCode });
    }

    private async Task<EditCourseContactDetailsViewModel> GetViewModel(string larsCode)
    {
        var result = await _mediator.Send(new GetProviderCourseDetailsQuery(Ukprn, larsCode));

        if (result == null)
        {
            _logger.LogError("Standard details not found for ukprn {Ukprn} and larscode {LarsCode}", Ukprn, larsCode);
            throw new InvalidOperationException($"Standard details not found for ukprn {Ukprn} and larscode {larsCode}");
        }

        var model = (EditCourseContactDetailsViewModel)result;

        return model;
    }
}
