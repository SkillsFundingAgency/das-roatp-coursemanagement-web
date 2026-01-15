using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ApprenticeshipUnits;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAnApprenticeshipUnit;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ApprenticeshipUnits.ConfirmApprenticeshipUnitControllerTests;
public class ConfirmApprenticeshipUnitControllerPostTests
{
    [Test, MoqAutoData]
    public async Task Index_InvalidState_ReturnsView(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmApprenticeshipUnitController sut,
        GetStandardInformationQueryResult queryResult,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.ModelState.AddModelError("key", "message");

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == sessionModel.LarsCode), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = await sut.Index(new ConfirmApprenticeshipUnitSubmitModel());

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as ConfirmApprenticeshipUnitViewModel;
        model.Should().NotBeNull();
        model!.ShortCourseInformation.Should().BeEquivalentTo(queryResult, o => o.ExcludingMissingMembers());
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == sessionModel.LarsCode), It.IsAny<CancellationToken>()), Times.Once);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<ShortCourseSessionModel>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Index_ValidState_IsCorrectShortCourseIsFalse_RedirectsToSelectAnApprenticeshipUnit(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmApprenticeshipUnitController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = await sut.Index(new ConfirmApprenticeshipUnitSubmitModel() { IsCorrectShortCourse = false });

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.SelectAnApprenticeshipUnit);
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == sessionModel.LarsCode), It.IsAny<CancellationToken>()), Times.Never);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<ShortCourseSessionModel>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Index_ValidState_IsCorrectShortCourseIsTrue_SetsSessionAndRedirectsToSelectAnApprenticeshipUnit(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmApprenticeshipUnitController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = await sut.Index(new ConfirmApprenticeshipUnitSubmitModel() { IsCorrectShortCourse = true });

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ConfirmApprenticeshipUnit);
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == sessionModel.LarsCode), It.IsAny<CancellationToken>()), Times.Once);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<ShortCourseSessionModel>()), Times.Once);
    }
}
