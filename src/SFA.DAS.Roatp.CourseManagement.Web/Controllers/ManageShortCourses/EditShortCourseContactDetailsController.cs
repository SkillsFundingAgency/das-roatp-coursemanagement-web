using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateContactDetails;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Extensions;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{learningType}/{larsCode}/edit-contact-details", Name = RouteNames.EditShortCourseContactDetails)]
public class EditShortCourseContactDetailsController(IMediator _mediator, ILogger<EditShortCourseContactDetailsController> _logger, IValidator<CourseContactDetailsSubmitModel> _validator) : ControllerBase
{
    public const string ViewPath = "~/Views/ShortCourses/ShortCourseContactDetails.cshtml";

    [HttpGet]
    public async Task<IActionResult> EditShortCourseContactDetails(LearningType learningType, string larsCode)
    {
        var providerCourseDetailsResponse = await GetProviderCourseDetails(larsCode);

        if (providerCourseDetailsResponse == null)
        {
            _logger.LogWarning("No data returned for ukprn {Ukprn} and LarsCode {LarsCode} for User: {UserId}. Redirecting to PageNotFound.", Ukprn, larsCode, UserId);

            return View(ViewsPath.PageNotFoundPath);
        }

        var viewModel = GetViewModel(providerCourseDetailsResponse, learningType);

        return View(ViewPath, viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditShortCourseContactDetails(LearningType learningType, string larsCode, CourseContactDetailsSubmitModel submitModel)
    {
        submitModel.ContactUsPhoneNumber = submitModel.ContactUsPhoneNumber?.Trim();
        submitModel.ContactUsEmail = submitModel.ContactUsEmail?.Trim();
        submitModel.StandardInfoUrl = submitModel.StandardInfoUrl?.Trim();

        var validatedResult = await _validator.ValidateAsync(submitModel);

        if (!validatedResult.IsValid) ModelState.AddValidationErrors(validatedResult.Errors);

        if (!ModelState.IsValid)
        {
            var viewModel = GetViewModel(new GetProviderCourseDetailsQueryResult(), learningType);

            return View(ViewPath, viewModel);
        }

        var providerCourseDetailsResponse = await GetProviderCourseDetails(larsCode);

        if (providerCourseDetailsResponse == null)
        {
            return RedirectToRoute(RouteNames.EditShortCourseContactDetails, new { Ukprn, learningType, larsCode });
        }

        if (!(providerCourseDetailsResponse.ContactUsPhoneNumber == submitModel.ContactUsPhoneNumber && providerCourseDetailsResponse.ContactUsEmail == submitModel.ContactUsEmail && providerCourseDetailsResponse.StandardInfoUrl == submitModel.StandardInfoUrl))
        {
            var command = (UpdateProviderCourseContactDetailsCommand)submitModel;
            command.LarsCode = larsCode;
            command.Ukprn = Ukprn;
            command.UserId = UserId;
            command.UserDisplayName = UserDisplayName;

            await _mediator.Send(command);
        }

        return RedirectToRoute(RouteNames.ManageShortCourseDetails, new { Ukprn, learningType, larsCode });
    }

    private async Task<GetProviderCourseDetailsQueryResult> GetProviderCourseDetails(string larsCode)
    {
        _logger.LogInformation("Getting provider course details for ukprn {Ukprn} and lasrcode {LarsCode}", Ukprn, larsCode);

        var result = await _mediator.Send(new GetProviderCourseDetailsQuery(Ukprn, larsCode));

        return result;
    }

    private static ShortCourseContactDetailsViewModel GetViewModel(GetProviderCourseDetailsQueryResult providerCourseDetails, LearningType learningType)
    {
        var model = (ShortCourseContactDetailsViewModel)providerCourseDetails;

        model.LearningType = learningType;
        model.SubmitButtonText = ButtonText.Confirm;
        model.Route = RouteNames.EditShortCourseContactDetails;
        model.IsAddJourney = false;
        model.ShowSavedContactDetailsText = false;

        return model;
    }
}