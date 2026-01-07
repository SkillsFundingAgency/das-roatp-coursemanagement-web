using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.SelectCourseTypeControllerTests;
public class SelectCourseTypeControllerGetTests
{
    [Test, MoqAutoData]
    public async Task Index_CourseTypeReturnsBothApprenticeshipAndApprenticeshipUnits_ReturnsView(
        [Frozen] Mock<IProviderCourseTypeService> providerCourseTypeService,
        [Greedy] SelectCourseTypeController sut)
    {
        // Arrange
        var courseTypes = new List<CourseTypeModel>()
        {
            new CourseTypeModel()
            {
                CourseType = CourseType.Apprenticeship
            },
            new CourseTypeModel()
            {
                CourseType = CourseType.ApprenticeshipUnit
            }
        };

        providerCourseTypeService.Setup(c => c.GetProviderCourseType(It.IsAny<int>())).ReturnsAsync(courseTypes);

        sut.AddDefaultContextWithUser();

        var viewStandardsLink = Guid.NewGuid().ToString();
        var manageApprenticeshipUnitsLink = Guid.NewGuid().ToString();
        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ViewStandards, viewStandardsLink)
            .AddUrlForRoute(RouteNames.ManageApprenticeshipUnits, manageApprenticeshipUnitsLink);

        // Act
        var result = await sut.Index() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().NotBeNull();
        var model = result!.Model as SelectCourseTypeViewModel;
        model!.ApprenticeshipsUrl.Should().Be(viewStandardsLink);
        model!.ApprenticeshipUnitsUrl.Should().Be(manageApprenticeshipUnitsLink);
        providerCourseTypeService.Verify(c => c.GetProviderCourseType(It.IsAny<int>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Index_CourseTypeReturnsApprenticeshipOnly_RedirectsToViewStandards(
        string courseType,
        [Frozen] Mock<IProviderCourseTypeService> providerCourseTypeService,
        [Greedy] SelectCourseTypeController sut)
    {
        // Arrange
        var courseTypes = new List<CourseTypeModel>()
        {
            new CourseTypeModel()
            {
                CourseType = CourseType.Apprenticeship
            }
        };

        providerCourseTypeService.Setup(c => c.GetProviderCourseType(It.IsAny<int>())).ReturnsAsync(courseTypes);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.Index();

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ViewStandards);
        providerCourseTypeService.Verify(c => c.GetProviderCourseType(It.IsAny<int>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Index_CourseTypeReturnsApprenticeshipUnitsOnly_RedirectsToManageApprenticeshipUnits(
        string courseType,
        [Frozen] Mock<IProviderCourseTypeService> providerCourseTypeService,
        [Greedy] SelectCourseTypeController sut)
    {
        // Arrange
        var courseTypes = new List<CourseTypeModel>()
        {
            new CourseTypeModel()
            {
                CourseType = CourseType.ApprenticeshipUnit
            }
        };

        providerCourseTypeService.Setup(c => c.GetProviderCourseType(It.IsAny<int>())).ReturnsAsync(courseTypes);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.Index();

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ManageApprenticeshipUnits);
        providerCourseTypeService.Verify(c => c.GetProviderCourseType(It.IsAny<int>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Index_CourseTypeReturnsEmpty_RedirectsToReviewYourDetails(
        [Frozen] Mock<IProviderCourseTypeService> providerCourseTypeService,
        [Greedy] SelectCourseTypeController sut)
    {
        // Arrange
        var courseTypes = new List<CourseTypeModel>();

        providerCourseTypeService.Setup(c => c.GetProviderCourseType(It.IsAny<int>())).ReturnsAsync(courseTypes);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.Index();

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        providerCourseTypeService.Verify(c => c.GetProviderCourseType(It.IsAny<int>()), Times.Once);
    }
}
