using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.SelectShortCourseTrainingVenuesControllerTests;
public class SelectShortCourseTrainingVenuesControllerPostTests
{
    [Test, MoqAutoData]
    public void SelectShortCourseTrainingVenue_InvalidState_ReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseTrainingVenuesController sut,
        ShortCourseSessionModel sessionModel,
        SelectShortCourseTrainingVenuesSubmitModel submitModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        sut.ModelState.AddModelError("key", "message");

        // Act
        var response = sut.SelectShortCourseTrainingVenue(submitModel, courseType);

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as SelectShortCourseTrainingVenuesViewModel;
        model.Should().NotBeNull();
        model!.TrainingVenues.Should().BeEquivalentTo(sessionModel.TrainingVenues);
        model!.CourseType.Should().Be(courseType);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<ShortCourseSessionModel>()), Times.Never);
    }

    [Test, MoqAutoData]
    public void SelectShortCourseTrainingVenue_SetsSessionCorrectlyAndRedirectsToSelectShortCourseTrainingVenue(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseTrainingVenuesController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;
        var submitModel = new SelectShortCourseTrainingVenuesSubmitModel()
        {
            SelectedProviderLocationIds = sessionModel.TrainingVenues.Select(l => l.ProviderLocationId).ToList(),
        };
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = sut.SelectShortCourseTrainingVenue(submitModel, courseType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.SelectShortCourseTrainingVenue);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.TrainingVenues.FirstOrDefault().ProviderLocationId == submitModel.SelectedProviderLocationIds.FirstOrDefault())), Times.Once());
    }

    [Test, MoqAutoData]
    public void SelectShortCourseTrainingVenue_SessionIsNull_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseTrainingVenuesController sut,
        SelectShortCourseTrainingVenuesSubmitModel submitModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.SelectShortCourseTrainingVenue(submitModel, courseType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
