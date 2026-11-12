using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Courses.Queries.GetProviderAvailableCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.ConfirmRegulatedShortCourseControllerTests;

public class ConfirmRegulatedShortCourseControllerGetTests
{
    [Test, MoqAutoData]
    public async Task WhenSessionIsValid_ThenReturnsView(
       [Frozen] Mock<IMediator> mediatorMock,
       [Frozen] Mock<ISessionService> sessionServiceMock,
       [Greedy] ConfirmRegulatedShortCourseController sut,
       GetProviderAvailableCourseDetailsQueryResult queryResult,
       ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var expectedLearningType = LearningType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderAvailableCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == sessionModel.LarsCode && q.CourseType == CourseType.ShortCourse), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = await sut.ConfirmRegulatedShortCourse(expectedLearningType);

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as ConfirmRegulatedShortCourseViewModel;
        model.Should().NotBeNull();
        model!.ShortCourseInformation.Should().BeEquivalentTo(queryResult, o => o.ExcludingMissingMembers());
        model!.LearningType.Should().Be(expectedLearningType);
        mediatorMock.Verify(m => m.Send(It.Is<GetProviderAvailableCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == sessionModel.LarsCode && q.CourseType == CourseType.ShortCourse), It.IsAny<CancellationToken>()), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.ShortCourseInformation == model.ShortCourseInformation)), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenSessionIsNull_ThenRedirectsToReviewYourDetails(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] ConfirmRegulatedShortCourseController sut,
    ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var expectedApprenticeshipType = LearningType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(() => null);

        // Act
        var response = await sut.ConfirmRegulatedShortCourse(expectedApprenticeshipType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        mediatorMock.Verify(m => m.Send(It.Is<GetProviderAvailableCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == sessionModel.LarsCode && q.CourseType == CourseType.ShortCourse), It.IsAny<CancellationToken>()), Times.Never);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<ShortCourseSessionModel>()), Times.Never);
    }
}
