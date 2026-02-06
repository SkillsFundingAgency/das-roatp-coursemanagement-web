using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.SelectShortCourseLocationOptionsControllerTests;
public class SelectShortCourseLocationOptionsControllerGetTests
{
    [Test, MoqAutoData]
    public void SelectShortCourseLocation_SessionIsValid_ReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        List<ShortCourseLocationOptionModel> locationOptions = new()
        {
            new ShortCourseLocationOptionModel { LocationOption = ShortCourseLocationOption.ProviderLocation, IsSelected = false },
            new ShortCourseLocationOptionModel { LocationOption = ShortCourseLocationOption.EmployerLocation, IsSelected = false },
            new ShortCourseLocationOptionModel { LocationOption = ShortCourseLocationOption.Online, IsSelected = false },
        };

        sessionModel.LocationOptions = new List<ShortCourseLocationOption>();

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.SelectShortCourseLocation(apprenticeshipType);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as SelectShortCourseLocationOptionsViewModel;
        model!.LocationOptions.Should().BeEquivalentTo(locationOptions);
        model.ApprenticeshipType.Should().Be(apprenticeshipType);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void SelectShortCourseLocation_SessionIsNull_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.SelectShortCourseLocation(apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
