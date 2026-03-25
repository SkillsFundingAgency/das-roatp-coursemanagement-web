using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateContactDetails;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.EditShortCourseContactDetailsControllerTests;
public class EditShortCourseContactDetailsControllerPostTests
{
    [Test, MoqAutoData]
    public async Task EditShortCourseContactDetails_InvalidModelState_ReturnsView(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseContactDetailsController sut,
        CourseContactDetailsSubmitModel model,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "error");

        // Act
        var result = await sut.EditShortCourseContactDetails(apprenticeshipType, larsCode, model);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        var viewModel = viewResult.Model as ShortCourseContactDetailsViewModel;
        viewModel.Should().NotBeNull();
        viewModel.SubmitButtonText.Should().Be(ButtonText.Confirm);
        viewModel.Route.Should().Be(RouteNames.EditShortCourseContactDetails);
        viewModel.IsAddJourney.Should().BeFalse();
        viewModel.ShowSavedContactDetailsText.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseContactDetails_InvalidModelState_VerifyMediatorIsNotInvoked(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] EditShortCourseContactDetailsController sut,
    CourseContactDetailsSubmitModel model,
    string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "error");

        // Act
        await sut.EditShortCourseContactDetails(apprenticeshipType, larsCode, model);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn == int.Parse(TestConstants.DefaultUkprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Never());
        mediatorMock.Verify(m => m.Send(It.IsAny<UpdateProviderCourseContactDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseContactDetails_ChangeToContactDetails_SendsUpdateCommandAndVerifyMediatorIsInvoked(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseContactDetailsController sut,
        CourseContactDetailsSubmitModel model,
        GetStandardDetailsQueryResult queryResult,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn == int.Parse(TestConstants.DefaultUkprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.EditShortCourseContactDetails(apprenticeshipType, larsCode, model);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.ManageShortCourseDetails);
        routeResult.RouteValues.Should().NotBeEmpty().And.HaveCount(3);
        routeResult.RouteValues.Should().ContainKey("ukprn").WhoseValue.Should().Be(int.Parse(TestConstants.DefaultUkprn));
        routeResult.RouteValues.Should().ContainKey("larsCode").WhoseValue.Should().Be(larsCode);
        routeResult.RouteValues.Should().ContainKey("apprenticeshipType").WhoseValue.Should().Be(apprenticeshipType);
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn == int.Parse(TestConstants.DefaultUkprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once());
        mediatorMock.Verify(m => m.Send(It.Is<UpdateProviderCourseContactDetailsCommand>(c => c.Ukprn == int.Parse(TestConstants.DefaultUkprn) && c.UserId == TestConstants.DefaultUserId && c.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseContactDetails_NoChangeToContactDetails_RedirectsToManageShortCourseDetailsAndVerifyMediatorIsNotInvoked(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseContactDetailsController sut,
        CourseContactDetailsSubmitModel model,
        GetStandardDetailsQueryResult queryResult,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        queryResult.ContactUsPhoneNumber = model.ContactUsPhoneNumber;
        queryResult.ContactUsEmail = model.ContactUsEmail;
        queryResult.StandardInfoUrl = model.StandardInfoUrl;

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn == int.Parse(TestConstants.DefaultUkprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.EditShortCourseContactDetails(apprenticeshipType, larsCode, model);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.ManageShortCourseDetails);
        mediatorMock.Verify(m => m.Send(It.Is<UpdateProviderCourseContactDetailsCommand>(c => c.Ukprn == int.Parse(TestConstants.DefaultUkprn) && c.UserId == TestConstants.DefaultUserId && c.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseContactDetails_ChangeToContactDetails_VerifyMediatorIsInvokedWithTrimmedFields(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] EditShortCourseContactDetailsController sut,
    CourseContactDetailsSubmitModel model,
    GetStandardDetailsQueryResult queryResult,
    string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        model.ContactUsEmail = " test@test.com ";
        model.ContactUsPhoneNumber = " 012345 ";
        model.StandardInfoUrl = " test@gmail.com ";

        var contactUsEmailTrimmed = "test@test.com";
        var contactUsPhoneNumberTrimmed = "012345";
        var standardInfoUrlTrimmed = "test@gmail.com";

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn == int.Parse(TestConstants.DefaultUkprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sut.AddDefaultContextWithUser();

        // Act
        await sut.EditShortCourseContactDetails(apprenticeshipType, larsCode, model);

        // Assert
        mediatorMock.Verify(m => m.Send(
            It.Is<UpdateProviderCourseContactDetailsCommand>(c =>
            c.ContactUsPhoneNumber == contactUsPhoneNumberTrimmed &&
            c.ContactUsEmail == contactUsEmailTrimmed &&
            c.StandardInfoUrl == standardInfoUrlTrimmed &&
            c.Ukprn == int.Parse(TestConstants.DefaultUkprn) &&
            c.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseContactDetails_GetStardardsReturnsNull_RedirectsToEditShortCourseContactDetails(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseContactDetailsController sut,
        CourseContactDetailsSubmitModel model,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.Ukprn == int.Parse(TestConstants.DefaultUkprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.EditShortCourseContactDetails(apprenticeshipType, larsCode, model);

        // Assert
        var viewResult = result as RedirectToRouteResult;
        viewResult.RouteName.Should().Be(RouteNames.EditShortCourseContactDetails);
    }
}
