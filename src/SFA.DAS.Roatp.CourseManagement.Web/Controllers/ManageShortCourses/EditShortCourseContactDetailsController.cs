using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateContactDetails;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;

[AuthorizeCourseType(CourseType.ShortCourse)]
[Route("{ukprn}/courses/{apprenticeshipType}/{larsCode}/edit-contact-details", Name = RouteNames.EditShortCourseContactDetails)]
public class EditShortCourseContactDetailsController(IMediator _mediator, ILogger<EditShortCourseContactDetailsController> _logger) : ControllerBase
{
    public const string ViewPath = "~/Views/ManageShortCourses/EditShortCourseContactDetailsView.cshtml";

    [HttpGet]
    public async Task<IActionResult> EditShortCourseContactDetails(ApprenticeshipType apprenticeshipType, string larsCode)
    {
        var apiResponse = await GetStandardDetails(larsCode);

        if (apiResponse == null)
        {
            _logger.LogWarning("No data returned for ukprn {Ukprn} and LarsCode {LarsCode} for User: {UserId}. Redirecting to PageNotFound.", Ukprn, larsCode, UserId);

            return View(ViewsPath.PageNotFoundPath);
        }

        var model = (EditShortCourseContactDetailsViewModel)apiResponse;
        model.ApprenticeshipType = apprenticeshipType;
        model.SubmitButtonText = ButtonText.Confirm;
        model.Route = RouteNames.EditShortCourseContactDetails;

        return View(ViewPath, model);
    }

    [HttpPost]
    public async Task<IActionResult> EditShortCourseContactDetails(ApprenticeshipType apprenticeshipType, string larsCode, CourseContactDetailsSubmitModel submitModel)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = new EditShortCourseContactDetailsViewModel();
            viewModel.ApprenticeshipType = apprenticeshipType;
            viewModel.SubmitButtonText = ButtonText.Confirm;
            viewModel.Route = RouteNames.EditShortCourseContactDetails;

            return View(ViewPath, viewModel);
        }

        submitModel.ContactUsPhoneNumber = submitModel.ContactUsPhoneNumber.Trim();
        submitModel.ContactUsEmail = submitModel.ContactUsEmail.Trim();
        submitModel.StandardInfoUrl = submitModel.StandardInfoUrl.Trim();

        var apiResponse = await GetStandardDetails(larsCode);

        if (apiResponse == null)
        {
            return RedirectToRoute(RouteNames.EditShortCourseContactDetails, new { Ukprn, apprenticeshipType, larsCode });
        }

        if (!(apiResponse.ContactUsPhoneNumber == submitModel.ContactUsPhoneNumber && apiResponse.ContactUsEmail == submitModel.ContactUsEmail && apiResponse.StandardInfoUrl == submitModel.StandardInfoUrl))
        {
            var command = (UpdateProviderCourseContactDetailsCommand)submitModel;
            command.LarsCode = larsCode;
            command.Ukprn = Ukprn;
            command.UserId = UserId;
            command.UserDisplayName = UserDisplayName;

            await _mediator.Send(command);
        }

        return RedirectToRoute(RouteNames.ManageShortCourseDetails, new { Ukprn, apprenticeshipType, larsCode });
    }

    private async Task<GetStandardDetailsQueryResult> GetStandardDetails(string larsCode)
    {
        var result = await _mediator.Send(new GetStandardDetailsQuery(Ukprn, larsCode));

        return result;
    }
}