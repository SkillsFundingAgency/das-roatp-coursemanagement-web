using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using System.Security.Claims;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers
{
    [TestFixture]
    public class ReviewYourDetailsControllerTests
    {
        [Test]
        public void Index_ReturnsViewWithModel()
        {
            var mockOptions = new Mock<IOptions<ProviderSharedUIConfiguration>>();
            var config = new ProviderSharedUIConfiguration() { DashboardUrl = @"https://dashboard.com" };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{new Claim(ProviderClaims.ProviderUkprn,"111")}, "mock"));
           
            var  viewStandardsUrl = "http://test/view-standards";
            var providerLocationsUrl = "http://test/provider-locations";
            var providerDescriptionUrl = "http://test/provider-description";
            
            var expectedModel = new ReviewYourDetailsViewModel()
            {
                BackUrl = config.DashboardUrl,
                ProviderLocationsUrl = providerLocationsUrl,
                StandardsUrl = viewStandardsUrl,
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
            
            sut.AddDefaultContextWithUser()
            .AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ViewStandards, viewStandardsUrl)
            .AddUrlForRoute(RouteNames.GetProviderLocations,providerLocationsUrl)
            .AddUrlForRoute(RouteNames.GetProviderDescription, providerDescriptionUrl);

            var result = sut.ReviewYourDetails() as ViewResult;
            result.Should().NotBeNull();
            result.ViewName.Should().Contain(nameof(ReviewYourDetailsController.ReviewYourDetails));
            result.Model.Should().BeEquivalentTo(expectedModel);
        }
    }
}
