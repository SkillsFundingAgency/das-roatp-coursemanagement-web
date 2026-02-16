using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Filters;
public class AuthorizeCourseTypeAttributeTests
{
    [Test, MoqAutoData]
    public async Task OnAuthorisation_WhenProviderCourseTypeExists_ShouldNotRedirect(
        [Frozen] Mock<IProviderCourseTypeService> providerCourseTypeServiceMock,
        CourseType courseType,
        List<CourseTypeModel> providerCourseType,
        int ukprn)
    {
        // Arrange

        providerCourseTypeServiceMock.Setup(p => p.GetProviderCourseType(ukprn)).ReturnsAsync(providerCourseType);

        var actionContext = new ActionContext
        {
            HttpContext = new DefaultHttpContext
            {
                RequestServices = new ServiceCollection()
                .AddSingleton(providerCourseTypeServiceMock.Object)
                .BuildServiceProvider(),
                User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ProviderClaims.ProviderUkprn, ukprn.ToString())], "12345"))
            },
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor(),
        };

        var filterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

        AuthorizeCourseTypeAttribute sut = new(courseType);

        // Act
        await sut.OnAuthorizationAsync(filterContext);

        // Assert
        filterContext.Result.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task OnAuthorisation_WhenProviderCourseTypeDoesNotExist_ShouldRedirectToPageNotFoundView(
    [Frozen] Mock<IProviderCourseTypeService> providerCourseTypeServiceMock,
        CourseType courseType,
        int ukprn)
    {
        // Arrange
        providerCourseTypeServiceMock.Setup(p => p.GetProviderCourseType(ukprn)).ReturnsAsync(new List<CourseTypeModel>());

        var actionContext = new ActionContext
        {
            HttpContext = new DefaultHttpContext
            {
                RequestServices = new ServiceCollection()
                .AddSingleton(providerCourseTypeServiceMock.Object)
                .BuildServiceProvider(),
                User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ProviderClaims.ProviderUkprn, ukprn.ToString())], "12345"))
            },
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor(),
        };

        var filterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

        AuthorizeCourseTypeAttribute sut = new(courseType);

        // Act
        await sut.OnAuthorizationAsync(filterContext);

        // Assert
        filterContext.Result.Should().BeOfType<ViewResult>();
        var viewResult = filterContext.Result! as ViewResult;
        viewResult.ViewName.Should().Be("~/Views/Error/PageNotFound.cshtml");
    }
}
