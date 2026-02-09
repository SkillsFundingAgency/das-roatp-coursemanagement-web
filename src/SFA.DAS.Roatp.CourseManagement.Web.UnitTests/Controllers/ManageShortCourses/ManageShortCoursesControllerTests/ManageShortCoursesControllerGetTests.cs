using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
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
    public void Index_CourseTypeReturnsShortCourse_ReturnsView(
        [Greedy] ManageShortCoursesController sut)
    {
        // Arrange
        var expectedApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        var selectShortCourseUrl = RouteNames.SelectShortCourse;
        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.SelectShortCourse, selectShortCourseUrl);

        // Act
        var result = sut.Index(expectedApprenticeshipType) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().NotBeNull();
        var model = result!.Model as ManageShortCoursesViewModel;
        model!.AddAShortCourseLink.Should().Be(selectShortCourseUrl);
        model!.ApprenticeshipType.Should().Be(expectedApprenticeshipType);
    }
}
