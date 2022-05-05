using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using System.Security.Claims;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers
{
    [TestFixture]
    public class ReviewYourDetailsControllerTests
    {
        private Mock<HttpContext> _httpContext;
        [Test]
        public void Index_ReturnsViewWithModel()
        {
            var mockOptions = new Mock<IOptions<ProviderSharedUIConfiguration>>();
            ProviderSharedUIConfiguration config = new ProviderSharedUIConfiguration() { DashboardUrl = @"https://dashboard.com" };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{new Claim(ProviderClaims.ProviderUkprn,"111")}, "mock"));
            _httpContext = new Mock<HttpContext>();
            _httpContext.Setup(a => a.User).Returns(user);
            var expectedModel = new ReviewYourDetailsViewModel(_httpContext.Object)
            {
                DashboardUrl = config.DashboardUrl,
            };
            mockOptions.Setup(o => o.Value).Returns(config);
            var sut = new ReviewYourDetailsController(mockOptions.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user },
                },
            };

            var result = sut.ReviewYourDetails() as ViewResult;
            result.Should().NotBeNull();
            result.ViewName.Should().Contain(nameof(ReviewYourDetailsController.ReviewYourDetails));
            result.Model.Should().BeEquivalentTo(expectedModel);
        }
    }
}
