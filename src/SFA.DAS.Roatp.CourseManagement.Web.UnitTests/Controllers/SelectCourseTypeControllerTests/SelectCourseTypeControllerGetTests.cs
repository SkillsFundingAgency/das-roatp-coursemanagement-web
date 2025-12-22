using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.SelectCourseTypeControllerTests;
public class SelectCourseTypeControllerGetTests
{
    [Test, MoqAutoData]
    public async Task Get_Index_GetProviderCourseTypeReturnsReturnsApprenticeshipAndApprenticeshipUnitCourseTypes_ReturnsView(
        [Frozen] Mock<IProviderCourseTypeService> providerCourseTypeService)
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, "111") }, "mock"));

        var courseTypes = new List<CourseTypeModel>()
        {
            new CourseTypeModel()
            {
                CourseType = CourseType.Apprenticeship.ToString()
            },
            new CourseTypeModel()
            {
                CourseType = CourseType.ApprenticeshipUnit.ToString()
            }
        };

        providerCourseTypeService.Setup(c => c.GetProviderCourseType(It.IsAny<int>())).ReturnsAsync(courseTypes);

        var sut = new SelectCourseTypeController(providerCourseTypeService.Object)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user },
            },
        };

        var viewStandardsLink = Guid.NewGuid().ToString();
        var selectCourseTypeLink = Guid.NewGuid().ToString();
        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ViewStandards, viewStandardsLink)
            .AddUrlForRoute(RouteNames.SelectCourseType, selectCourseTypeLink);

        // Act
        var result = await sut.Index() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().NotBeNull();
        var model = result!.Model as SelectCourseTypeViewModel;
        model!.ApprenticeshipsUrl.Should().Be(viewStandardsLink);
        model!.ApprenticeshipUnitsUrl.Should().Be(selectCourseTypeLink);
        providerCourseTypeService.Verify(c => c.GetProviderCourseType(It.IsAny<int>()), Times.Once);
    }

    [Test]
    [MoqInlineAutoData("Apprenticeship")]
    [MoqInlineAutoData("ApprenticeshipUnit")]
    public async Task Get_Index_GetProviderCourseTypeReturnsReturnsOnlyOneCourseType_RedirectsToCorrectAction(
        string courseType,
        [Frozen] Mock<IProviderCourseTypeService> providerCourseTypeService)
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, "111") }, "mock"));

        var courseTypes = new List<CourseTypeModel>()
        {
            new CourseTypeModel()
            {
                CourseType = courseType
            }
        };

        providerCourseTypeService.Setup(c => c.GetProviderCourseType(It.IsAny<int>())).ReturnsAsync(courseTypes);

        var sut = new SelectCourseTypeController(providerCourseTypeService.Object)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user },
            },
        };

        // Act
        var result = await sut.Index();

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ViewStandards);
        providerCourseTypeService.Verify(c => c.GetProviderCourseType(It.IsAny<int>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Get_Index_GetProviderCourseTypeReturnsEmpty_RedirectsToCorrectAction(
        [Frozen] Mock<IProviderCourseTypeService> providerCourseTypeService)
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, "111") }, "mock"));

        var courseTypes = new List<CourseTypeModel>();

        providerCourseTypeService.Setup(c => c.GetProviderCourseType(It.IsAny<int>())).ReturnsAsync(courseTypes);

        var sut = new SelectCourseTypeController(providerCourseTypeService.Object)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user },
            },
        };

        // Act
        var result = await sut.Index();

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        providerCourseTypeService.Verify(c => c.GetProviderCourseType(It.IsAny<int>()), Times.Once);
    }
}
