using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderContact.Queries;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.SelectShortCourseControllerTests;
public class SelectShortCourseControllerPostTests
{
    [Test, MoqAutoData]
    public async Task SelectShortCourse_InvalidState_ReturnsView(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] SelectShortCourseController sut,
            GetAvailableProviderStandardsQueryResult queryResult)
    {
        // Arrange
        var courseType = CourseType.ShortCourse;

        sut.AddDefaultContextWithUser();
        sut.ModelState.AddModelError("key", "message");

        mediatorMock.Setup(m => m.Send(It.Is<GetAvailableProviderStandardsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.CourseType == CourseType.ShortCourse), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        // Act
        var response = await sut.SelectShortCourse(new SelectShortCourseSubmitModel() { CourseType = courseType }, courseType);

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as SelectShortCourseViewModel;
        model.Should().NotBeNull();
        model!.ShortCourses.Should().BeEquivalentTo(queryResult.AvailableCourses, o => o.ExcludingMissingMembers());
        model!.CourseType.Should().Be(courseType);
        mediatorMock.Verify(m => m.Send(It.Is<GetAvailableProviderStandardsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.CourseType == CourseType.ShortCourse), It.IsAny<CancellationToken>()), Times.Once());
        sessionServiceMock.Verify(s => s.Set(It.IsAny<ShortCourseSessionModel>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task SelectShortCourse_ValidState_SetsSessionAndRedirectsToConfirmApprenticeshipUnit(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<ISessionService> sessionServiceMock,
            [Greedy] SelectShortCourseController sut,
            GetLatestProviderContactQueryResult queryResult)
    {
        // Arrange
        var courseType = CourseType.ShortCourse;

        var submitModel = new SelectShortCourseSubmitModel() { CourseType = courseType };

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetLatestProviderContactQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        // Act
        var response = await sut.SelectShortCourse(submitModel, courseType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ConfirmShortCourse);
        mediatorMock.Verify(m => m.Send(It.Is<GetAvailableProviderStandardsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.CourseType == CourseType.ShortCourse), It.IsAny<CancellationToken>()), Times.Never());
        mediatorMock.Verify(m => m.Send(It.Is<GetLatestProviderContactQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Once());
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.LarsCode == submitModel.SelectedLarsCode && m.SavedProviderContactModel!.EmailAddress == queryResult.EmailAddress && m.SavedProviderContactModel!.PhoneNumber == queryResult.PhoneNumber)), Times.Once);
    }
}