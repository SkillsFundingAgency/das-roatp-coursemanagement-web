using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.ConfirmShortCourseControllerTests;
public class ConfirmShortCourseControllerGetTests
{
    [Test, MoqAutoData]
    public async Task ConfirmShortCourse_ReturnsView(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmShortCourseController sut,
        GetStandardInformationQueryResult queryResult,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var expectedCourseType = CourseType.ShortCourse;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == sessionModel.LarsCode), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = await sut.ConfirmShortCourse(expectedCourseType);

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as ConfirmShortCourseViewModel;
        model.Should().NotBeNull();
        model!.ShortCourseInformation.Should().BeEquivalentTo(queryResult, o => o.ExcludingMissingMembers());
        model!.CourseType.Should().Be(expectedCourseType);
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == sessionModel.LarsCode), It.IsAny<CancellationToken>()), Times.Once);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.ShortCourseInformation == model.ShortCourseInformation)), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task ConfirmShortCourse_SessionIsNull_RedirectsToReviewYourDetails(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] ConfirmShortCourseController sut,
    ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var expectedCourseType = CourseType.ShortCourse;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(() => null);

        // Act
        var response = await sut.ConfirmShortCourse(expectedCourseType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == sessionModel.LarsCode), It.IsAny<CancellationToken>()), Times.Never);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<ShortCourseSessionModel>()), Times.Never);
    }
}