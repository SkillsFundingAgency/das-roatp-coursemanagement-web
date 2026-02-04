using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.ManageShortCoursesControllerTests;
public class ManageShortCoursesControllerGetTests
{
    [Test, MoqAutoData]
    public async Task Index_CourseTypeReturnsApprenticeshipUnit_ReturnsView(
        [Frozen] Mock<IProviderCourseTypeService> providerCourseTypeService,
        [Greedy] ManageShortCoursesController sut)
    {
        // Arrange
        var courseTypes = new List<CourseTypeModel>()
        {
            new CourseTypeModel()
            {
                CourseType = CourseType.ShortCourse
            }
        };

        var expectedCourseType = CourseType.ShortCourse;

        providerCourseTypeService.Setup(c => c.GetProviderCourseType(It.IsAny<int>())).ReturnsAsync(courseTypes);

        sut.AddDefaultContextWithUser();

        var selectShortCourseUrl = RouteNames.SelectShortCourse;
        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.SelectShortCourse, selectShortCourseUrl);

        // Act
        var result = await sut.Index(expectedCourseType) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().NotBeNull();
        var model = result!.Model as ManageShortCoursesViewModel;
        model!.AddAShortCourseLink.Should().Be(selectShortCourseUrl);
        model!.CourseType.Should().Be(expectedCourseType);
        providerCourseTypeService.Verify(c => c.GetProviderCourseType(It.IsAny<int>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Index_CourseTypeDoesNotReturnApprenticeshipUnit_RedirectsToReviewYourDetails(
        [Frozen] Mock<IProviderCourseTypeService> providerCourseTypeService,
        [Greedy] ManageShortCoursesController sut)
    {
        // Arrange
        var courseTypes = new List<CourseTypeModel>()
        {
            new CourseTypeModel()
            {
                CourseType = CourseType.Apprenticeship
            }
        };

        var expectedCourseType = CourseType.ShortCourse;

        providerCourseTypeService.Setup(c => c.GetProviderCourseType(It.IsAny<int>())).ReturnsAsync(courseTypes);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.Index(expectedCourseType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        providerCourseTypeService.Verify(c => c.GetProviderCourseType(It.IsAny<int>()), Times.Once);
    }
}
