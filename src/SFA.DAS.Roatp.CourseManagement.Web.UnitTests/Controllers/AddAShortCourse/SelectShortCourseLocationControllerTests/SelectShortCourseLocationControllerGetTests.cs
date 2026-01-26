using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.SelectShortCourseLocationControllerTests;
public class SelectShortCourseLocationControllerGetTests
{
    [Test, MoqAutoData]
    public void SelectShortCourseLocation_SessionIsValid_ReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationController sut,
        ShortCourseSessionModel sessionModel
    )
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.SelectShortCourseLocation(courseType);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as SelectShortCourseLocationViewModel;
        model!.ShortCourseLocations.Should().BeEquivalentTo(sessionModel.ShortCourseLocations);
        model.CourseType.Should().Be(courseType);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void SelectShortCourseLocation_SessionIsNull_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationController sut)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.SelectShortCourseLocation(courseType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
