using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Common;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Factories;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.SelectShortCourseControllerTests;

public class SelectShortCourseControllerGetTests
{
    [Test, MoqAutoData]
    public async Task SelectShortCourse_ReturnsView(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] SelectShortCourseController sut,
        GetAvailableProviderStandardsQueryResult queryResult)
    {
        // Arrange
        var learningType = LearningType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetAvailableProviderStandardsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.CourseType == CourseType.ShortCourse), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        // Act
        var response = await sut.SelectShortCourse(learningType);

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as SelectShortCourseViewModel;
        model.Should().NotBeNull();
        model!.ShortCourses.Should().BeEquivalentTo(queryResult.AvailableCourses, o => o.ExcludingMissingMembers());
        model!.LearningType.Should().Be(learningType);
        mediatorMock.Verify(m => m.Send(It.Is<GetAvailableProviderStandardsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.CourseType == CourseType.ShortCourse), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task SelectShortCourse_ShortCoursesNotAvailable_RedirectsToShortCoursesNotAvailable(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] SelectShortCourseController sut)
    {
        // Arrange
        var learningType = LearningType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetAvailableProviderStandardsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.CourseType == CourseType.ShortCourse), It.IsAny<CancellationToken>())).ReturnsAsync(new GetAvailableProviderStandardsQueryResult());

        // Act
        var response = await sut.SelectShortCourse(learningType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ShortCoursesNotAvailable);
    }

    [Test, MoqAutoData]
    public async Task CoursesNotAvailable_ReturnsView(
        [Frozen] Mock<IBreadcrumbsViewModelFactory> breadcrumbsFactoryMock,
        [Greedy] SelectShortCourseController sut)
    {
        // Arrange
        var expectedLearningType = LearningType.ApprenticeshipUnit;

        var expectedBreadcrumbs = new BreadcrumbsViewModel();
        expectedBreadcrumbs.Add(BreadcrumbNames.ProviderDashboard, "https://test/dashboard");
        expectedBreadcrumbs.Add(BreadcrumbNames.ManageYourCourses, "https://test/review-your-details");

        breadcrumbsFactoryMock
            .Setup(f => f.Create(It.IsAny<IUrlHelper>(), It.IsAny<BreadcrumbsViewModelFactoryModel>()))
            .Returns(expectedBreadcrumbs);

        sut.AddDefaultContextWithUser();

        // Act
        var response = sut.CoursesNotAvailable(expectedLearningType) as ViewResult;

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response!.Model.Should().NotBeNull();
            var model = response!.Model as ShortCourseShutterPagesViewModel;
            model.LearningType.Should().Be(expectedLearningType);
            model.Breadcrumbs.Should().BeEquivalentTo(expectedBreadcrumbs);
        }
    }
}