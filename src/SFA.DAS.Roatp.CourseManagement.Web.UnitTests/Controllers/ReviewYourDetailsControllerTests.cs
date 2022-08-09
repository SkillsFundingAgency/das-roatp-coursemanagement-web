using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using System.Security.Claims;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers
{
    [TestFixture]
    public class ReviewYourDetailsControllerTests
    {
        [Test]
        public void Index_ReturnsViewWithModel()
        {
            var mockOptions = new Mock<IOptions<ProviderSharedUIConfiguration>>();
            ProviderSharedUIConfiguration config = new ProviderSharedUIConfiguration() { DashboardUrl = @"https://dashboard.com" };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{new Claim(ProviderClaims.ProviderUkprn,"111")}, "mock"));
            Mock<IUrlHelper> urlHelper = new Mock<IUrlHelper>();
            string verifyUrl = "http://test";
            string providerDescriptionUrl = "http://test/provider-description";
            UrlRouteContext verifyRouteValues = null;
            urlHelper
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c =>
                   c.RouteName.Equals(RouteNames.ViewStandards)
               )))
               .Returns(verifyUrl)
               .Callback<UrlRouteContext>(c =>
               {
                   verifyRouteValues = c;
               });
            urlHelper
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c =>
                   c.RouteName.Equals(RouteNames.GetProviderLocations)
               )))
               .Returns(verifyUrl)
               .Callback<UrlRouteContext>(c =>
               {
                   verifyRouteValues = c;
               });

            urlHelper
                .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c =>
                    c.RouteName.Equals(RouteNames.ProviderDescription)
                )))
                .Returns(providerDescriptionUrl)
                .Callback<UrlRouteContext>(c =>
                {
                    verifyRouteValues = c;
                });
            var expectedModel = new ReviewYourDetailsViewModel()
            {
                BackUrl = config.DashboardUrl,
                ProviderLocationsUrl = verifyUrl,
                StandardsUrl = verifyUrl,
                ProviderDescriptionUrl = providerDescriptionUrl
            };
            mockOptions.Setup(o => o.Value).Returns(config);
            var sut = new ReviewYourDetailsController(mockOptions.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user },
                },
            };
            sut.Url = urlHelper.Object;

            var result = sut.ReviewYourDetails() as ViewResult;
            result.Should().NotBeNull();
            result.ViewName.Should().Contain(nameof(ReviewYourDetailsController.ReviewYourDetails));
            result.Model.Should().BeEquivalentTo(expectedModel);
        }
    }
}
