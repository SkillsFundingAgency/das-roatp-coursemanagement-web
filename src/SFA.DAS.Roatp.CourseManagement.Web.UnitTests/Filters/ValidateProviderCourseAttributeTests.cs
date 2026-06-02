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
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Filters;

public class ValidateProviderCourseAttributeTests
{
    [Test, MoqAutoData]
    public async Task When_LarsCodeExistsAndCourseTypeMatches_Then_ShouldNotRedirect(
        [Frozen] Mock<IProviderCourseDetailsService> providerCourseDetailsServiceMock,
        CourseType courseType,
        GetProviderCourseDetailsQueryResult apiResponse,
        int ukprn,
        string larsCode)
    {
        // Arrange
        apiResponse.CourseType = courseType;

        providerCourseDetailsServiceMock.Setup(p => p.GetProviderCourseDetails(ukprn, larsCode)).ReturnsAsync(apiResponse);

        var actionContext = CreateActionContext(providerCourseDetailsServiceMock.Object, ukprn, larsCode);

        var filterContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), controller: new object());

        ValidateProviderCourseAttribute sut = new(courseType);

        // Act
        await sut.OnActionExecutionAsync(filterContext, () =>
        Task.FromResult(new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), new object())));

        // Assert
        filterContext.Result.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task When_LarsCodeDoesNotExistInRoute_Then_ShouldRedirectToPageNotFound(
        [Frozen] Mock<IProviderCourseDetailsService> providerCourseDetailsServiceMock,
        CourseType courseType,
        GetProviderCourseDetailsQueryResult apiResponse,
        int ukprn,
        string larsCode)
    {
        // Arrange
        apiResponse.CourseType = courseType;

        providerCourseDetailsServiceMock.Setup(p => p.GetProviderCourseDetails(ukprn, larsCode)).ReturnsAsync(apiResponse);

        var actionContext = CreateActionContext(providerCourseDetailsServiceMock.Object, ukprn, null);

        var filterContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), controller: new object());

        ValidateProviderCourseAttribute sut = new(courseType);

        // Act
        await sut.OnActionExecutionAsync(filterContext, () =>
        Task.FromResult(new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), new object())));

        // Assert
        var viewResult = filterContext.Result as ViewResult;
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }

    [Test, MoqAutoData]
    public async Task When_CourseTypeDoesNotMatch_Then_ShouldRedirectToPageNotFound(
        [Frozen] Mock<IProviderCourseDetailsService> providerCourseDetailsServiceMock,
        GetProviderCourseDetailsQueryResult apiResponse,
        int ukprn,
        string larsCode)
    {
        // Arrange
        var courseType = CourseType.ShortCourse;
        apiResponse.CourseType = CourseType.Apprenticeship;

        providerCourseDetailsServiceMock.Setup(p => p.GetProviderCourseDetails(ukprn, larsCode)).ReturnsAsync(apiResponse);

        var actionContext = CreateActionContext(providerCourseDetailsServiceMock.Object, ukprn, larsCode);

        var filterContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), controller: new object());

        ValidateProviderCourseAttribute sut = new(courseType);

        // Act
        await sut.OnActionExecutionAsync(filterContext, () =>
        Task.FromResult(new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), new object())));

        // Assert
        var viewResult = filterContext.Result as ViewResult;
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }

    [Test, MoqAutoData]
    public async Task When_ApiReturnsNull_Then_ShouldRedirectToPageNotFound(
        [Frozen] Mock<IProviderCourseDetailsService> providerCourseDetailsServiceMock,
        CourseType courseType,
        int ukprn,
        string larsCode)
    {
        // Arrange
        providerCourseDetailsServiceMock.Setup(p => p.GetProviderCourseDetails(ukprn, larsCode)).ReturnsAsync((GetProviderCourseDetailsQueryResult)null);

        var actionContext = CreateActionContext(providerCourseDetailsServiceMock.Object, ukprn, larsCode);

        var filterContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), controller: new object());

        ValidateProviderCourseAttribute sut = new(courseType);

        // Act
        await sut.OnActionExecutionAsync(filterContext, () =>
        Task.FromResult(new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), new object())));

        // Assert
        var viewResult = filterContext.Result as ViewResult;
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }

    private static ActionContext CreateActionContext(IProviderCourseDetailsService providerCourseDetailsService, int ukprn, string larsCode)
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
                    .AddSingleton<ILogger<ValidateProviderCourseAttribute>>(NullLogger<ValidateProviderCourseAttribute>.Instance)
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
