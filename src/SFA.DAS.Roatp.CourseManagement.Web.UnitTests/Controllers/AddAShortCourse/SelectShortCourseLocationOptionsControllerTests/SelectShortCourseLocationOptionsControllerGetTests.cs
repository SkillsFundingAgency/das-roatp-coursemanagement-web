using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

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

        sessionModel.LocationOptions =
        [
            ShortCourseLocationOption.ProviderLocation,
            ShortCourseLocationOption.EmployerLocation,
            ShortCourseLocationOption.Online
        ];

        List<ShortCourseLocationOptionModel> locationOptions = new()
        {
            new ShortCourseLocationOptionModel { LocationOption = ShortCourseLocationOption.ProviderLocation, IsSelected = true },
            new ShortCourseLocationOptionModel { LocationOption = ShortCourseLocationOption.EmployerLocation, IsSelected = true },
            new ShortCourseLocationOptionModel { LocationOption = ShortCourseLocationOption.Online, IsSelected = true },
        };

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.SelectShortCourseLocation(apprenticeshipType);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as SelectShortCourseLocationOptionsViewModel;
        model!.LocationOptions.Should().BeEquivalentTo(locationOptions);
        model.ApprenticeshipType.Should().Be(apprenticeshipType);
        model.Route.Should().Be(RouteNames.SelectShortCourseLocationOption);
        model.IsAddJourney.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test]
    [MoqInlineAutoData(false, ButtonText.Continue)]
    [MoqInlineAutoData(true, ButtonText.Confirm)]
    public void SelectShortCourseLocation_HasSeenSummaryPageIsTrueOrFalse_ReturnsExpectedButtonText(
        bool seenSummaryPage,
        string expectedSubmitButtonText,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sessionModel.HasSeenSummaryPage = seenSummaryPage;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.SelectShortCourseLocation(apprenticeshipType);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as SelectShortCourseLocationOptionsViewModel;
        model.SubmitButtonText.Should().Be(expectedSubmitButtonText);
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
