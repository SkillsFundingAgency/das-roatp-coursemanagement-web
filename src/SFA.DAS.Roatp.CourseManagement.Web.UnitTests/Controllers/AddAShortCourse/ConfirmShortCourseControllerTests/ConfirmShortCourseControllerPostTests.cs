using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.ConfirmShortCourseControllerTests;
public class ConfirmShortCourseControllerPostTests
{
    [Test, MoqAutoData]
    public void Index_InvalidState_ReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmShortCourseController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.ModelState.AddModelError("key", "message");

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = sut.Index(new ConfirmShortCourseSubmitModel());

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as ConfirmShortCourseViewModel;
        model.Should().NotBeNull();
        model!.ShortCourseInformation.Should().BeEquivalentTo(sessionModel.ShortCourseInformation, o => o.ExcludingMissingMembers());
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ShortCourseSessionModel)), Times.Never);
    }

    [Test, MoqAutoData]
    public void Index_ValidState_IsCorrectShortCourseIsFalse_RedirectsToSelectAnApprenticeshipUnit(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmShortCourseController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = sut.Index(new ConfirmShortCourseSubmitModel() { IsCorrectShortCourse = false });

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.SelectAnApprenticeshipUnit);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ShortCourseSessionModel)), Times.Once);
    }

    [Test, MoqAutoData]
    public void Index_ValidState_IsCorrectShortCourseIsTrue_SetsSessionAndRedirectsToSelectAnApprenticeshipUnit(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmShortCourseController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = sut.Index(new ConfirmShortCourseSubmitModel() { IsCorrectShortCourse = true });

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ConfirmApprenticeshipUnit);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ShortCourseSessionModel)), Times.Never);
    }

    [Test, MoqAutoData]
    public void Index_SessionIsNull_RedirectsToReviewYourDetails(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] ConfirmShortCourseController sut,
    ShortCourseSessionModel sessionModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(() => null);

        // Act
        var response = sut.Index(new ConfirmShortCourseSubmitModel());

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ShortCourseSessionModel)), Times.Never);
    }
}
