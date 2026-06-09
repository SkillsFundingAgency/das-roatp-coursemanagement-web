using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Filters;

public class CheckCourseTypesAttributeTests
{
    [Test, MoqAutoData]
    public async Task WhenHandlingRequest_LarsCodeExistsAndCourseTypeMatches_ThenShouldNotRedirect(
        [Frozen] Mock<IProviderCourseDetailsCachedService> providerCourseDetailsServiceMock,
        CourseType courseType,
        StandardDetails apiResponse,
        int ukprn,
        string larsCode)
    {
        // Arrange
        apiResponse.CourseType = courseType;

        providerCourseDetailsServiceMock.Setup(p => p.GetCachedProviderCourseDetails(ukprn, larsCode)).ReturnsAsync(apiResponse);

        var actionContext = CreateActionContext(providerCourseDetailsServiceMock.Object, ukprn, larsCode);

        var filterContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), controller: new object());

        CheckCourseTypeAttribute sut = new(courseType);

        // Act
        await sut.OnActionExecutionAsync(filterContext, () =>
        Task.FromResult(new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), new object())));

        // Assert
        filterContext.Result.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task WhenHandlingRequest_LarsCodeDoesNotExistInRoute_ThenShouldRedirectToPageNotFound(
        [Frozen] Mock<IProviderCourseDetailsCachedService> providerCourseDetailsServiceMock,
        CourseType courseType,
        StandardDetails apiResponse,
        int ukprn,
        string larsCode)
    {
        // Arrange
        apiResponse.CourseType = courseType;

        providerCourseDetailsServiceMock.Setup(p => p.GetCachedProviderCourseDetails(ukprn, larsCode)).ReturnsAsync(apiResponse);

        var actionContext = CreateActionContext(providerCourseDetailsServiceMock.Object, ukprn, null);

        var filterContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), controller: new object());

        CheckCourseTypeAttribute sut = new(courseType);

        // Act
        await sut.OnActionExecutionAsync(filterContext, () =>
        Task.FromResult(new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), new object())));

        // Assert
        var viewResult = filterContext.Result as ViewResult;
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }

    [Test, MoqAutoData]
    public async Task WhenHandlingRequest_CourseTypeDoesNotMatch_ThenShouldRedirectToPageNotFound(
        [Frozen] Mock<IProviderCourseDetailsCachedService> providerCourseDetailsServiceMock,
        StandardDetails apiResponse,
        int ukprn,
        string larsCode)
    {
        // Arrange
        var courseType = CourseType.ShortCourse;
        apiResponse.CourseType = CourseType.Apprenticeship;

        providerCourseDetailsServiceMock.Setup(p => p.GetCachedProviderCourseDetails(ukprn, larsCode)).ReturnsAsync(apiResponse);

        var actionContext = CreateActionContext(providerCourseDetailsServiceMock.Object, ukprn, larsCode);

        var filterContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), controller: new object());

        CheckCourseTypeAttribute sut = new(courseType);

        // Act
        await sut.OnActionExecutionAsync(filterContext, () =>
        Task.FromResult(new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), new object())));

        // Assert
        var viewResult = filterContext.Result as ViewResult;
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }

    [Test, MoqAutoData]
    public async Task WhenHandlingRequest_ApiReturnsNull_ThenShouldRedirectToPageNotFound(
        [Frozen] Mock<IProviderCourseDetailsCachedService> providerCourseDetailsServiceMock,
        CourseType courseType,
        int ukprn,
        string larsCode)
    {
        // Arrange
        providerCourseDetailsServiceMock.Setup(p => p.GetCachedProviderCourseDetails(ukprn, larsCode)).ReturnsAsync((StandardDetails)null);

        var actionContext = CreateActionContext(providerCourseDetailsServiceMock.Object, ukprn, larsCode);

        var filterContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), controller: new object());

        CheckCourseTypeAttribute sut = new(courseType);

        // Act
        await sut.OnActionExecutionAsync(filterContext, () =>
        Task.FromResult(new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), new object())));

        // Assert
        var viewResult = filterContext.Result as ViewResult;
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }

    private static ActionContext CreateActionContext(IProviderCourseDetailsCachedService providerCourseDetailsService, int ukprn, string larsCode)
    {
        var routeData = new RouteData();

        if (larsCode is not null)
        {
            routeData.Values["larsCode"] = larsCode;
        }

        return new ActionContext
        {
            HttpContext = new DefaultHttpContext
            {
                RequestServices = new ServiceCollection()
                    .AddSingleton(providerCourseDetailsService)
                    .AddSingleton<ILogger<CheckCourseTypeAttribute>>(NullLogger<CheckCourseTypeAttribute>.Instance)
                    .BuildServiceProvider(),
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        [new Claim(ProviderClaims.ProviderUkprn, ukprn.ToString())],
                        "12345"))
            },
            RouteData = routeData,
            ActionDescriptor = new ActionDescriptor(),
        };
    }
}
