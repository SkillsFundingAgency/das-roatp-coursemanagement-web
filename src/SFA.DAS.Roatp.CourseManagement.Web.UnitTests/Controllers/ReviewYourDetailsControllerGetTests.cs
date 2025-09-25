using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers
{
    [TestFixture]
    public class ReviewYourDetailsControllerGetTests
    {
        [Test]
        public void Index_ReturnsViewWithModel()
        {
            var mockSessionService = new Mock<ISessionService>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, "111") }, "mock"));

            var viewStandardsUrl = "http://test/view-standards";
            var providerLocationsUrl = "http://test/provider-locations";
            var providerDescriptionUrl = "http://test/provider-description";
            var providerContactUrl = "http://test/provider-contact";

            var expectedModel = new ReviewYourDetailsViewModel()
            {
                BackUrl = null,
                ProviderLocationsUrl = providerLocationsUrl,
                StandardsUrl = viewStandardsUrl,
                ProviderDescriptionUrl = providerDescriptionUrl,
                ProviderContactUrl = providerContactUrl
            };

            var sut = new ReviewYourDetailsController(mockSessionService.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user },
                },
            };

            sut.AddDefaultContextWithUser()
            .AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ViewStandards, viewStandardsUrl)
            .AddUrlForRoute(RouteNames.GetProviderLocations, providerLocationsUrl)
            .AddUrlForRoute(RouteNames.GetProviderDescription, providerDescriptionUrl)
            .AddUrlForRoute(RouteNames.CheckProviderContactDetails, providerContactUrl);

            var result = sut.ReviewYourDetails() as ViewResult;
            result.Should().NotBeNull();
            result!.ViewName.Should().Contain(nameof(ReviewYourDetailsController.ReviewYourDetails));
            result.Model.Should().BeEquivalentTo(expectedModel);
            mockSessionService.Verify(x => x.Delete(nameof(ProviderContactSessionModel)), Times.Once);
        }
    }
}
