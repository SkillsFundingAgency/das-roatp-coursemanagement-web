using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Humanizer;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.ManageShortCoursesControllerTests;
public class ManageShortCoursesControllerGetTests
{
    [Test, MoqAutoData]
    public async Task Index_CourseTypeReturnsShortCourse_ReturnsView(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ManageShortCoursesController sut,
        GetAllProviderStandardsQueryResult queryResult)
    {
        // Arrange
        var expectedApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var expectedStandardDetailsLink = Guid.NewGuid().ToString();

        object isShortCourseDeleted = null;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.ShowShortCourseDeletedBannerTempDataKey, out isShortCourseDeleted));
        sut.TempData = tempDataMock.Object;

        mediatorMock.Setup(x => x.Send(It.IsAny<GetAllProviderStandardsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sut.AddDefaultContextWithUser();

        var selectShortCourseUrl = RouteNames.SelectShortCourse;
        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.SelectShortCourse, selectShortCourseUrl)
            .AddUrlForRoute(RouteNames.ManageShortCourseDetails, expectedStandardDetailsLink);

        // Act
        var result = await sut.Index(expectedApprenticeshipType) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().NotBeNull();
        var model = result!.Model as ManageShortCoursesViewModel;
        model.ShortCourses.First().StandardUrl.Should().Be(expectedStandardDetailsLink);
        model!.AddAShortCourseLink.Should().Be(selectShortCourseUrl);
        model!.ApprenticeshipType.Should().Be(expectedApprenticeshipType);
        model!.ShowShortCourseHeading.Should().BeTrue();
        model!.ShowDeleteShortCourseNotificationBanner.Should().BeFalse();
        tempDataMock.Verify(x => x.TryGetValue(TempDataKeys.ShowShortCourseDeletedBannerTempDataKey, out isShortCourseDeleted), Times.Once);
        tempDataMock.Verify(t => t.Remove(TempDataKeys.ShowShortCourseDeletedBannerTempDataKey), Times.Never);
        mediatorMock.Verify(x => x.Send(It.IsAny<GetAllProviderStandardsQuery>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task Index_TempDataRetursShowDeleteBannerAsTrue_ReturnsViewWithBanner(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ManageShortCoursesController sut,
        GetAllProviderStandardsQueryResult queryResult)
    {
        // Arrange
        var expectedApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var expectedStandardDetailsLink = Guid.NewGuid().ToString();

        object isShortCourseDeleted = true;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.ShowShortCourseDeletedBannerTempDataKey, out isShortCourseDeleted));
        sut.TempData = tempDataMock.Object;

        mediatorMock.Setup(x => x.Send(It.IsAny<GetAllProviderStandardsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sut.AddDefaultContextWithUser();

        var selectShortCourseUrl = RouteNames.SelectShortCourse;
        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.SelectShortCourse, selectShortCourseUrl)
            .AddUrlForRoute(RouteNames.ManageShortCourseDetails, expectedStandardDetailsLink);

        // Act
        var result = await sut.Index(expectedApprenticeshipType) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().NotBeNull();
        var model = result!.Model as ManageShortCoursesViewModel;
        model.ShortCourses.First().StandardUrl.Should().Be(expectedStandardDetailsLink);
        model!.AddAShortCourseLink.Should().Be(selectShortCourseUrl);
        model!.ApprenticeshipType.Should().Be(expectedApprenticeshipType);
        model!.ShowShortCourseHeading.Should().BeTrue();
        model!.ShowDeleteShortCourseNotificationBanner.Should().BeTrue();
        model!.Banner.ApprenticeshipType.Should().Be(expectedApprenticeshipType.Humanize());
        tempDataMock.Verify(x => x.TryGetValue(TempDataKeys.ShowShortCourseDeletedBannerTempDataKey, out isShortCourseDeleted), Times.Once);
        tempDataMock.Verify(t => t.Remove(TempDataKeys.ShowShortCourseDeletedBannerTempDataKey), Times.Once);
        mediatorMock.Verify(x => x.Send(It.IsAny<GetAllProviderStandardsQuery>(), It.IsAny<CancellationToken>()), Times.Once());
    }
}
